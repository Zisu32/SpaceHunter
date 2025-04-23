using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SpaceHunter.Models;

namespace SpaceHunter;

public class CollisionHandler
{
    private readonly GameState _state;
    private List<Box2> _enemyBoxes = new();
    private double _damageCooldown;

    public CollisionHandler(GameState state)
    {
        _state = state;
    }

    // TODO, method for finding collision with enemy projectile

    public void Update(FrameEventArgs frameArgs)
    {
        if (_damageCooldown <= 0.0)
        {
            EnemyCollisionCheck();
        }
        else
        {
            Console.WriteLine("collision -- invincible");
            _damageCooldown -= frameArgs.Time;
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
                Console.WriteLine("Enemy damage");
                
                // player can only damage one enemy
                return;
            }
        }
    }

    private void EnemyCollisionCheck()
    {
        // any enemy Box has collision with PlayerBox
        if (_state.Enemies.Any(enemy => TwoBoxCollisionCheck(_state.PlayerBox, enemy.Box)))
        {
            _state.PlayerHealth -= ConstantBalancingValues.EnemyDamage;
            _damageCooldown = ConstantBalancingValues.InvincibleDuration;
            _state.IsPlayerHurt = true;
            _state.PlayerHurtTimer = 1.0;
        }
    }

    public static bool TwoBoxCollisionCheck(Box2 a, Box2 b)
    {
        bool xCollision = !(a.Max.X <= b.Min.X || a.Min.X >= b.Max.X);
        bool yCollision = !(a.Max.Y <= b.Min.Y || a.Min.Y >= b.Max.Y);

        return xCollision && yCollision;
    }
}