using OpenTK.Mathematics;
using Zenseless.OpenTK;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

public class Enemy
{
    private int _health = ConstantBalancingValues.EnemyHealth;

    private float _animationTimer = 0f;
    private uint _currentFrame = 0;
    private readonly uint _frameCount;
    private readonly uint _columns;
    private readonly uint _rows;

    public readonly double IdleMovementDelay;
    public bool IdleMoving = false;

    public Enemy(double idleMovementDelay, double idleMovementDelayRandom, uint rows = 1, uint columns = 4)
    {
        IdleMovementDelay = idleMovementDelay;

        _columns = columns;
        _rows = rows;
        _frameCount = _columns * _rows;
    }

    public Box2 Box { get; set; }
    public Box2 TargetBox { get; set; }

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

    public double LastIdleMovement = 0;
    public double CurrentIdleMovementRandom = 0;

    public event EventHandler? OnDeath;

    public void Update(float deltaTime)
    {
        // Animation frame update
        _animationTimer += deltaTime;
        if (_animationTimer > 0.2f) // adjust speed as you like
        {
            _currentFrame = (_currentFrame + 1) % _frameCount;
            _animationTimer = 0f;
        }
    }

    public void DrawEnemy(Texture2D texture)
    {
        TextureHelper.DrawSprite(Box, texture.Handle, _currentFrame, _columns, _rows);
    }
}