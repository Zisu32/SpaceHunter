using OpenTK.Mathematics;

namespace SpaceHunter.Models;

public class GameState
{
    #region Player Fields

    public Box2 PlayerBox = new Box2(0, 0, TextureSizes.PlayerSizeX, TextureSizes.PlayerSizeY);
    public Box2 PlayerHitBox => PlayerBox
        .Scaled(ConstantBalancingValues.PlayerHitBoxScale, PlayerBox.Center + new Vector2(0f, 2f));
    public Box2 DebugPlayerBox => PlayerHitBox;

    public Box2? PlayerAttackBox = null;

    public bool PlayerAlive => PlayerHealth > 0;

    public int PlayerHealth = ConstantBalancingValues.MaxPlayerHealth;

    public PlayerState PlayerState = PlayerState.idle_r;

    //red flash on damage
    public bool IsPlayerHurt { get; set; } = false;
    public double PlayerHurtTimer { get; set; } = 0.0;

    #endregion

    public Portal? Portal { get; set; }
    public readonly List<Heart> Hearts = new();
    public readonly List<FlyingEnemy> FlyingEnemies = new();
    public readonly List<Enemy> Enemies = new();
    public float LevelWidth { get; set; } = 250f;

    public Endboss? Endboss { get; set; }
    public bool PlayerInAir = false;
    public bool IsGameStarted = false;

    public WorldHandler WorldHandler { get; set; }

    public bool IsShowingLevelTransition { get; set; } 
    public double LevelTransitionTimer { get; set; }

    // Neue Level-Logik
    public int CurrentLevel = 1;
    public int MaxLevels = 2;

    public void NextLevel()
    {
        if (CurrentLevel < MaxLevels)
        {
            CurrentLevel++;
            Enemies.Clear();
            FlyingEnemies.Clear();
            WorldHandler.SpawnEndboss();
            PlayerBox = new Box2(0, 0, TextureSizes.PlayerSizeX, TextureSizes.PlayerSizeY); // Spieler zurücksetzen
        }
        else
        {
            // Spiel beenden oder zurücksetzen
            CurrentLevel = 1;
            IsGameStarted = false; // Zurück zum Hauptmenü
        }
    }
}