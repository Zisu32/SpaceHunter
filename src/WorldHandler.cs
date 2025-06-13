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
    private readonly TextureManager _textureManager;
    private readonly CollisionHandler _collisionHandler;

    private readonly List<Vector2> _groundEnemyPositions = new()
    {
        new Vector2(15f, 0.7f),
        new Vector2(35f, 0.7f),
        new Vector2(50f, 0.7f),
        new Vector2(100f, 0.7f),
        new Vector2(120f, 0.7f),
        new Vector2(130f, 0.7f),
        new Vector2(160f, 0.7f),
        new Vector2(170f, 0.7f),
        new Vector2(195f, 0.7f),
        new Vector2(220f, 0.7f)
    };

    private readonly List<Vector2> _flyingEnemyPositions = new()
    {
        new Vector2(65f, 1.5f),
        new Vector2(80f, 1.5f),
        new Vector2(90f, 1.5f),
        new Vector2(115f, 1.5f),
        new Vector2(125f, 1.5f),
        new Vector2(150f, 1.5f),
        new Vector2(180f, 1.5f),
        new Vector2(190f, 1.5f),
        new Vector2(200f, 1.5f)
    };

    private readonly List<Vector2> _heartPositions = new()
    {
        new Vector2(40f, 2f),
        new Vector2(85f, 2f),
        new Vector2(150f, 2f),
        new Vector2(200f, 2f),
        new Vector2(230f, 2f),
    };

    public WorldHandler(Camera camera, GameState state, KeyGroup playerKeys, Keyboard keyboard,
        TextureManager textureManager, CollisionHandler collisionHandler)
    {
        _camera = camera;
        _state = state;
        _playerKeys = playerKeys;
        _keyboard = keyboard;
        _textureManager = textureManager;
        _collisionHandler = collisionHandler;
    }

    public void SpawnInitial()
    {
        SpawnGroundEnemies();
        SpawnFlyingEnemies();
        SpawnHearts();
        SpawnPortal();
    }

    private void SpawnGroundEnemies()
    {
        foreach (var pos in _groundEnemyPositions)
        {
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

    private void SpawnFlyingEnemies()
    {
        foreach (var pos in _flyingEnemyPositions)
        {
            Box2 startBox = new Box2(
                pos.X,
                pos.Y,
                pos.X + TextureManager.FlyingEnemyRectangle.Size.X,
                pos.Y + TextureManager.FlyingEnemyRectangle.Size.Y
            );

            var flying = new FlyingEnemy(startBox, _textureManager._flyingEnemy);
            flying.OnDeath += FlyingEnemyDeath;
            _state.FlyingEnemies.Add(flying);
        }
    }

    private void SpawnHearts()
    {
        foreach (var pos in _heartPositions)
        {
            Console.WriteLine($"Spawning heart at {pos}");
            _state.Hearts.Add(new Heart(pos, _textureManager._heart));
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

    public void SpawnEndboss()
    {
        if (_state.CurrentLevel == 2)
        {
            _state.Endboss = new Endboss(
                _state,
                TextureManager.EndbossRectangle,
                _textureManager
            );
        }
        else
        {
            _state.Endboss = null;
        }
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
        _state.Hearts.ForEach(heart => heart.Update((float)frameArgs.Time));
        _state.Portal?.Update((float)frameArgs.Time, _state.Enemies, _state.FlyingEnemies, _state.PlayerBox);
        _state.Endboss?.Update((float)frameArgs.Time, _state.PlayerBox);

        _collisionHandler.CheckAllEnemyCollisions();
    }
}
