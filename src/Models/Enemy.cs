using OpenTK.Mathematics;

namespace SpaceHunter.Models;

public class Enemy
{
    private int _health = ConstantBalancingValues.EnemyHealth;

    public readonly double IdleMovementDelay;

    // public readonly double IdleMovementDelayRandom; // currently not used
    public bool IdleMoving = false;

    public Enemy(double idleMovementDelay, double idleMovementDelayRandom)
    {
        IdleMovementDelay = idleMovementDelay;
        // IdleMovementDelayRandom = idleMovementDelayRandom;
    }

    // goes from 0, at ground to 100, on top 
    public float FloatingPosition { get; set; }

    // false = up, true = down
    public bool FloatingDirection { get; set; }

    public Box2 GroundBox { get; set; }
    public Box2 TargetBox { get; set; }
    public Box2 Box { get; set; }

    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
            {
                // TODO death animation
                // TODO damage cooldown or smth.

                OnDeath?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public double LastIdleMovement = 0;

    // is just always initialized at 0
    public double CurrentIdleMovementRandom = 0;

    public event EventHandler? OnDeath;
}