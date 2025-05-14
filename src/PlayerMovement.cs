using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
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

    // how many players heights the jump is high
    // TODO, move the consts in seperate classes
    // TODO extract player box size (5f) to const somewhere, or auto generate
    private const float JumpHeight = 3.5f;
    private const float JumpDuration = 0.8f;
    private float _jumpTime;

    private float _playerSpeed;
    private const float PlayerSpeedAdd = 0.005f;
    private static float _playerSpeedDiv = 1.1f;
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
        #region Movement

        // movement stopped while attacking
        if (_attackTime <= 0)
        {
            ProcessKeys();

            if (_state.PlayerInAir)
            {
                JumpMovement(frameArgs);
            }
        }

        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;

        if (
            MathF.Abs(_playerSpeed) < 0.005f // playerSpeed small
            || playerBoxMin.X + _playerSpeed < 0f // out of bound 0
            || playerBoxMax.X + _playerSpeed >=
            TextureManager.BackgroundRectangle.Max.X + TextureSizes.PlayerSizeX / 2) // out of bounds max
        {
            _playerSpeed = 0;
        }

        if (_playerSpeed != 0)
        {
            // moves player
            playerBoxMin.X += _playerSpeed;
            playerBoxMax.X += _playerSpeed;
            Console.WriteLine($"Player Speed: {_playerSpeed:N4} MinBox:{playerBoxMin.X}");
        }

        _playerSpeed /= _playerSpeedDiv;

        _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);

        #endregion

        // attac

        if (_playerKeys.PressedKeys.Contains(Keys.F) && _attackTime <= 0 && !_state.PlayerInAir)
        {
            _attackTime = ConstantBalancingValues.AttackDuration;
            _playerKeys.RemovePressed(Keys.F); // F has to be pressed multiple times, for multiple attacks

            // stop movement
            _playerSpeed = 0;
        }

        if (_attackTime > 0)
        {
            Vector2 hitBoxMin = new Vector2(_state.PlayerBox.Max.X, _state.PlayerBox.Min.Y);
            // TODO extract HitBox Size ( + 2) to const 

            // TODO, implement attack rotation
            Vector2 hitBoxMax = _playerDirection switch
            {
                SimpleDirection.RIGHT => new Vector2(_state.PlayerBox.Max.X + 2, _state.PlayerBox.Max.Y),
                // SimpleDirection.LEFT => new Vector2(_state.PlayerBox.Max.X - 2, _state.PlayerBox.Max.Y),
                SimpleDirection.LEFT => new Vector2(_state.PlayerBox.Max.X + 2, _state.PlayerBox.Max.Y),
                _ => throw new ArgumentOutOfRangeException()
            };

            _state.PlayerHitBox = new Box2(hitBoxMin, hitBoxMax);

            _attackTime -= frameArgs.Time;
        }
        else // _attackTime <= 0
        {
            _state.PlayerHitBox = null;
        }

        #region Animation

        if (_attackTime > 0)
        {
            _state.PlayerState = _playerDirection == SimpleDirection.LEFT ? PlayerState.attack_l : PlayerState.attack_r;
            return;
        }

        if (playerBoxMin.Y < 0.0001f)
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

    // TODO, camera does not move up/down
    // TODO, switch to upwards speed approach, like left/right speed
    private void JumpMovement(FrameEventArgs frameArgs)
    {
        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;

        // stop jump, first wait the predefined time. Then wait for play to return to ground
        if (_jumpTime > JumpDuration - 0.02f && playerBoxMin.Y < 0.0001f)
        {
            _state.PlayerInAir = false;
            playerBoxMin.Y = 0;
            playerBoxMax.Y = TextureSizes.PlayerSizeY;
            _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);
            _state.PlayerState = PlayerState.idle_r;
            return;
        }

        float jumpDistance = (float)(TextureSizes.PlayerSizeY * JumpHeight * (frameArgs.Time / JumpDuration));

        if (_jumpTime > JumpDuration / 2)
        {
            // move down
            playerBoxMax.Y -= jumpDistance;
            playerBoxMin.Y -= jumpDistance;
        }
        else
        {
            // move up
            playerBoxMax.Y += jumpDistance;
            playerBoxMin.Y += jumpDistance;
        }

        _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);
        _jumpTime += (float)frameArgs.Time;
    }

    private void ProcessKeys()
    {
        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;

        if (_playerKeys.PressedKeys.Contains(Keys.Left))
        {
            if (playerBoxMin.X < 0)
            {
                return;
            }

            _playerSpeed -= PlayerSpeedAdd;
            _playerDirection = SimpleDirection.LEFT;
        }
        else if (_playerKeys.PressedKeys.Contains(Keys.Right))
        {
            // 5f is size of player Box
            if (playerBoxMax.X >= TextureManager.BackgroundRectangle.Max.X + TextureSizes.PlayerSizeX / 2)
            {
                return;
            }

            _playerSpeed += PlayerSpeedAdd;
            _playerDirection = SimpleDirection.RIGHT;
        }

        if (_playerKeys.PressedKeys.Contains(Keys.Space))
        {
            // TODO, this should only be set once to prevent somehow becoming invincible
            if (!_state.PlayerInAir)
            {
                _state.PlayerInAir = true;
                _jumpTime = 0;
                _state.PlayerState = PlayerState.jump_r;
            }

            _playerKeys.RemovePressed(Keys.Space);
        }

        // TODO change Jump to velocity based system

        _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);

        // move camera
        Vector2 cameraCenter = _camera.Center;

        // prevent the camera from moving outside of background
        if (playerBoxMin.X + _camera.ScreenWidth < TextureManager.BackgroundRectangle.Max.X)
        {
            cameraCenter.X = -playerBoxMin.X;
            _camera.Center = cameraCenter;
        }
    }
}