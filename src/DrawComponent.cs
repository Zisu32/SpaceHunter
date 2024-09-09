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

    public async Task Draw(FrameEventArgs obj)
    {
        _textureManager.DrawBackground();
        _textureManager.DrawPlayerTex(_state.PlayerBox, _state.playerState, obj);

        DebugDrawHelper.DrawRectangle(_state.PlayerBox, Color.Blue);
        if (_state.PlayerHitBox != null)
        {
            DebugDrawHelper.DrawRectangle(_state.PlayerHitBox.Value, Color.Yellow);
        }

        foreach (Enemy enemy in _state.enemies)
        {
            _textureManager.DrawEnemy(enemy.Box);
            DebugDrawHelper.DrawRectangle(enemy.Box, Color.Red);
        }

        _textureManager.DrawHealthbar();

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