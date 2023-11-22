using CameraTest.Models;
using OpenTK.Core.Exceptions;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKLib;

namespace CameraTest;

internal static class Program
{
    private static Camera _camera;
    private static BufferedKeyGroup _translationKeys;
    private static BufferedKeyGroup _rotationKeys;
    private static BufferedKeyGroup _scaleKeys;
    private static BufferedKeyGroup _playerKeys;
    private static GameState _state;

    public static void Main(string[] args)
    {
        _state = new GameState();


        OpenTKManager manager = new OpenTKManager(new DrawComponent(_state));

        _translationKeys = new BufferedKeyGroup(new List<Keys>
        {
            Keys.A, Keys.D, Keys.W, Keys.S
        });

        _rotationKeys = new BufferedKeyGroup(new List<Keys>()
        {
            Keys.Q, Keys.E
        });

        _scaleKeys = new BufferedKeyGroup(new List<Keys>()
        {
            Keys.Z, Keys.X
        });

        _playerKeys = new BufferedKeyGroup(new List<Keys>()
        {
            Keys.Up, Keys.Right, Keys.Down, Keys.Left
        });

        _camera = manager.Camera;
        _camera.Center = new Vector2(-1, 0);
        manager.Keyboard.AddKeyGroup(_translationKeys);
        manager.Keyboard.AddKeyGroup(_rotationKeys);
        manager.Keyboard.AddKeyGroup(_scaleKeys);
        manager.Keyboard.AddKeyGroup(_playerKeys);
        manager.GameStateUpdateEvent += GameUpdate;

        manager.DisplayWindow();
    }

    private static void GameUpdate(object? sender, FrameEventArgs e)
    {
        Translation();
        Rotation();
        Scale();
        PlayerMove();
    }

    private static void PlayerMove()
    {
        Vector2 playerBoxMin = _state.PlayerBox.Min;
        Vector2 playerBoxMax = _state.PlayerBox.Max;

        switch (_playerKeys.LastPressed)
        {
            case Keys.Up:
                playerBoxMin.Y += 0.1f;
                playerBoxMax.Y += 0.1f;
                break;
            case Keys.Down:
                playerBoxMin.Y -= 0.1f;
                playerBoxMax.Y -= 0.1f;
                break;
            case Keys.Left:
                playerBoxMin.X -= 0.1f;
                playerBoxMax.X -= 0.1f;
                break;
            case Keys.Right:
                playerBoxMin.X += 0.1f;
                playerBoxMax.X += 0.1f;
                break;

            default:
                return;
        }

        _state.PlayerBox = new Box2(playerBoxMin, playerBoxMax);
        _playerKeys.LastPressed = null;
    }

    private static void Scale()
    {
        switch (_scaleKeys.LastPressed)
        {
            case Keys.Z:
                _camera.Scale += .1f;
                break;
            case Keys.X:
                _camera.Scale -= .1f;
                break;
            default:
                return;
        }

        _scaleKeys.LastPressed = null;
    }

    private static void Rotation()
    {
        switch (_rotationKeys.LastPressed)
        {
            case Keys.Q:
                _camera.Rotation += 2;
                break;
            case Keys.E:
                _camera.Rotation -= 2;
                break;
            default:
                return;
        }

        _rotationKeys.LastPressed = null;
    }

    private static void Translation()
    {
        Vector2 cameraCenter = _camera.Center;

        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (_translationKeys.LastPressed)
        {
            case Keys.A:
                cameraCenter.X -= 0.1f;
                break;
            case Keys.D:
                cameraCenter.X += 0.1f;
                break;
            case Keys.W:
                cameraCenter.Y += 0.1f;
                break;
            case Keys.S:
                cameraCenter.Y -= 0.1f;
                break;
            default:
                return;
        }

        _translationKeys.LastPressed = null;
        _camera.Center = cameraCenter;
    }
}