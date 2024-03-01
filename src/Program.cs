using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

internal static class Program
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    private static Camera _camera;
    private static BufferedKeyGroup _translationKeys;
    private static BufferedKeyGroup _rotationKeys;
    private static BufferedKeyGroup _scaleKeys;
    private static BufferedKeyGroup _playerKeys;
    private static GameState _state;
    private static WorldHandler _worldHandler;
    private static OpenTKManager _manager;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public static void Main(string[] args)
    {
        _state = new GameState();

        _manager = new OpenTKManager(new DrawComponent(_state));

        _translationKeys = new BufferedKeyGroup(new List<Keys>
        {
            Keys.A, Keys.D, Keys.W, Keys.S
        });

        _rotationKeys = new BufferedKeyGroup(new List<Keys>
        {
            Keys.Q, Keys.E
        });

        _scaleKeys = new BufferedKeyGroup(new List<Keys>
        {
            Keys.Z, Keys.X
        });

        _playerKeys = new BufferedKeyGroup(new List<Keys>
        {
            Keys.Up, Keys.Right, Keys.Down, Keys.Left, Keys.Space, Keys.F
        });

        _camera = _manager.Camera;

        _worldHandler = new WorldHandler(_camera, _state, _playerKeys, _manager.Keyboard);

        _manager.Keyboard.AddKeyGroup(_translationKeys);
        _manager.Keyboard.AddKeyGroup(_rotationKeys);
        _manager.Keyboard.AddKeyGroup(_scaleKeys);
        _manager.Keyboard.AddKeyGroup(_playerKeys);
        _manager.GameStateUpdateEvent += GameUpdate;

        _manager.DisplayWindow();
    }

    private static void GameUpdate(object? sender, FrameEventArgs frameArgs)
    {
        _worldHandler.Update(frameArgs);
        Translation();
        // Rotation();
        Scale();
        // PlayerMove();
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

        Console.WriteLine($"Scale = {_camera.Scale}");
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

        Console.WriteLine($"Rotation = {_camera.Rotation}");
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

        Console.WriteLine($"Center = {_camera.Center}");
        _translationKeys.LastPressed = null;
        _camera.Center = cameraCenter;
    }
}