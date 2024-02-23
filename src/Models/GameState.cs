using OpenTK.Mathematics;

namespace SpaceHunter.Models;

public class GameState
{
    public Box2 PlayerBox = new Box2(0, 0, 5, 5);
    public bool PlayerInAir = false;
    public PlayerState playerState = PlayerState.idle;
}