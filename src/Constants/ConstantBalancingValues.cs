
using OpenTK.Mathematics;

namespace SpaceHunter;

public static class ConstantBalancingValues
{
    public const int MaxPlayerHealth = 100;
    public const int EnemyDamage = 10;
    public const int EnemyHealth = 10;
    public const int EndbossHealth = 50;
    public const double InvincibleDuration = 1.0; // seconds
    public const double AttackDuration = 0.5; // seconds
    public const int AttackDamage = 10;
    public const float AttackBoxLength = 0.6f;
    public static readonly Vector2 PlayerHitBoxScale = new Vector2(0.5f, 0.9f);
}