using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using SpaceHunter.Models;

namespace SpaceHunter;

public class EnemyLogic
{
    private const int MaxFloat = 3;
    private const float FloatPerFrame = 0.3f;
    private static readonly Random _random = new();

    public static void FloatMovement(Enemy enemy)
    {
        Vector2 enemyGroundCenter = enemy.GroundBox.Center;

        // TODO, don't move linearly
        float newY = enemyGroundCenter.Y + (enemy.FloatingPosition * 0.01f) * MaxFloat
                                         + enemy.Box.Size.Y * 0.5f; // prevent floating into the ground

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

    public static void SidewaysMovement(Enemy enemy, FrameEventArgs frameArgs)
    {
        if (enemy.IdleMoving)
        {
            // this only moves the box on the X Axis
            Box2 enemyBox = enemy.Box;
            Vector2 enemyBoxCenter = enemyBox.Center;
            float differenceX = enemy.TargetBox.Center.X - enemyBoxCenter.X;

            // TODO this does not move linearly
            enemyBoxCenter.X += differenceX * 0.01f;
            enemyBox.Center = enemyBoxCenter;
            enemy.Box = enemyBox;


            if (Vector2.Distance(enemyBoxCenter, enemy.TargetBox.Center) <= 0.1f)
            {
                enemy.IdleMoving = false;
                enemy.LastIdleMovement = 0;
                enemy.CurrentIdleMovementRandom = _random.NextDouble(); // is between 0 and 1
            }

            return;
        }

        enemy.LastIdleMovement += frameArgs.Time;

        if (enemy.LastIdleMovement >
            enemy.IdleMovementDelay + enemy.CurrentIdleMovementRandom)
        {
            enemy.IdleMoving = true;

            Box2 newTarget = new Box2(enemy.Box.Min, enemy.Box.Max);
            Vector2 newTargetCenter = newTarget.Center;
            float distance = (float)_random.NextDouble() * 5f;

            if (_random.Next(2) == 1)
            {
                distance *= -1;
            }
            
            newTargetCenter.X += distance;

            newTarget.Center = newTargetCenter;
            enemy.TargetBox = newTarget;
        }
    }
}