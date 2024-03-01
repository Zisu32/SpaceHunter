using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
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
        _textureManager.DrawPlayerTex(_state.PlayerBox, _state.playerState, obj);
        
        foreach (Box2 enemyBox in _state.enemyBoxes)
        {
            _textureManager.DrawEnemy(enemyBox);
        }
        
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

    public Camera Camera { get; set; }
}