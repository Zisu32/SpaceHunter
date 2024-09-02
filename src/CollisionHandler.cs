using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using SpaceHunter.Models;

namespace SpaceHunter;

public class CollisionHandler
{
    private readonly GameState _state;
    private List<Box2> _enemyBoxes = new List<Box2>();

    public CollisionHandler(GameState state)
    {
        _state = state;
    }

    // TODO, method for finding enemy which is touching player
    // TODO, method for finding collision with enemy projectile

    public void Update(FrameEventArgs frameArgs)
    {
        foreach (Box2 enemyBox in _state.enemyBoxes)
        {
            bool enemyCollision = TwoBoxCollisionCheck(_state.PlayerBox, enemyBox);
            if (enemyCollision)
            {
                Console.WriteLine("Player collision");
                _state.PlayerAlive = false;
                _state.playerState = PlayerState.death;
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