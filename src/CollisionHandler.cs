using OpenTK.Mathematics;

namespace SpaceHunter;

public class CollisionHandler
{
    private List<Box2> _enemyBoxes = new List<Box2>();
    private Box2 _playerBox;
    
    // TODO, method for finding enemy which is touching player
    // TODO, method for finding collision with enemy projectile
    
    public static bool TwoBoxCollisionCheck(Box2 a, Box2 b)
    {
        bool xCollision = !(a.Max.X <= b.Min.X || a.Min.X >= b.Max.X);
        bool yCollision = !(a.Max.Y <= b.Min.Y || a.Min.Y >= b.Max.Y);
        
        return xCollision && yCollision;
    }
}