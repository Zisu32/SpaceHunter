using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace OpenTKLib;

public static class DebugDrawHelper
{
    public static void DrawRectangle(Box2 position, Color4 color)
    {
        GL.BindTexture(TextureTarget.Texture2D, 0);
        GL.Color4(color);
        GL.LineWidth(5f);
        
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(position.Min);
        GL.Vertex2(position.Max.X, position.Min.Y);
        GL.End();
        
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(position.Max.X, position.Min.Y);
        GL.Vertex2(position.Max);
        GL.End();
        
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(position.Max);
        GL.Vertex2(position.Min.X, position.Max.Y);
        GL.End();
        
        GL.Begin(PrimitiveType.Lines);
        GL.Vertex2(position.Min.X, position.Max.Y);
        GL.Vertex2(position.Min);
        GL.End();
    }
}