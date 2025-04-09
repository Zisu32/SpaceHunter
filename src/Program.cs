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

    public static void Main(string[] args)
    {
        _state = new GameState();

        // TODO, framemrate should probably be locked
        // time between frames is not used anywhere
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
        Console.WriteLine($"Player Health: {_state.PlayerHealth}");
        _worldHandler.Update(frameArgs);
        _collisionHandler.Update(frameArgs);
        // player alive
        if (_state.PlayerAlive)
        {
            _playerMovementHandler.Update(frameArgs);
        }

        // player death
        if (!_state.PlayerAlive)
        {
            _state.PlayerState = PlayerState.death;
            Console.WriteLine("dead");
        }
        
        // player hurt
        if (_state.IsPlayerHurt)
        {
            _state.PlayerHurtTimer -= frameArgs.Time;
            if (_state.PlayerHurtTimer <= 0)
            {
                _state.IsPlayerHurt = false;
                _state.PlayerHurtTimer = 0;
            }
        }
        
        // Heart
        foreach (Heart heart in _state.Hearts)
        {
            if (!heart.IsCollected &&
                heart.Box.Min.X < _state.PlayerBox.Max.X &&
                heart.Box.Max.X > _state.PlayerBox.Min.X &&
                heart.Box.Min.Y < _state.PlayerBox.Max.Y &&
                heart.Box.Max.Y > _state.PlayerBox.Min.Y)
            {
                heart.IsCollected = true;

                _state.PlayerHealth += 10;
                Console.WriteLine("+10 Health");
                if (_state.PlayerHealth > ConstantBalancingValues.MaxPlayerHealth)
                {
                    _state.PlayerHealth = ConstantBalancingValues.MaxPlayerHealth;
                }
            }
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