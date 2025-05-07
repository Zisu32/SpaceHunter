using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTKLib;
using Zenseless.OpenTK;

namespace SpaceHunter;

public class Heart
{
    public Box2 Box { get; set; }
    public bool IsCollected { get; set; } = false;

    private float _elapsedTime = 0f;
    private readonly Texture2D _texture;

    public Heart(Vector2 position, Texture2D texture)
    {
        float size = 1f;
        Box = new Box2(position.X, position.Y, position.X + size, position.Y + size);
        _texture = texture;
    }

    public void Update(float deltaTime)
    {
        _elapsedTime += deltaTime;
    }

    public void DrawHeart()
    {
        float bounce = 1f + 0.1f * MathF.Sin(_elapsedTime * 5f);

        Vector2 center = Box.Center;
        Vector2 size = Box.Size * bounce;

        Box2 bounceBox = new Box2(
            center - size / 2f,
            center + size / 2f
        );

        TextureHelper.DrawRectangularTexture(bounceBox, _texture.Handle);
    }
}