﻿using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

internal static class Program
{
    private static CollisionHandler _collisionHandler;
    private static TextureManager _textureManager;
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
        _textureManager = new TextureManager();

        // Setup Keys
        _translationKeys = new KeyGroup(new List<Keys> { Keys.A, Keys.D, Keys.W, Keys.S });
        _rotationKeys = new KeyGroup(new List<Keys> { Keys.Q, Keys.E });
        _scaleKeys = new KeyGroup(new List<Keys> { Keys.Z, Keys.X });
        _playerKeys = new KeyGroup(new List<Keys> { Keys.A, Keys.D, Keys.W, Keys.S, Keys.F });
        _startKey = new KeyGroup(new List<Keys> { Keys.Enter });

        var drawComponent = new DrawComponent(_state, _textureManager);

        _manager = new OpenTKManager(drawComponent);

        _camera = _manager.Camera;
        _camera.Scale = 6f;

        Console.WriteLine($"Camera Scale: {_camera.Scale}");
        Console.WriteLine($"Camera Center: {_camera.Center}");
        Console.WriteLine($"Camera Width: {_camera.ScreenWidth}");

        _collisionHandler = new CollisionHandler(_state);
        _manager.Keyboard.AddKeyGroup(_startKey);
        _worldHandler = new WorldHandler(_camera, _state, _playerKeys, _manager.Keyboard, _textureManager, _collisionHandler);
        _playerMovementHandler = new PlayerMovement(_state, _playerKeys, _manager.Keyboard, _camera);
        _state.WorldHandler = _worldHandler;
        
        _manager.Keyboard.AddKeyGroup(_translationKeys);
        _manager.Keyboard.AddKeyGroup(_rotationKeys);
        _manager.Keyboard.AddKeyGroup(_scaleKeys);
        _manager.Keyboard.AddKeyGroup(_playerKeys);
        _textureManager.Initialize();
        _worldHandler.SpawnInitial();
        _manager.GameStateUpdateEvent += GameUpdate;
        _manager.DisplayWindow();
    }

    private static void GameUpdate(object? sender, FrameEventArgs frameArgs)
    {
        // Restart game from GameOver screen
        if (_state.IsGameOver && _startKey.PressedKeys.Contains(Keys.Enter))
        {
            _state.ResetGame();
            return;
        }
        
        if (_state.IsShowingLevelTransition)
        {
            _state.LevelTransitionTimer -= frameArgs.Time;
            if (_state.LevelTransitionTimer <= 0)
            {
                _state.IsShowingLevelTransition = false;
                _state.NextLevel();
            }
            return;
        }

        if (!_state.IsGameStarted && _startKey.PressedKeys.Contains(Keys.Enter))
        {
            _state.IsGameStarted = true;
        }

        if (!_state.IsGameStarted)
        {
            return;
        }
        
        _worldHandler.Update(frameArgs);
        _collisionHandler.UpdateCooldown(frameArgs);

        // Player alive
        if (_state.PlayerAlive)
        {
            _playerMovementHandler.Update(frameArgs);
        }
        else
        {
            Console.WriteLine("dead");
            _state.PlayerState = PlayerState.death;
            if (_textureManager.DeathAnimationFinished)
            {
                _camera.Center = Vector2.Zero;
                _state.IsGameOver = true;
                _state.IsGameStarted = false;
            }
        }

        // Player hurt
        if (_state.IsPlayerHurt)
        {
            _state.PlayerHurtTimer -= frameArgs.Time;
            if (_state.PlayerHurtTimer <= 0)
            {
                _state.IsPlayerHurt = false;
                _state.PlayerHurtTimer = 0;
            }
        }
        
        // Player Victory
        if (_state.IsGameWon)
        {
            Console.WriteLine("Game won!"); 
            _camera.Center = Vector2.Zero;
            if (_startKey.PressedKeys.Contains(Keys.Enter))
            {
                _state.IsGameStarted = false;
                _state.ResetGame();
            }
        }

        // Heart collection
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
    }
}
