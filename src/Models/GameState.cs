using OpenTK.Mathematics;

namespace CameraTest.Models;

public class GameState
{
    public const int BlocksVertical = 7;

    public const int BlocksHorizontal = 10;

    // paddle is a quarter of window length
    public const float PlayerLength = 2.0f / 4;

    public float PlayerPosition { get; set; } = 0.5f;

    public bool[,] Grid { get; set; } = new bool[BlocksHorizontal, BlocksVertical];


    // range 0 - 1
    public float BallXPosition { get; set; } = 0.5f;
    public float BallYPosition { get; set; } = 0.11f;

    public Direction BallDirection = Direction.UpLeft;

    public float DebugLineHeight { get; set; } = 3.0f; // line isn't displayed at 3f

    public Box2 PlayerBox = new Box2(0, 0, 1, 1);
}