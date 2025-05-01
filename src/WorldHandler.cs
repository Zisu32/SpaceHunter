using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

/// <summary>
/// Handles position and movement of player and enemies.
/// </summary>
public class WorldHandler
{
    private readonly Camera _camera;
    private readonly GameState _state;
    private readonly KeyGroup _playerKeys;
    private readonly Keyboard _keyboard;
    private readonly Random _random;
    private readonly TextureManager _textureManager;

    public WorldHandler(Camera camera, GameState state, KeyGroup playerKeys, Keyboard keyboard,
        TextureManager textureManager)
    {
        _camera = camera;
        _state = state;
        _playerKeys = playerKeys;
        _keyboard = keyboard;
        _random = new Random();
        _textureManager = textureManager;
    }

    public void SpawnInitialEnemiesAndHearts()
    {
        // Normal enemy
        Vector2 enemyPosition = new Vector2(5, 0);
        var enemy = new Enemy(2, 0.5)
        {
            Box = new Box2(enemyPosition.X, enemyPosition.Y,
                enemyPosition.X + TextureSizes.Enemy1SizeX,
                enemyPosition.Y + TextureSizes.Enemy1SizeY),

            TargetBox = new Box2(enemyPosition.X, enemyPosition.Y,
                enemyPosition.X + TextureSizes.Enemy1SizeX,
                enemyPosition.Y + TextureSizes.Enemy1SizeY)
        };
        enemy.OnDeath += EnemyDeath;
        _state.Enemies.Add(enemy);
        
        // Flying enemy
        var flying = new FlyingEnemy(TextureManager.FlyingEnemyRectangle, _textureManager._flyingEnemy);
        flying.OnDeath += FlyingEnemyDeath;
        _state.FlyingEnemies.Add(flying);

        // heart
        _state.Hearts.Add(new Heart(new Vector2(15f, 2f)));
    }

    private void EnemyDeath(object? sender, EventArgs e)
    {
        if (sender is not Enemy enemy)
            throw new InvalidCastException("Enemy Death sender is not Enemy.");

        _state.Enemies.Remove(enemy);
        Console.WriteLine("Enemy died!");
    }

    private void FlyingEnemyDeath(object? sender, EventArgs e)
    {
        if (sender is not FlyingEnemy flyingEnemy)
            throw new InvalidCastException("Flying Enemy Death sender is not FlyingEnemy.");

        _state.FlyingEnemies.Remove(flyingEnemy);
        Console.WriteLine("Flying enemy died!");
    }

    public void Update(FrameEventArgs frameArgs)
    {
        foreach (Enemy enemyState in _state.Enemies.ToList())
        {
            if (enemyState.IdleMoving)
            {
                Box2 enemyBox = enemyState.Box;
                Vector2 enemyBoxCenter = enemyBox.Center;
                float differenceX = enemyState.TargetBox.Center.X - enemyBoxCenter.X;

                enemyBoxCenter.X += differenceX * 0.01f;
                enemyBox.Center = enemyBoxCenter;
                enemyState.Box = enemyBox;

                if (Vector2.Distance(enemyBoxCenter, enemyState.TargetBox.Center) <= 0.1f)
                {
                    enemyState.IdleMoving = false;
                    enemyState.LastIdleMovement = 0;
                    enemyState.CurrentIdleMovementRandom = _random.NextDouble();
                }

                continue;
            }

            enemyState.LastIdleMovement += frameArgs.Time;

            if (enemyState.LastIdleMovement > enemyState.IdleMovementDelay + enemyState.CurrentIdleMovementRandom)
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