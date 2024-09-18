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
    private readonly KeyGroup _playerKeys;
    private readonly Keyboard _keyboard;

    public WorldHandler(Camera camera, GameState state, KeyGroup playerKeys, Keyboard keyboard)
    {
        _camera = camera;
        _state = state;
        _playerKeys = playerKeys;
        _keyboard = keyboard;

        // setup initial camera parameters
        // TODO, this seems like the wrong place for this
        _camera.Scale = 4.0f;

        // TODO, actual enemy Logic
        _state.enemies.Add(new Enemy { Box = new Box2(15f, 0f, 20f, 5f) });
        _state.enemies.First().OnDeath += EnemyDeath;
    }

    private void EnemyDeath(object? sender, EventArgs e)
    {
        if (sender is not Enemy enemy)
        {
            throw new InvalidCastException("Enemy Death sender is not Enemy. Something is very wrong");
        }

        _state.enemies.Remove(enemy);
        Console.WriteLine("enemy death");
    }

    public void Update(FrameEventArgs frameArgs)
    {
    }
}