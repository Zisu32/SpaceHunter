using OpenTK.Mathematics;
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

    public WorldHandler(Camera camera, GameState state, BufferedKeyGroup playerKeys)
    {
        _camera = camera;
        _state = state;
        _playerKeys = playerKeys;

        // setup initial camera parameters
        _camera.Scale = 4.0f;
    }

    public void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;

        switch (_playerKeys.LastPressed)
        {
            // case Keys.Up:
            //     playerBoxMin.Y += 0.1f;
            //     playerBoxMax.Y += 0.1f;
            //     break;
            // case Keys.Down:
            //     playerBoxMin.Y -= 0.1f;
            //     playerBoxMax.Y -= 0.1f;
            //     break;
            case Keys.Left:

                // TODO: get floated, this somehow allows player to move camera out of bounds
                if (playerBoxMin.X < 0)
                {
                    return;
                }

                playerBoxMin.X -= 0.2f;
                playerBoxMax.X -= 0.2f;
                break;
            case Keys.Right:

                // TODO: where do the 5f come from?
                // 5f is size of player Box
                if (playerBoxMax.X >= TextureManager.BackgroundRectangle.Max.X + 5f / 2)
                {
                    return;
                }

                playerBoxMin.X += 0.2f;
                playerBoxMax.X += 0.2f;
                break;

            default:
                return;
        }

        Console.WriteLine($"PlayerPosMin: {playerBoxMin}");
        Console.WriteLine($"PlayerPosMax: {playerBoxMax}");

        _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);
        _playerKeys.LastPressed = null;


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