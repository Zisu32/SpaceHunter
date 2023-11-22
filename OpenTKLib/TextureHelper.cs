using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Zenseless.OpenTK;
using Zenseless.Patterns;

namespace OpenTKLib;

public static class TextureHelper
{
    private static readonly Box2 DefaultBox = new Box2(0f, 0f, 1f, 1f);

    /// <summary>
    /// Enables Capability to display textures including alpha channels
    /// </summary>
    public static void InitalizeOpenGLTextures()
    {
        GL.Enable(EnableCap.Texture2D);
        // these two calls are required to enable alpha on textures
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        GL.Enable(EnableCap.Blend);
    }
    
    public static void DrawRectangularTexture(Box2 rectangle, Handle<Texture> texture)
    {
        GL.BindTexture(TextureTarget.Texture2D, texture);

        // prevent color problems
        GL.Color4(Color.White);

        GL.Begin(PrimitiveType.Quads);

        // bottom left
        GL.TexCoord2(DefaultBox.Min);
        GL.Vertex2(rectangle.Min);
        // bottom right
        GL.TexCoord2(DefaultBox.Max.X, DefaultBox.Min.Y);
        GL.Vertex2(rectangle.Max.X, rectangle.Min.Y);
        // top right
        GL.TexCoord2(DefaultBox.Max);
        GL.Vertex2(rectangle.Max);
        // top left
        GL.TexCoord2(DefaultBox.Min.X, DefaultBox.Max.Y);
        GL.Vertex2(rectangle.Min.X, rectangle.Max.Y);

        GL.End();
    }

    public static Texture2D LoadNonFilteringTexture(string path)
    {
        Texture2D texture = ResourceLoader.LoadTexture(path);
        GL.TextureParameter(texture.Handle.Id, TextureParameterName.TextureMagFilter, (int)All.Nearest);
        GL.TextureParameter(texture.Handle.Id, TextureParameterName.TextureMinFilter, (int)All.Nearest);

        return texture;
    }
}