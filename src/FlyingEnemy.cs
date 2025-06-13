using OpenTK.Mathematics;
using Zenseless.OpenTK;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

public class FlyingEnemy
{
    private static readonly Random _random = new();
    private readonly Texture2D _texture;
    private Box2 _position;
    private float _animationTimer;
    private uint _currentFrame;
    private readonly uint _frameCount;
    private readonly uint _columns;
    private readonly uint _rows;

    private int _health = ConstantBalancingValues.EnemyHealth;

    private Vector2 _patrolStart;
    private Vector2 _patrolEnd;
    private float _patrolSpeed = 1.5f;
    private int _patrolDirection = 1; // 1 = right, -1 = left

    private bool _chasingPlayer = false;
    private const float PatrolDistance = 8f;
    private const float PlayerDetectionRange = 4f;
    private const float ChaseSpeed = 4f;

    public Box2 Bounds => _position;

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
            {
                OnDeath?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public event EventHandler? OnDeath;

    public FlyingEnemy(Box2 position, Texture2D texture, uint columns = 5, uint rows = 1)
    {
        _texture = texture;
        _position = position;
        _columns = columns;
        _rows = rows;
        _frameCount = columns * rows;

        _animationTimer = 0f;
        _currentFrame = 0;

        var center = _position.Center;
        _patrolStart = new Vector2(center.X - PatrolDistance / 2f, center.Y);
        _patrolEnd = new Vector2(center.X + PatrolDistance / 2f, center.Y);
    }

    public void Update(float deltaTime, Box2 playerBox)
    {
        Animate(deltaTime);

        float distanceToPlayer = Math.Abs(playerBox.Center.X - _position.Center.X);
        _chasingPlayer = distanceToPlayer <= PlayerDetectionRange;

        if (_chasingPlayer)
            ChasePlayer(deltaTime, playerBox);
        else
            Patrol(deltaTime);
    }

    private void Animate(float deltaTime)
    {
        _animationTimer += deltaTime;
        if (_animationTimer > 0.1f)
        {
            _currentFrame = (_currentFrame + 1) % _frameCount;
            _animationTimer = 0f;
        }
    }

    private void ChasePlayer(float deltaTime, Box2 playerBox)
    {
        float playerX = playerBox.Center.X;
        float currentX = _position.Center.X;

        float direction = Math.Sign(playerX - currentX);
        float move = direction * ChaseSpeed * deltaTime;

        float newX = currentX + move;

        // Clamp to patrol range
        newX = Math.Clamp(newX, _patrolStart.X, _patrolEnd.X);

        UpdatePositionX(newX);
    }

    private void Patrol(float deltaTime)
    {
        float currentX = _position.Center.X;
        float move = _patrolDirection * _patrolSpeed * deltaTime;
        float newX = currentX + move;

        // Reverse direction at patrol bounds
        if (newX > _patrolEnd.X)
        {
            newX = _patrolEnd.X;
            _patrolDirection = -1;
        }
        else if (newX < _patrolStart.X)
        {
            newX = _patrolStart.X;
            _patrolDirection = 1;
        }

        UpdatePositionX(newX);
    }

    private void UpdatePositionX(float newCenterX)
    {
        Vector2 size = _position.Size;
        Vector2 min = new Vector2(newCenterX - size.X / 2f, _position.Min.Y);
        Vector2 max = new Vector2(newCenterX + size.X / 2f, _position.Max.Y);
        _position = new Box2(min, max);
    }

    public void DrawFlyingEnemy()
    {
        TextureHelper.DrawSprite(_position, _texture.Handle, _currentFrame, _columns, _rows);
    }
}
