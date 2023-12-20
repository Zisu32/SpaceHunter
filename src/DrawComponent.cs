using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using OpenTKLib;
using SpaceHunter.Models;

#pragma warning disable CS0618 // Type or member is obsolete

namespace SpaceHunter;

public class DrawComponent : IDrawComponent
{
    private readonly TextureManager _textureManager;
    private readonly GameState _state;
    private const int CircleSegments = 50;

    public DrawComponent(GameState state)
    {
        this._state = state;
        this._textureManager = new TextureManager();
    }

    public const float Space = 0.01f;

    public async Task Draw(FrameEventArgs obj)
    {
        _textureManager.DrawBackground();
        _textureManager.DrawPlayer(_state.PlayerBox);

        ErrorCode errorCode = GL.GetError();
        
        

        if (errorCode != ErrorCode.NoError)
        {
            Console.Error.WriteLine($"GL Error: {errorCode}");
        }
    }

    public void Initialize()
    {
        _textureManager.Initialize();
    }
}