using OpenTK.Mathematics;
using SpaceHunter.Models;

namespace SpaceHunter;

public class EnemyLogic
{
    private const int MaxFloat = 5;
    private const float FloatPerFrame = 0.1f;

    public static void FloatMovement(Enemy enemy)
    {
        Vector2 enemyGroundCenter = enemy.GroundBox.Center;

        float newY = enemyGroundCenter.Y + (enemy.FloatingPosition * 0.01f) * MaxFloat
                                         + enemy.Box.Size.Y * 0.5f; // prevent floating into the ground

        Console.WriteLine("newY = " + newY);

        Box2 enemyBox = enemy.Box;
        Vector2 enemyCenter = enemyBox.Center;

        enemyCenter.Y = newY;
        enemyBox.Center = enemyCenter;
        enemy.Box = enemyBox;

        if (!enemy.FloatingDirection)
        {
            enemy.FloatingPosition += FloatPerFrame;
        }
        else
        {
            enemy.FloatingPosition -= FloatPerFrame;
        }

        if (enemy.FloatingPosition <= 0)
        {
            enemy.FloatingDirection = false;
        }
        else if (enemy.FloatingPosition >= 100)
        {
            enemy.FloatingDirection = true;
        }
    }
}