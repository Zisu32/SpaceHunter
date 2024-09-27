using OpenTK.Mathematics;

namespace SpaceHunter.Models;

public class Enemy
{
    private int _health = ConstantBalancingValues.EnemyHealth;
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

    public event EventHandler? OnDeath;
}