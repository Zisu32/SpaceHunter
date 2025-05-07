using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SpaceHunter.Models;

namespace SpaceHunter;

public class CollisionHandler
{
    private readonly GameState _state;
    private double _damageCooldown;

    public CollisionHandler(GameState state)
    {
        _state = state;
    }

    // TODO, method for finding collision with enemy projectile

    public void UpdateCooldown(FrameEventArgs frameArgs)
    {
        if (_damageCooldown > 0f)
            _damageCooldown -= (float)frameArgs.Time;

        if (_damageCooldown < 0f)
        {
            _damageCooldown = 0f;
        }

        PlayerAttackCheck();
    }

    private void PlayerAttackCheck()
    {
        if (_state.PlayerHitBox == null)
        {
            return;
        }

        // TODO LINQ first?
        foreach (Enemy enemy in _state.Enemies)
        {
            if (TwoBoxCollisionCheck(_state.PlayerHitBox.Value, enemy.Box))
            {
                enemy.Health -= ConstantBalancingValues.AttackDamage;
                Console.WriteLine("Enemy took damage");
                
                // player can only damage one enemy
                return;
            }
        }
        foreach (FlyingEnemy flyingEnemy in _state.FlyingEnemies)
        {
            if (TwoBoxCollisionCheck(_state.PlayerHitBox.Value, flyingEnemy.Bounds))
            {
                flyingEnemy.Health -= ConstantBalancingValues.AttackDamage;
                Console.WriteLine("Flying took enemy damage");
                
                // player can only damage one enemy
                return; 
            }
        }
    }

    private void CheckCollisionWithEnemies(IEnumerable<Box2> enemyBoxes)
    {
        if (_damageCooldown > 0f) return; //No Damge while invincible

        if (enemyBoxes.Any(box => TwoBoxCollisionCheck(_state.PlayerBox, box)))
        {
            _state.PlayerHealth -= ConstantBalancingValues.EnemyDamage;
            _damageCooldown = ConstantBalancingValues.InvincibleDuration;
            _state.IsPlayerHurt = true;
            _state.PlayerHurtTimer = 1.0;
            Console.WriteLine("Player took damage!");

        }
    }
    public void CheckAllEnemyCollisions()
    {
        CheckCollisionWithEnemies(_state.Enemies.Select(e => e.Box));
        CheckCollisionWithEnemies(_state.FlyingEnemies.Select(e => e.Bounds));
        CheckLaserCollisions();
    }

    private void CheckLaserCollisions()
    {
        if (_damageCooldown > 0f) return;

        foreach (var enemy in _state.Enemies)
        {
            foreach (var laser in enemy.LaserBeams)
            {
                if (TwoBoxCollisionCheck(_state.PlayerBox, laser))
                {
                    _state.PlayerHealth -= 10;
                    _damageCooldown = ConstantBalancingValues.InvincibleDuration;
                    _state.IsPlayerHurt = true;
                    _state.PlayerHurtTimer = 1.0;
                    Console.WriteLine("Player hit by laser!");
                    return; // Only one laser hit per frame
                }
            }
        }
    }

    public static bool TwoBoxCollisionCheck(Box2 a, Box2 b)
    {
        bool xCollision = !(a.Max.X <= b.Min.X || a.Min.X >= b.Max.X);
        bool yCollision = !(a.Max.Y <= b.Min.Y || a.Min.Y >= b.Max.Y);

        return xCollision && yCollision;
    }
}