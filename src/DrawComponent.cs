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
        // DrawBlockGrid();
        
        _textureManager.DrawBackground();
        // _textureManager.DrawPlayer(_state.PlayerBox);
        
        DrawPlayer();
        
        // DrawDebugLine(_state.DebugLineHeight);

        ErrorCode errorCode = GL.GetError();

        if (errorCode != ErrorCode.NoError)
        {
            Console.WriteLine("GL Error");
        }
    }

    public void Initialize()
    {
        _textureManager.Initialize();
    }

    private void DrawBall()
    {
        GL.Color4(Color.White);

        float ballX = _state.BallXPosition * 2.0f - 1;
        float ballY = _state.BallYPosition * 2.0f - 1;
        GenericDrawFunctions.DrawCircle(new Vector2(ballX, ballY),
            CircleSegments, 0.08);
    }


    private void DrawBlockGrid()
    {
        const float quadSizeX = (2.0f / GameState.BlocksHorizontal);
        const float quadSizeY = (1.0f / GameState.BlocksVertical);

        for (int y = 0; y < GameState.BlocksVertical; y++)
        {
            for (int x = 0; x < GameState.BlocksHorizontal; x++)
            {
                float colorY = y * (1.0f / GameState.BlocksVertical);

                // HSV: x is hue, y is Saturation, z is value, w is alpha
                GL.Color4(Color4.FromHsv(new Vector4(colorY, 1, 1, 1)));

                GL.Begin(BeginMode.Quads);
                // bottom left
                GL.Vertex2(quadSizeX * x, quadSizeY * y);
                // bottom right
                GL.Vertex2(quadSizeX * x + quadSizeX - Space, quadSizeY * y);
                // top right
                GL.Vertex2(quadSizeX * x + quadSizeX - Space, quadSizeY * y + quadSizeY - Space);
                // top Left
                GL.Vertex2(quadSizeX * x, quadSizeY * y + quadSizeY - Space);

                GL.End();
            }
        }
    }

    public static void DrawDebugLine(float height)
    {
        GL.Color4(Color.White);

        GL.Begin(BeginMode.Quads);

        // GenericDrawFunctions.DrawLineY(height, Color.White);

        GenericDrawFunctions.DrawLineX(height, Color.White);

        GL.End();
    }

    private void DrawPlayer()
    {
        _textureManager.DrawPlayer(_state.PlayerBox);
    }
}