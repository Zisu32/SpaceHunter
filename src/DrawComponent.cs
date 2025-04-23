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
    private Portal _portal;
    private bool _enteredPortal = false;


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
        //Debug Boxes (Blue for Player, Yellow for Hitbox)
        DebugDrawHelper.DrawRectangle(_state.PlayerBox, Color.Blue);
        if (_state.PlayerHitBox != null)
        {
            DebugDrawHelper.DrawRectangle(_state.PlayerHitBox.Value, Color.Yellow);
        }

        //Draw Enemies
        foreach (Enemy enemy in _state.Enemies)
        {
            _textureManager.DrawEnemy(enemy.Box);
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
        
        // TODO, extract gameLogic, only draw portal here
        //Draw Portal if all enemies are defeated
        _portal.IsVisible = !_state.Enemies.Any();
        _portal.Update((float)obj.Time);
        _portal.DrawPortal();
        if (_portal.IsVisible &&
            CollisionHandler.TwoBoxCollisionCheck(_state.PlayerBox, _portal.Bounds))
        {
            _enteredPortal = true;
            Console.WriteLine($"Enter Portal");
        }
    }

    private void DrawMenu()
    {
        _textureManager.DrawMenuScreen();
    }


    public void Initialize()
    {
        _textureManager.Initialize();
        _state.Hearts.Add(new Heart(new Vector2(15f, 2f)));
        _portal = new Portal(TextureManager.PortalRectangle, _textureManager._portalTexture);
    }

    public Camera Camera { get; set; }
}