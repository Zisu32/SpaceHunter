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
            _damageCooldown-= frameArgs.Time;
        }
    }

    private void EnemyCollisionCheck()
    {
        foreach (Box2 enemyBox in _state.enemyBoxes)
        {
            if (TwoBoxCollisionCheck(_state.PlayerBox, enemyBox))
            {
                Console.WriteLine("Player collision");

                _state.PlayerHealth -= ConstantBalancingValues.EnemyDamage;
                _damageCooldown = ConstantBalancingValues.InvincibleDuration;

                break;
            }
        }
    }

    private static bool TwoBoxCollisionCheck(Box2 a, Box2 b)
    {
        bool xCollision = !(a.Max.X <= b.Min.X || a.Min.X >= b.Max.X);
        bool yCollision = !(a.Max.Y <= b.Min.Y || a.Min.Y >= b.Max.Y);

        return xCollision && yCollision;
    }
}