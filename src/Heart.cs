using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace SpaceHunter;

public class Heart
{
    public Box2 Box { get; set; }
    public bool IsCollected { get; set; } = false;

    private float _elapsedTime = 0f;

    public Heart(Vector2 position)
    {
        float size = 1f; // Heart size
        Box = new Box2(position.X, position.Y, position.X + size, position.Y + size);
    }

    public void Update(float deltaTime)
    {
        _elapsedTime += deltaTime;
    }

    public void DrawHeart()
    {
        float bounce = 1f + 0.1f * MathF.Sin(_elapsedTime * 5f);
        float pixelSize = MathF.Min(Box.Size.X, Box.Size.Y) / 8f * bounce;

        float centerX = (Box.Min.X + Box.Max.X) / 2f;
        float centerY = (Box.Min.Y + Box.Max.Y) / 2f;

        // 8x8 pixel heart layout (1 = pixel on, 0 = off)
        int[,] heartPixels = new int[,]
        {
            { 0, 1, 1, 0, 0, 1, 1, 0 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 1, 1, 1, 1, 1, 1, 1, 1 },
            { 0, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 1, 1, 1, 1, 0, 0 },
            { 0, 0, 0, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0 },
        };

        // Calculate top-left of heart grid
        float startX = centerX - 4 * pixelSize;
        float startY = centerY + 4 * pixelSize;

        GL.Color3(0.80f, 0.0f, 0.0f);

        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                if (heartPixels[y, x] == 1)
                {
                    float px = startX + x * pixelSize;
                    float py = startY - y * pixelSize;
                    DrawPixel(px, py, pixelSize);
                }
            }
        }
    }

    private static void DrawPixel(float x, float y, float size)
    {
        GL.Begin(PrimitiveType.Quads);
        GL.Vertex2(x, y);
        GL.Vertex2(x + size, y);
        GL.Vertex2(x + size, y - size);
        GL.Vertex2(x, y - size);
        GL.End();
    }
}