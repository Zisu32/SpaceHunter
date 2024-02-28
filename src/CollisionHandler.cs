using OpenTK.Mathematics;

namespace SpaceHunter;

public class CollisionHandler
{
    public static bool TwoBoxCollisionCheck(Box2 a, Box2 b)
    {
        bool xCollision = !(a.Max.X <= b.Min.X || a.Min.X >= b.Max.X);
        bool yCollision = !(a.Max.Y <= b.Min.Y || a.Min.Y >= b.Max.Y);

        // if (xCollision)
        // {
        //     Console.WriteLine("xCollision");
        // }
        //
        // if (yCollision)
        // {
        //     Console.WriteLine("yCollision");
        // }
        
        return xCollision && yCollision;
    }
}