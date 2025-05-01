using OpenTK.Mathematics;

namespace SpaceHunter.Models;

public class GameState
{
    #region Player Fields

    public Box2 PlayerBox = new Box2(0, 0, TextureSizes.PlayerSizeX, TextureSizes.PlayerSizeY);

    public Box2? PlayerHitBox = null;

    public bool PlayerAlive => PlayerHealth > 0;
    
    public int PlayerHealth = ConstantBalancingValues.MaxPlayerHealth;
    
    public PlayerState PlayerState = PlayerState.idle_r;
    
    //red flash on damage
    public bool IsPlayerHurt { get; set; } = false;
    public double PlayerHurtTimer { get; set; } = 0.0;
    #endregion

    public readonly List<Heart> Hearts = new(); 
    public readonly List<FlyingEnemy> FlyingEnemies = new();
    public readonly List<Enemy> Enemies = new();
    public bool PlayerInAir = false;
    public bool IsGameStarted = false;

}