using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTKLib;
using Zenseless.OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SpaceHunter;

public class DrawEffects
{
    public static void DrawStaticEnemyLaser(Vector2 start, Vector2 end)
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.Color3(Color.Yellow);
        GL.LineWidth(5f);
        
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(start);
        GL.Vertex2(end);

        GL.End();
    }
}