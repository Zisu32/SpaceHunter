using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

public class PlayerMovement
{
    private readonly Camera _camera;
    private readonly GameState _state;
    private readonly BufferedKeyGroup _playerKeys;
    private readonly Keyboard _keyboard;

    // how many players heights the jump is high
    // TODO, move the consts in seperate classes
    // TODO extract player box size (5f) to const somewhere, or auto generate
    private const float JumpHeight = 2.5f;
    private const float JumpDuration = 0.5f;
    private float _jumpTime;

    private float _playerSpeed = 0;
    private const float PlayerSpeedAdd = 0.01f;
    private static float PlayerSpeedDiv = 1.1f;

    public PlayerMovement(GameState state, BufferedKeyGroup playerKeys, Keyboard keyboard, Camera camera)
    {
        _state = state;
        _playerKeys = playerKeys;
        _keyboard = keyboard;
        _camera = camera;
    }

    public void Update(FrameEventArgs frameArgs)
    {
        MovePlayer();
        if (_state.PlayerInAir)
        {
            JumpMovement(frameArgs);
            // TODO, no
            PlayerSpeedDiv = 1.0001f;
        }
        else
        {
            // TODO, stop SpeedDiv should be constant
            PlayerSpeedDiv = 1.1f;
        }

        // TODO, remove or move
        if (CollisionHandler.TwoBoxCollisionCheck(_state.PlayerBox, _state.enemyBoxes[0]))
        {
            // Console.WriteLine("player enemy collision");
        }

        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;

        if (MathF.Abs(_playerSpeed) < 0.00001f
            || playerBoxMin.X + _playerSpeed < 0f
            || playerBoxMax.X + _playerSpeed >= TextureManager.BackgroundRectangle.Max.X + 5f / 2)
        {
            _playerSpeed = 0;
        }


        if (_playerSpeed != 0)
        {
            playerBoxMin.X += _playerSpeed;
            playerBoxMax.X += _playerSpeed;
            Console.WriteLine($"Player Speed: {_playerSpeed:N4} MinBox:{playerBoxMin.X}");
        }

        _playerSpeed /= PlayerSpeedDiv;

        _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);
    }

    // TODO, switch to upwards speed approach, like left/right speed
    // TODO, why does this not move left/right? --> space stops instantly reduces speed to zero
    // maybe decrease move speed less in air? actual fix: no bufferedKeyGroups
    private void JumpMovement(FrameEventArgs frameArgs)
    {
        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;

        // stop jump, first wait the predefined time. Then wait for play to return to ground
        if (_jumpTime > JumpDuration - 0.02f && playerBoxMin.Y < 0.0001f)
        {
            _state.PlayerInAir = false;
            playerBoxMin.Y = 0;
            playerBoxMax.Y = 5F;
            _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);
            _state.playerState = PlayerState.idle_r;
            return;
        }

        float jumpDistance = (float)(5f * JumpHeight * (frameArgs.Time / JumpDuration));

        if (_jumpTime > JumpDuration / 2)
        {
            // move down
            playerBoxMax.Y -= jumpDistance;
            playerBoxMin.Y -= (jumpDistance);
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

    private void MovePlayer()
    {
        if (_playerKeys.LastPressed != null)
        {
            // Console.WriteLine($"PlayerKey: {_playerKeys.LastPressed}");
        }

        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;


        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (_playerKeys.LastPressed)
        {
            case Keys.Left:
                if (playerBoxMin.X < 0)
                {
                    return;
                }

                _playerSpeed -= PlayerSpeedAdd;
                _state.playerState = PlayerState.run_l;
                break;
            case Keys.Right:
                // 5f is size of player Box
                if (playerBoxMax.X >= TextureManager.BackgroundRectangle.Max.X + 5f / 2)
                {
                    return;
                }

                _playerSpeed += PlayerSpeedAdd;
                _state.playerState = PlayerState.run_r;
                break;
            case Keys.Space:
                // TODO, this should only be set once to prevent somehow becoming invincible
                if (_state.PlayerInAir)
                {
                    break;
                }

                _state.PlayerInAir = true;
                _jumpTime = 0;
                _state.playerState = PlayerState.jump_r;
                break;

            case Keys.F:
                _state.playerState = PlayerState.attack_r;
                break;

            default:
                if (!_state.PlayerInAir)
                {
                    if (_playerKeys.LastPressed == Keys.Left)
                    {
                        _state.playerState = PlayerState.idle_l;
                    }
                    else
                    {
                        _state.playerState = PlayerState.idle_r;
                    }
                }

                break;
        }

        _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);

        // TODO, this does not help, buffered Key Groups need to be replaced
        Keys? lastKey = _playerKeys.LastPressed;
        if (lastKey != null)
        {
            if (!_keyboard.CheckKeyDown((Keys)lastKey))
            {
                _playerKeys.LastPressed = null;
            }
        }


        // move camera
        Vector2 cameraCenter = _camera.Center;

        // prevent the camera from moving outside of background
        if (playerBoxMin.X + _camera.ScreenWidth < TextureManager.BackgroundRectangle.Max.X)
        {
            cameraCenter.X = -playerBoxMin.X;
            _camera.Center = cameraCenter;
        }

        // Console.WriteLine($"Camera: {cameraCenter.X}");
        // Console.WriteLine("");
    }
}