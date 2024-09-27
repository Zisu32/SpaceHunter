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

        // TODO, actual enemy Logic
        Vector2 enemyPosition = new Vector2(15f, 0f);
        _state.Enemies.Add(new Enemy
        {
            Box = new Box2(enemyPosition.X, enemyPosition.Y, enemyPosition.X + TextureSizes.Enemy1SizeX,
                enemyPosition.Y + TextureSizes.Enemy1SizeY)
        });
        _state.Enemies.First().OnDeath += EnemyDeath;
    }

    private void EnemyDeath(object? sender, EventArgs e)
    {
        if (sender is not Enemy enemy)
        {
            throw new InvalidCastException("Enemy Death sender is not Enemy. Something is very wrong");
        }

        _state.Enemies.Remove(enemy);
        Console.WriteLine("enemy death");
    }

    public void Update(FrameEventArgs frameArgs)
    {
    }
}