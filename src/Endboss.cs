using OpenTK.Mathematics;
using Zenseless.OpenTK;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

public class Endboss
{
    private readonly GameState _state;
    private static readonly Random _random = new Random();
    private readonly Texture2D _texture;
    private Box2 _position;
    private float _animationTimer;
    private uint _currentFrame;
    private readonly uint _frameCount;
    private readonly uint _columns;
    private readonly uint _rows;
    private int _health = ConstantBalancingValues.EnemyHealth;
    private bool _idleMoving = false;
    private double _lastIdleMovement = 0;
    private double _currentIdleMovementRandom = 0;
    private Box2 _targetBox;

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
    public Endboss(GameState state, Box2 position, Texture2D texture, uint columns = 6, uint rows = 1)
    {
        _state = state;
        _position = position;
        _texture = texture;
        _frameCount = columns * rows;
        _columns = columns;
        _rows = rows;
        _animationTimer = 0f;
        _currentFrame = 0;
        _targetBox = _position;
    }

    public void Update(float deltaTime, Box2 playerBox)
    {
        Animate(deltaTime);
        Move(deltaTime);
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
    private void Move(float deltaTime)
{
    if (_idleMoving)
    {
        Vector2 center = _position.Center;
        float differenceX = _targetBox.Center.X - center.X;

        // Move smoothly towards target
        center.X += differenceX * 0.01f;

        Vector2 min = _position.Min;
        Vector2 max = _position.Max;

        float move = differenceX * 0.01f;
        min.X += move;
        max.X += move;

        _position = new Box2(min, max);

        // Close enough to stop moving
        if (Math.Abs(differenceX) <= 0.1f)
        {
            _idleMoving = false;
            _lastIdleMovement = 0;
            _currentIdleMovementRandom = _random.NextDouble();
        }

        return;
    }

    // Not moving: check if time to move
    _lastIdleMovement += deltaTime;

    if (_lastIdleMovement > 2.0 + _currentIdleMovementRandom)
    {
        // 50% chance to decide to move at all
        if (_random.NextDouble() < 0.5)
        {
            _idleMoving = true;

            // Border checks
            float offset = 0f;
            float leftBorder = 0f;
            float rightBorder = 16f * 4.5f;  // Use same value as your background width (BackgroundRectangle)

            float buffer = 1.0f;  // Buffer zone so it doesn't "stick" to the edge

            Vector2 currentCenter = _position.Center;

            // At left edge? Always move right
            if (currentCenter.X <= leftBorder + buffer)
            {
                offset = 5f;  // Move right
            }
            // At right edge? Always move left
            else if (currentCenter.X >= rightBorder - buffer)
            {
                offset = -5f;  // Move left
            }
            else
            {
                // Otherwise, random choice
                offset = (_random.Next(0, 2) == 0 ? -1f : 1f) * 5f;
            }

            // Set new target
            Vector2 newCenter = currentCenter;
            newCenter.X += offset;

            Vector2 min = _position.Min;
            Vector2 max = _position.Max;
            Vector2 size = _position.Size;

            min.X = newCenter.X - size.X / 2f;
            max.X = newCenter.X + size.X / 2f;

            _targetBox = new Box2(min, max);
        }

        // Reset timer even if no move was chosen
        _lastIdleMovement = 0;
        _currentIdleMovementRandom = _random.NextDouble();
    }
}

    public void DrawEndboss()
    {
        TextureHelper.DrawSprite(_position, _texture.Handle, _currentFrame, _columns, _rows);
    }
}
