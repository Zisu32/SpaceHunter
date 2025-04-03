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
    private readonly Random _random;

    public WorldHandler(Camera camera, GameState state, KeyGroup playerKeys, Keyboard keyboard)
    {
        _camera = camera;
        _state = state;
        _playerKeys = playerKeys;
        _keyboard = keyboard;
        _random = new Random();

        // TODO, actual enemy Logic
        Vector2 enemyPosition = new Vector2(5, 0);
        _state.Enemies.Add(new Enemy(2, 0.5)
        {
            Box = new Box2(enemyPosition.X, enemyPosition.Y, enemyPosition.X + TextureSizes.Enemy1SizeX,
                enemyPosition.Y + TextureSizes.Enemy1SizeY),
            TargetBox = new Box2(enemyPosition.X, enemyPosition.Y, enemyPosition.X + TextureSizes.Enemy1SizeX,
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
        foreach (Enemy enemyState in _state.Enemies)
        {
            if (enemyState.IdleMoving)
            {
                // this only moves the box on the X Axis
                Box2 enemyBox = enemyState.Box;
                Vector2 enemyBoxCenter = enemyBox.Center;
                float differenceX = enemyState.TargetBox.Center.X - enemyBoxCenter.X;

                // TODO this does not move linearly
                enemyBoxCenter.X += differenceX * 0.01f;
                enemyBox.Center = enemyBoxCenter;
                enemyState.Box = enemyBox;


                if (Vector2.Distance(enemyBoxCenter, enemyState.TargetBox.Center) <= 0.1f)
                {
                    enemyState.IdleMoving = false;
                    enemyState.LastIdleMovement = 0;
                    enemyState.CurrentIdleMovementRandom = _random.NextDouble(); // is between 0 and 1
                }
                
                return;
            }

            enemyState.LastIdleMovement += frameArgs.Time;

            if (enemyState.LastIdleMovement >
                enemyState.IdleMovementDelay + enemyState.CurrentIdleMovementRandom)
            {
                enemyState.IdleMoving = true;

                Box2 newTarget = new Box2(enemyState.Box.Min, enemyState.Box.Max);
                Vector2 newTargetCenter = newTarget.Center;
                newTargetCenter.X += 5;

                newTarget.Center = newTargetCenter;
                enemyState.TargetBox = newTarget;
            }
        }
    }
}