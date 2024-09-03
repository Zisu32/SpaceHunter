using OpenTK.Mathematics;

namespace SpaceHunter.Models;

public class GameState
{
    public Box2 PlayerBox = new Box2(0, 0, 5, 5);

    public Box2? PlayerHitBox = null;

    public bool PlayerAlive => PlayerHealth > 0;
    public int PlayerHealth = ConstantBalancingValues.MaxPlayerHealth;

    // TODO, replace Box2 with enemy class
    public List<Box2> enemyBoxes = new List<Box2>();
    public bool PlayerInAir = false;
    public PlayerState playerState = PlayerState.idle_r;
}