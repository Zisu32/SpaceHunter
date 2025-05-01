using System.Drawing;
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
    public readonly List<Box2> LaserBeams = new();
    public float _laserCooldown = 0f; // asnchron shooting
    public const float LaserCooldownTime = 5f; // Fires every 2 seconds
    public const float LaserSpeed = 10f; // Laser speed per second
    
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
        // Animation update
        _animationTimer += deltaTime;
        if (_animationTimer > 0.2f)
        {
            _currentFrame = (_currentFrame + 1) % _frameCount;
            _animationTimer = 0f;
        }

        // Laser shooting logic
        _laserCooldown -= deltaTime;
        if (_laserCooldown <= 0f)
        {
            ShootLaser();
            _laserCooldown = LaserCooldownTime;
        }

        // Move existing lasers
        for (int i = LaserBeams.Count - 1; i >= 0; i--)
        {
            var oldLaser = LaserBeams[i];
            var movedLaser = new Box2(
                oldLaser.Min.X - LaserSpeed * deltaTime,
                oldLaser.Min.Y,
                oldLaser.Max.X - LaserSpeed * deltaTime,
                oldLaser.Max.Y
            );

            // Travel not longer then 5f
            float distanceMoved = Box.Min.X - movedLaser.Max.X;

            if (distanceMoved >= 10f || movedLaser.Max.X < 0)
            {
                LaserBeams.RemoveAt(i);
            }
            else
            {
                LaserBeams[i] = movedLaser;
            }

        }
    }

    private void ShootLaser()
    {
        var laser = new Box2(                          //Need to be Box because CollisionHandler only compare Box vs Box
            Box.Min.X - 0.5f,                      // Start a bit left of the enemy
            Box.Center.Y - 0.02f,                // Y position (centered at enemy's middle height)
            Box.Min.X,                           // End X (left side of enemy)
            Box.Center.Y + 0.02f                 // Make it very thin
        );
        LaserBeams.Add(laser);
    }


    public void DrawEnemy(Texture2D texture)
    {
        TextureHelper.DrawSprite(Box, texture.Handle, _currentFrame, _columns, _rows);
        // Draw lasers
        foreach (var laser in LaserBeams)
        {
            DebugDrawHelper.DrawRectangle(laser, Color.Yellow);
        }
    }
}