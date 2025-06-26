using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTKLib;
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
        if (_state.PlayerAttackBox == null)
        {
            return;
        }

        foreach (Enemy enemy in _state.Enemies)
        {
            if (TwoBoxCollisionCheck(_state.PlayerAttackBox.Value, enemy.Box))
            {
                enemy.Health -= ConstantBalancingValues.AttackDamage;
                Console.WriteLine("Enemy took damage");

                // player can only damage one enemy
                return;
            }
        }

        foreach (FlyingEnemy flyingEnemy in _state.FlyingEnemies)
        {
            if (TwoBoxCollisionCheck(_state.PlayerAttackBox.Value, flyingEnemy.Bounds))
            {
                flyingEnemy.Health -= ConstantBalancingValues.AttackDamage;
                Console.WriteLine("Flying took enemy damage");

                // player can only damage one enemy
                return;
            }
        }

        if (_state.Endboss != null)
        {
            if (TwoBoxCollisionCheck(_state.PlayerAttackBox.Value, _state.Endboss.Bounds))
            {
                _state.Endboss.Health -= ConstantBalancingValues.AttackDamage;
                Console.WriteLine("Endboss took damage");
                return;
            }
        }
    }

    private void CheckCollisionWithEnemies(IEnumerable<Box2> enemyBoxes)
    {
        if (_damageCooldown > 0f) return; //No Damge while invincible

        Box2 playerBox = _state.PlayerHitBox;

        foreach (var enemybox in enemyBoxes)
        {
            Box2 shrinkenemybox = ShrinkBox(enemybox, .4f);

            if (TwoBoxCollisionCheck(playerBox, shrinkenemybox))
            {
                _state.PlayerHealth -= ConstantBalancingValues.EnemyDamage;
                _damageCooldown = ConstantBalancingValues.InvincibleDuration;
                _state.IsPlayerHurt = true;
                _state.PlayerHurtTimer = 1.0;
                Console.WriteLine("Player took damage!");
                break;
            }
        }
    }
    private void CheckPlayerHitByEndboss()
    {
        if (_damageCooldown > 0f || _state.Endboss == null) return;

        Box2 playerBox = _state.PlayerHitBox;
        Box2 endbossBox = _state.Endboss.Bounds;

        if (TwoBoxCollisionCheck(playerBox, endbossBox))
        {
            _state.PlayerHealth -= ConstantBalancingValues.EndbossDamage;  // Use Endboss damage constant
            _damageCooldown = ConstantBalancingValues.InvincibleDuration;
            _state.IsPlayerHurt = true;
            _state.PlayerHurtTimer = 1.0;
            Console.WriteLine("Player hit by Endboss!");
        }
    }


    private void CheckLaserCollisions()
    {
        if (_damageCooldown > 0f) return;

        foreach (var enemy in _state.Enemies)
        {
            foreach (var laser in enemy.LaserBeams)
            {
                if (TwoBoxCollisionCheck(_state.PlayerHitBox, laser))
                {
                    _state.PlayerHealth -= 10;
                    _damageCooldown = ConstantBalancingValues.InvincibleDuration;
                    _state.IsPlayerHurt = true;
                    _state.PlayerHurtTimer = 1.0;
                    Console.WriteLine("Player hit by laser!");
                    return;
                }
            }
        }

        if (_state.Endboss != null)
        {
            foreach (var (box, _, _) in _state.Endboss.LaserBeams)
            {
                if (TwoBoxCollisionCheck(_state.PlayerHitBox, box))
                {
                    _state.PlayerHealth -= 20;
                    _damageCooldown = ConstantBalancingValues.InvincibleDuration;
                    _state.IsPlayerHurt = true;
                    _state.PlayerHurtTimer = 1.0;
                    Console.WriteLine("Player hit by ENDBOSS laser!");
                    return;
                }
            }
        }
    }

    public void CheckAllEnemyCollisions()
    {
        CheckCollisionWithEnemies(_state.Enemies.Select(e => e.Box));
        CheckCollisionWithEnemies(_state.FlyingEnemies.Select(e => e.Bounds));
        if (_state.Endboss != null)
        {
            CheckCollisionWithEnemies(new[] { _state.Endboss.Bounds });
        }
        CheckPlayerHitByEndboss();
        CheckLaserCollisions();
    }

    public static bool TwoBoxCollisionCheck(Box2 a, Box2 b)
    {
        bool xCollision = !(a.Max.X <= b.Min.X || a.Min.X >= b.Max.X);
        bool yCollision = !(a.Max.Y <= b.Min.Y || a.Min.Y >= b.Max.Y);

        return xCollision && yCollision;
    }

    private static Box2 ShrinkBox(Box2 box, float scale)
    {
        return box.Scaled(new Vector2(scale, scale), box.Center);
    }
}