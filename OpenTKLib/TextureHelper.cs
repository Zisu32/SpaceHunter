using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Zenseless.OpenTK;
using Zenseless.Patterns;
using SpaceHunter;

namespace OpenTKLib;

public static class TextureHelper
{
    private static readonly Box2 DefaultBox = new Box2(0f, 0f, 1f, 1f);
    static uint spriteId = 0;

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
    public static void DrawSprite2Col(Box2 rectangle, Handle<Texture> texture, uint spriteId)
    {
        GL.BindTexture(TextureTarget.Texture2D, texture);

        // how many sprites are in each column and row
        const uint columns = 2;
        const uint rows = 1;
        // calculate the current frame of an animation
        // var spriteId = (uint)MathF.Round(texture.NormalizedAnimationTime * (columns * rows - 1));
        var texCoords = SpriteSheetTools.CalcTexCoords(spriteId, columns, rows);
        //Draw(texture.Bounds, texCoords);

        // prevent color problems
        GL.Color4(Color.White);

        GL.Begin(PrimitiveType.Quads);

        // bottom left
        GL.TexCoord2(texCoords.Min);
        GL.Vertex2(rectangle.Min);
        // bottom right
        GL.TexCoord2(texCoords.Max.X, texCoords.Min.Y);
        GL.Vertex2(rectangle.Max.X, rectangle.Min.Y);
        // top right
        GL.TexCoord2(texCoords.Max);
        GL.Vertex2(rectangle.Max);
        // top left
        GL.TexCoord2(texCoords.Min.X, texCoords.Max.Y);
        GL.Vertex2(rectangle.Min.X, rectangle.Max.Y);

        GL.End();
    }
    
    public static void DrawSprite4Col(Box2 rectangle, Handle<Texture> texture, FrameEventArgs obj)
    {
        GL.BindTexture(TextureTarget.Texture2D, texture);

        // how many sprites are in each column and row
        const uint columns = 4;
        const uint rows = 1;
        
        float NormalizedAnimationTime = (float)(obj.Time);
        if (NormalizedAnimationTime >= 0)
        {
            spriteId = (spriteId + 1) % 4; 
        }
        Console.WriteLine(spriteId);
        var texCoords = SpriteSheetTools.CalcTexCoords(spriteId, columns, rows);


        // prevent color problems
        GL.Color4(Color.White);

        GL.Begin(PrimitiveType.Quads);

        // bottom left
        GL.TexCoord2(texCoords.Min);
        GL.Vertex2(rectangle.Min);
        // bottom right
        GL.TexCoord2(texCoords.Max.X, texCoords.Min.Y);
        GL.Vertex2(rectangle.Max.X, rectangle.Min.Y);
        // top right
        GL.TexCoord2(texCoords.Max);
        GL.Vertex2(rectangle.Max);
        // top left
        GL.TexCoord2(texCoords.Min.X, texCoords.Max.Y);
        GL.Vertex2(rectangle.Min.X, rectangle.Max.Y);

        GL.End();
        System.Threading.Thread.Sleep(200);
    }
    
    public static void DrawSprite6Col(Box2 rectangle, Handle<Texture> texture, uint spriteId)
    {
        GL.BindTexture(TextureTarget.Texture2D, texture);

        // how many sprites are in each column and row
        const uint columns = 6;
        const uint rows = 1;
        // calculate the current frame of an animation
        // var spriteId = (uint)MathF.Round(texture.NormalizedAnimationTime * (columns * rows - 1));
        var texCoords = SpriteSheetTools.CalcTexCoords(spriteId, columns, rows);
        //Draw(texture.Bounds, texCoords);

        // prevent color problems
        GL.Color4(Color.White);

        GL.Begin(PrimitiveType.Quads);

        // bottom left
        GL.TexCoord2(texCoords.Min);
        GL.Vertex2(rectangle.Min);
        // bottom right
        GL.TexCoord2(texCoords.Max.X, texCoords.Min.Y);
        GL.Vertex2(rectangle.Max.X, rectangle.Min.Y);
        // top right
        GL.TexCoord2(texCoords.Max);
        GL.Vertex2(rectangle.Max);
        // top left
        GL.TexCoord2(texCoords.Min.X, texCoords.Max.Y);
        GL.Vertex2(rectangle.Min.X, rectangle.Max.Y);

        GL.End();
    }
    
    public static void DrawSprite8Col(Box2 rectangle, Handle<Texture> texture, uint spriteId)
    {
        GL.BindTexture(TextureTarget.Texture2D, texture);

        // how many sprites are in each column and row
        const uint columns = 8;
        const uint rows = 1;
        // calculate the current frame of an animation
        // var spriteId = (uint)MathF.Round(texture.NormalizedAnimationTime * (columns * rows - 1));
        var texCoords = SpriteSheetTools.CalcTexCoords(spriteId, columns, rows);
        //Draw(texture.Bounds, texCoords);

        // prevent color problems
        GL.Color4(Color.White);

        GL.Begin(PrimitiveType.Quads);

        // bottom left
        GL.TexCoord2(texCoords.Min);
        GL.Vertex2(rectangle.Min);
        // bottom right
        GL.TexCoord2(texCoords.Max.X, texCoords.Min.Y);
        GL.Vertex2(rectangle.Max.X, rectangle.Min.Y);
        // top right
        GL.TexCoord2(texCoords.Max);
        GL.Vertex2(rectangle.Max);
        // top left
        GL.TexCoord2(texCoords.Min.X, texCoords.Max.Y);
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