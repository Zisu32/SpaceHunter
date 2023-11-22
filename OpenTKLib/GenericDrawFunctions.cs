using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenTKLib;

public static class GenericDrawFunctions
{
    public static void DrawCircle(Vector2 center, int segments, double radius)
    {
        GL.Begin(BeginMode.TriangleFan);
        // circle center
        GL.Vertex2(center);

        for (int i = 0; i <= segments; i++)
        {
            double theta = 2.0 * Math.PI * i / segments;
            double x = radius * Math.Cos(theta);
            double y = radius * Math.Sin(theta);

            GL.Vertex2(center.X + x, center.Y + y);
        }

        GL.End();
    }

    public static void DrawLineX(float position, Color color, float width = 0.005f)
    {
        GL.Color4(color);
        
        GL.Begin(BeginMode.Quads);
        
        GL.Vertex2(position - width, -1);
        GL.Vertex2(position - width, 1);
        GL.Vertex2(position, 1);
        GL.Vertex2(position, -1);
        
        GL.End();
    }


    public static void DrawLineY(float height, Color color, float width = 0.005f)
    {
        GL.Color4(color);
        
        GL.Begin(BeginMode.Quads);
        
        GL.Vertex2(-1, height - width);
        GL.Vertex2(1, height - width);
        GL.Vertex2(1, height);
        GL.Vertex2(-1, height);
        
        GL.End();
    }
}