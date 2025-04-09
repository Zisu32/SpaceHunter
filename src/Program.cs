using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

internal static class Program
{
    private static CollisionHandler _collisionHandler;
    private static Camera _camera = null!;
    private static KeyGroup _translationKeys = null!;
    private static KeyGroup _rotationKeys = null!;
    private static KeyGroup _scaleKeys = null!;
    private static KeyGroup _playerKeys = null!;
    private static GameState _state = null!;
    private static WorldHandler _worldHandler = null!;
    private static PlayerMovement _playerMovementHandler = null!;
    private static OpenTKManager _manager = null!;
    private static KeyGroup _startKey = null!;


    public static void Main(string[] args)
    {
        _state = new GameState();

        _manager = new OpenTKManager(new DrawComponent(_state));

        _translationKeys = new KeyGroup(new List<Keys>
        {
            Keys.A, Keys.D, Keys.W, Keys.S
        });

        _rotationKeys = new KeyGroup(new List<Keys>
        {
            Keys.Q, Keys.E
        });

        _scaleKeys = new KeyGroup(new List<Keys>
        {
            Keys.Z, Keys.X
        });

        _playerKeys = new KeyGroup(new List<Keys>
        {
            Keys.Up, Keys.Right, Keys.Down, Keys.Left, Keys.Space, Keys.F
        });

        _startKey = new KeyGroup(new List<Keys> { Keys.Enter });
        _manager.Keyboard.AddKeyGroup(_startKey);


        #region Setup camera

        _camera = _manager.Camera;
        _camera.Scale = 6f;

        Console.WriteLine($"Camera Scale: {_camera.Scale}");
        Console.WriteLine($"Camera Center: {_camera.Center}");
        Console.WriteLine($"Camera Width: {_camera.ScreenWidth}");

        #endregion


        _worldHandler = new WorldHandler(_camera, _state, _playerKeys, _manager.Keyboard);
        _playerMovementHandler = new PlayerMovement(_state, _playerKeys, _manager.Keyboard, _camera);
        _collisionHandler = new CollisionHandler(_state);

        _manager.Keyboard.AddKeyGroup(_translationKeys);
        _manager.Keyboard.AddKeyGroup(_rotationKeys);
        _manager.Keyboard.AddKeyGroup(_scaleKeys);
        _manager.Keyboard.AddKeyGroup(_playerKeys);
        _manager.GameStateUpdateEvent += GameUpdate;

        _manager.DisplayWindow();
    }

    private static void GameUpdate(object? sender, FrameEventArgs frameArgs)
    {
        if (!_state.IsGameStarted && _startKey.PressedKeys.Contains(Keys.Enter))
        {
            _state.IsGameStarted = true;
            return;
        }


        _worldHandler.Update(frameArgs);
        _collisionHandler.Update(frameArgs);

        if (_state.PlayerAlive)
        {
            _playerMovementHandler.Update(frameArgs);
        }

        // player actions
        if (!_state.PlayerAlive)
        {
            _state.PlayerState = PlayerState.death;
            Console.WriteLine("ded");
        }

        #region Camera debug

        // Translation();
        // Rotation();
        Scale();
        // PlayerMove();

        #endregion
    }

    private static void Scale()
    {
        if (_scaleKeys.PressedKeys.Contains(Keys.Z))
        {
            _camera.Scale += .1f;
        }

        if (_scaleKeys.PressedKeys.Contains(Keys.X))
        {
            _camera.Scale -= .1f;
        }

        Console.WriteLine($"Scale = {_camera.Scale}");
    }

    private static void Rotation()
    {
        if (_rotationKeys.PressedKeys.Contains(Keys.Q))
        {
            _camera.Rotation += 2;
        }

        if (_rotationKeys.PressedKeys.Contains(Keys.E))
        {
            _camera.Rotation -= 2;
        }

        Console.WriteLine($"Rotation = {_camera.Rotation}");
    }

    private static void Translation()
    {
        Vector2 cameraCenter = _camera.Center;


        if (_translationKeys.PressedKeys.Contains(Keys.A))
        {
            cameraCenter.X -= 0.1f;
        }

        if (_translationKeys.PressedKeys.Contains(Keys.D))
        {
            cameraCenter.X += 0.1f;
        }

        if (_translationKeys.PressedKeys.Contains(Keys.W))
        {
            cameraCenter.Y += 0.1f;
        }

        if (_translationKeys.PressedKeys.Contains(Keys.S))
        {
            cameraCenter.Y -= 0.1f;
        }


        Console.WriteLine($"Center = {_camera.Center}");
        _camera.Center = cameraCenter;
    }
}