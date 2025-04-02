using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace SpaceHunter;

public class Heart
{
    public Box2 Box { get; set; }
    public bool IsCollected { get; set; } = false;

    public Heart(Vector2 position)
    {
        float size = 1f; // Heart size
        Box = new Box2(position.X, position.Y, position.X + size, position.Y + size);
    }

    public static void DrawHeart(Box2 box, float time)
    {
        int points = 30;

        // Bouncing scale using sine wave
        float bounce = 1f + 0.1f * MathF.Sin(time * 5f); // Frequency/speed multiplier
        float baseScale = MathF.Min(box.Size.X, box.Size.Y) / 32f;
        float scale = bounce * baseScale;

        float centerX = (box.Min.X + box.Max.X) / 2f;
        float centerY = (box.Min.Y + box.Max.Y) / 2f;

        // Black Border
        GL.Color3(0f, 0f, 0f);
        GL.Begin(PrimitiveType.TriangleFan);
        GL.Vertex2(centerX, centerY);
        for (int i = 0; i <= points; i++)
        {
            float t = i * MathF.PI * 2f / points;
            float x = 16 * MathF.Pow(MathF.Sin(t), 3);
            float y = 13 * MathF.Cos(t) - 5 * MathF.Cos(2 * t) - 2 * MathF.Cos(3 * t) - MathF.Cos(4 * t);
            x *= scale * 1.3f;
            y *= scale * 1.3f;
            GL.Vertex2(centerX + x, centerY + y);
        }
        GL.End();

        // Red Heart
        GL.Color3(1.0f, 0.0f, 0.0f);
        GL.Begin(PrimitiveType.TriangleFan);
        GL.Vertex2(centerX, centerY);
        for (int i = 0; i <= points; i++)
        {
            float t = i * MathF.PI * 2f / points;
            float x = 16 * MathF.Pow(MathF.Sin(t), 3);
            float y = 13 * MathF.Cos(t) - 5 * MathF.Cos(2 * t) - 2 * MathF.Cos(3 * t) - MathF.Cos(4 * t);
            x *= scale;
            y *= scale;
            GL.Vertex2(centerX + x, centerY + y);
        }
        GL.End();
    }
}