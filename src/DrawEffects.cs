using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTKLib;
using Zenseless.OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SpaceHunter;

public class DrawEffects
{
    public static void DrawStaticEnemyLaser(Vector2 position, float length, int section)
    {
        // section: 0 - 9


        float sectionLength = length / 10;

        float startX = position.X + sectionLength * section;
        float endX = position.X + sectionLength;

        Vector2 start = new Vector2(startX, position.Y);
        Vector2 end = new Vector2(endX, position.Y);

        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.Color3(Color.Yellow);
        GL.LineWidth(5f);

        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(start);
        GL.Vertex2(end);

        GL.End();
    }
}