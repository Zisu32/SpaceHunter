using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKLib;
using SpaceHunter.Models;
using SpaceHunter.Models.Enums;

namespace SpaceHunter;

public class PlayerMovement
{
    private readonly Camera _camera;
    private readonly GameState _state;
    private readonly KeyGroup _playerKeys;
    private readonly Keyboard _keyboard;

    private float _velocityY = 0f;
    private const float Gravity = -85f;
    private const float JumpVelocity = 37f;
    private bool _isJumpKeyHeld = false;

    private float _playerSpeed;
    private const float Acceleration = 55;
    private const float DecelerationPercent = .98f;
    private double _attackTime;
    private SimpleDirection _playerDirection = SimpleDirection.RIGHT;

    public PlayerMovement(GameState state, KeyGroup playerKeys, Keyboard keyboard, Camera camera)
    {
        _state = state;
        _playerKeys = playerKeys;
        _keyboard = keyboard;
        _camera = camera;
    }

    public void Update(FrameEventArgs frameArgs)
    {
        // movement stopped while attacking
        if (_attackTime <= 0)
        {
            ReadJumpKeyAndMoveCamera();

            if (_state.PlayerInAir)
            {
                JumpMovement(frameArgs);
            }
        }

        WalkingMovement(frameArgs);

        #region Attack

        if (_playerKeys.PressedKeys.Contains(Keys.F) && _attackTime <= 0 && !_state.PlayerInAir)
        {
            _attackTime = ConstantBalancingValues.AttackDuration;
            _playerKeys.RemovePressed(Keys.F); // F has to be pressed multiple times, for multiple attacks

            // stop movement
            _playerSpeed = 0;
        }

        if (_attackTime > 0)
        {
            Vector2 hitBoxMin;
            Vector2 hitBoxMax;
            if (_playerDirection == SimpleDirection.LEFT)
            {
                hitBoxMin = new Vector2(_state.PlayerBox.Min.X, _state.PlayerBox.Min.Y);
                hitBoxMax = new Vector2(_state.PlayerBox.Min.X - ConstantBalancingValues.AttackBoxLength,
                    _state.PlayerBox.Max.Y);
            }
            else
            {
                // just use Player to right as default case
                hitBoxMin = new Vector2(_state.PlayerBox.Max.X, _state.PlayerBox.Min.Y);
                hitBoxMax = new Vector2(_state.PlayerBox.Max.X + ConstantBalancingValues.AttackBoxLength,
                    _state.PlayerBox.Max.Y);
            }

            _state.PlayerHitBox = new Box2(hitBoxMin, hitBoxMax);

            _attackTime -= frameArgs.Time;
        }
        else // _attackTime <= 0
        {
            _state.PlayerHitBox = null;
        }

        #endregion

        #region Animation

        if (_attackTime > 0)
        {
            _state.PlayerState = _playerDirection == SimpleDirection.LEFT ? PlayerState.attack_l : PlayerState.attack_r;
            return;
        }

        if (_state.PlayerBox.Min.Y < 0.0001f)
        {
            if (_playerSpeed == 0)
            {
                // idle animation
                _state.PlayerState = _playerDirection == SimpleDirection.LEFT ? PlayerState.idle_l : PlayerState.idle_r;
            }
            else
            {
                // player run Animation
                _state.PlayerState = _playerDirection == SimpleDirection.LEFT ? PlayerState.run_l : PlayerState.run_r;
            }
        }
        else // jump
        {
            _state.PlayerState = _playerDirection == SimpleDirection.LEFT ? PlayerState.jump_l : PlayerState.jump_r;
        }

        #endregion
    }

    // TODO remove
    // frame time debug
    private Stopwatch _stopwatch = new Stopwatch();
    private double _accelerationAmount = 0;

    private void WalkingMovement(FrameEventArgs frameArgs)
    {
        // TODO playerSpeed Cap
        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;
        float deltaTime = (float)frameArgs.Time;
        float accelValue = Acceleration * deltaTime;
        
        if (_playerKeys.PressedKeys.Contains(Keys.D) && !_stopwatch.IsRunning)
        {
            _stopwatch.Start();
        }else if (_stopwatch.IsRunning && _playerKeys.PressedKeys.Contains(Keys.D))
        {
            _accelerationAmount += accelValue;
        }else if (!_playerKeys.PressedKeys.Contains(Keys.D) && _stopwatch.IsRunning)
        {
            _stopwatch.Stop();

            double accelPerMs = _accelerationAmount / _stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"accelPerMs: {accelPerMs}");
        }
        
        
        // Console.WriteLine($"accelValue: {accelValue}");

        // read keys and apply 'acceleration'
        if (_playerKeys.PressedKeys.Contains(Keys.A))
        {
            if (playerBoxMin.X < 0)
            {
                return;
            }

            // Console.WriteLine("speed sub");
            _playerSpeed -= accelValue;
            _playerDirection = SimpleDirection.LEFT;
        }
        else if (_playerKeys.PressedKeys.Contains(Keys.D))
        {
            if (playerBoxMax.X >= TextureManager.BackgroundRectangle.Max.X + TextureSizes.PlayerSizeX / 2)
            {
                return;
            }

            // Console.WriteLine("speed add");
            _playerSpeed += accelValue;
            _playerDirection = SimpleDirection.RIGHT;
        }

        float deccelValue = (Acceleration - 25) * deltaTime * Math.Sign(_playerSpeed) * -1;

        _playerSpeed += deccelValue;

        float playerMoveDistance = _playerSpeed * deltaTime;

        // apply velocity 
        if ( // if player is not moving or out of bounds, instantly stop
            MathF.Abs(_playerSpeed) < 0.005f // playerSpeed small
            || playerBoxMin.X + playerMoveDistance < 0f // out of bound 0
            || playerBoxMax.X + playerMoveDistance >=
            TextureManager.BackgroundRectangle.Max.X + TextureSizes.PlayerSizeX / 2) // out of bounds max
        {
            _playerSpeed = 0;
        }

        if (_playerSpeed != 0)
        {
            // moves player
            playerBoxMin.X += playerMoveDistance;
            playerBoxMax.X += playerMoveDistance;
        }

        _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);
    }

    private void JumpMovement(FrameEventArgs frameArgs)
    {
        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;

        // If jump key is released and player is moving upward, increase gravity to cut jump short
        if (!_isJumpKeyHeld && _velocityY > 0)
        {
            _velocityY += Gravity * 3f * (float)frameArgs.Time; // stronger gravity to cut jump
            if (_velocityY < 0)
                _velocityY = 0;
        }
        else
        {
            // normal gravity
            _velocityY += Gravity * (float)frameArgs.Time;
        }

        // Move player vertically
        playerBoxMin.Y += _velocityY * (float)frameArgs.Time;
        playerBoxMax.Y += _velocityY * (float)frameArgs.Time;

        // Check for ground collision
        if (playerBoxMin.Y <= 0f)
        {
            playerBoxMin.Y = 0f;
            playerBoxMax.Y = TextureSizes.PlayerSizeY;
            _velocityY = 0f;
            _state.PlayerInAir = false;
        }

        _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);
    }

    private void ReadJumpKeyAndMoveCamera()
    {
        Vector2 playerBoxMin = _state.PlayerBox.Min;

        // Update jump key held status and start jump if needed
        if (_playerKeys.PressedKeys.Contains(Keys.W))
        {
            if (!_state.PlayerInAir)
            {
                _state.PlayerInAir = true;
                _velocityY = JumpVelocity;
                _state.PlayerState = PlayerState.jump_r;
            }

            _isJumpKeyHeld = true;
        }
        else
        {
            _isJumpKeyHeld = false;
        }

        // move camera
        Vector2 cameraCenter = _camera.Center;

        // prevent the camera from moving outside of background

        // calculate camera X to center player
        cameraCenter.X = -playerBoxMin.X + _camera.ScreenWidth / 2 - TextureSizes.PlayerSizeX / 2;

        // clamp camera to not show black background
        float minCameraX = 0f;
        float maxCameraX = TextureManager.BackgroundRectangle.Max.X - _camera.ScreenWidth;
        float screenLeft = -cameraCenter.X;

        if (screenLeft < minCameraX)
        {
            // snap to background start
            cameraCenter.X = -minCameraX;
        }
        else if (screenLeft > maxCameraX)
        {
            // snap to background end
            cameraCenter.X = -maxCameraX;
        }

        _camera.Center = cameraCenter;
    }
}