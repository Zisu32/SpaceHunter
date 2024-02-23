using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

/// <summary>
/// Handles position and movement of player an enemies
/// </summary>
public class WorldHandler
{
    private readonly Camera _camera;
    private readonly GameState _state;
    private readonly BufferedKeyGroup _playerKeys;
    private readonly Keyboard _keyboard;
    private const float JumpDuration = 0.5f;
    // how many players heights the jump is high
    private const float JumpHeight = 2.5f; 
    private float _jumpTime;

    public WorldHandler(Camera camera, GameState state, BufferedKeyGroup playerKeys, Keyboard keyboard)
    {
        _camera = camera;
        _state = state;
        _playerKeys = playerKeys;
        _keyboard = keyboard;

        // setup initial camera parameters
        _camera.Scale = 4.0f;
    }

    public void Update(FrameEventArgs frameArgs)
    {
        MovePlayer();
        if (_state.PlayerInAir)
        {
            JumpMovement(frameArgs);
        }
    }

    // TODO extract player box size (5f) to const somewhere

    private void JumpMovement(FrameEventArgs frameArgs)
    {
        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;

        // stop jump, first wait the predefined time. Then wait for play to return to ground
        if (_jumpTime > JumpDuration - 0.02f && playerBoxMin.Y < 0.0001f)
        {
            _state.PlayerInAir = false;
            playerBoxMin.Y = 0;
            _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);
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
            Console.WriteLine($"PlayerKey: {_playerKeys.LastPressed}");
        }

        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (_playerKeys.LastPressed)
        {
            case Keys.Left:

                // TODO: get floated, this somehow allows player to move camera out of bounds
                if (playerBoxMin.X < 0)
                {
                    return;
                }

                playerBoxMin.X -= 0.2f;
                playerBoxMax.X -= 0.2f;
                _state.playerState = PlayerState.run_l;
                break;
            case Keys.Right:

                // 5f is size of player Box
                if (playerBoxMax.X >= TextureManager.BackgroundRectangle.Max.X + 5f / 2)
                {
                    return;
                }

                playerBoxMin.X += 0.2f;
                playerBoxMax.X += 0.2f;
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
                break;

            default:
                if (_playerKeys.LastPressed == Keys.Left)
                {
                    _state.playerState = PlayerState.idle_l;
                }
                else
                {
                    _state.playerState = PlayerState.idle_r;
                }
                return;
        }

        Console.WriteLine($"PlayerPosMin: {playerBoxMin}");
        Console.WriteLine($"PlayerPosMax: {playerBoxMax}");

        _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);

        // TODO, don't stop movement instantly
        if (!_keyboard.CheckKeyDown(Keys.Left) || !_keyboard.CheckKeyDown(Keys.Right))
        {
            _playerKeys.LastPressed = null;
        }


        // move camera
        Vector2 cameraCenter = _camera.Center;

        // prevent the camera from moving outside of background
        if (playerBoxMin.X + _camera.ScreenWidth < TextureManager.BackgroundRectangle.Max.X)
        {
            cameraCenter.X = -playerBoxMin.X;
            _camera.Center = cameraCenter;
        }

        Console.WriteLine($"Camera: {cameraCenter.X}");
        Console.WriteLine("");
    }
}