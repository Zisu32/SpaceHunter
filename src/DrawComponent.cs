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
    private readonly Healthbar _healthbar;

    //Constructur
    public DrawComponent(GameState state)
    {
        this._state = state;
        this._textureManager = new TextureManager();
        this._healthbar = new Healthbar();
    }

    public async Task Draw(FrameEventArgs obj)
    {
        if (!_state.IsGameStarted)
        {
            DrawMenu();
            return;
        }

        //Draw Background First
        _textureManager.DrawBackground();

        //Draw health bar
        _healthbar.DrawHealthBar(_state.PlayerHealth, ConstantBalancingValues.MaxPlayerHealth);

        //Draw Player Sprite inside the Blue Rectangle
        _textureManager.DrawPlayerTex(_state.PlayerBox, _state.PlayerState, obj, _state.IsPlayerHurt);

        // Vector2 playerBoxMax = _state.PlayerBox.Max;
        //
        // playerBoxMax.X += 2;
        //
        // Vector2 end = new Vector2(playerBoxMax.X, playerBoxMax.Y);
        //
        // end.X += 3;

        DrawEffects.DrawStaticEnemyLaser(new Vector2(5,5) , new Vector2(7,5));
        
        //Debug Boxes (Blue for Player, Yellow for Hitbox)
        DebugDrawHelper.DrawRectangle(_state.PlayerBox, Color.Blue);
        if (_state.PlayerHitBox != null)
        {
            DebugDrawHelper.DrawRectangle(_state.PlayerHitBox.Value, Color.Yellow);
        }

        //Draw Enemies
        foreach (Enemy enemy in _state.Enemies)
        {
            _textureManager.DrawBlueEnemy(enemy.Box);
            DebugDrawHelper.DrawRectangle(enemy.Box, Color.Red);
        }

        ErrorCode errorCode = GL.GetError();
        if (errorCode != ErrorCode.NoError)
        {
            Console.WriteLine($"OpenGL Error: {errorCode}");
        }

        //Draw Heart
        foreach (Heart heart in _state.Hearts)
        {
            if (!heart.IsCollected)
            {
                heart.Update((float)obj.Time);
                heart.DrawHeart();
            }
        }
    }

    private void DrawMenu()
    {
        _textureManager.DrawMenuScreen();
    }


    public void Initialize()
    {
        _textureManager.Initialize();
        _state.Hearts.Add(new Heart(new Vector2(30f, 2f)));
    }

    public Camera Camera { get; set; }
}