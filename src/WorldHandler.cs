using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

public class WorldHandler
{
    private readonly Camera _camera;
    private readonly GameState _state;
    private readonly KeyGroup _playerKeys;
    private readonly Keyboard _keyboard;
    private readonly Random _random;
    private readonly TextureManager _textureManager;
    private readonly CollisionHandler _collisionHandler;

    public WorldHandler(Camera camera, GameState state, KeyGroup playerKeys, Keyboard keyboard,
        TextureManager textureManager, CollisionHandler collisionHandler)
    {
        _camera = camera;
        _state = state;
        _playerKeys = playerKeys;
        _keyboard = keyboard;
        _random = new Random();
        _textureManager = textureManager;
        _collisionHandler = collisionHandler;
    }

    public void SpawnInitial()
    {
        SpawnGroundEnemies(3);
        SpawnFlyingEnemies(4);
        SpawnHearts(1);
        SpawnPortal();
    }

    private void SpawnGroundEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = GetRandomGroundEnemyStartPosition();
            var enemy = new Enemy(2, 0.5)
            {
                Box = new Box2(
                    pos.X,
                    pos.Y,
                    pos.X + TextureManager.StaticEnemyRectangle.Size.X,
                    pos.Y + TextureManager.StaticEnemyRectangle.Size.Y),

                TargetBox = new Box2(
                    pos.X,
                    pos.Y,
                    pos.X + TextureManager.StaticEnemyRectangle.Size.X,
                    pos.Y + TextureManager.StaticEnemyRectangle.Size.Y)
            };
            enemy.OnDeath += EnemyDeath;
            _state.Enemies.Add(enemy);
        }
    }

    private void SpawnFlyingEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomPos = GetRandomFlyingEnemyStartPosition();
            Box2 startBox = new Box2(
                randomPos.X,
                randomPos.Y,
                randomPos.X + TextureManager.FlyingEnemyRectangle.Size.X,
                randomPos.Y + TextureManager.FlyingEnemyRectangle.Size.Y
            );

            var flying = new FlyingEnemy(startBox, _textureManager._flyingEnemy);
            flying.OnDeath += FlyingEnemyDeath;
            _state.FlyingEnemies.Add(flying);
        }
    }

    private void SpawnHearts(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomPos = GetRandomHeartStartPosition();
            Console.WriteLine($"Spawning heart at {randomPos}");
            _state.Hearts.Add(new Heart(randomPos, _textureManager._heart));
        }
    }

    private void SpawnPortal()
    {
        _state.Portal = new Portal(
            _state,
            TextureManager.PortalRectangle,
            _textureManager._portalTexture
        );
    }

    private Vector2 GetRandomFlyingEnemyStartPosition()
    {
        float x = (float)(_random.NextDouble() * 45f) + 5f;
        float y = 1f;
        return new Vector2(x, y);
    }

    private Vector2 GetRandomGroundEnemyStartPosition()
    {
        float x = (float)(_random.NextDouble() * 45f) + 5f;
        float y = 0.7f;
        return new Vector2(x, y);
    }

    private Vector2 GetRandomHeartStartPosition()
    {
        float x = (float)(_random.NextDouble() * 45f) + 5f;
        float y = 2f;
        return new Vector2(x, y);
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
        _state.Enemies.ForEach(enemy => enemy.Update((float)frameArgs.Time));
        _state.FlyingEnemies.ForEach(enemy => enemy.Update((float)frameArgs.Time, _state.PlayerBox));
        _state.Hearts.ForEach(enemy => enemy.Update((float)frameArgs.Time));
        _state.Portal?.Update((float)frameArgs.Time, _state.Enemies, _state.FlyingEnemies, _state.PlayerBox);

        _collisionHandler.CheckAllEnemyCollisions();
    }
}