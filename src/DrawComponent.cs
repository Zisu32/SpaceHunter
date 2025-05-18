using System.Drawing;
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
    private readonly Healthbar _healthbar;
    private bool _enteredPortal = false;
    private readonly CollisionHandler _collisionHandler;


    //Constructur
    public DrawComponent(GameState state, TextureManager textureManager)
    {
        this._state = state;
        this._textureManager = textureManager;
        this._healthbar = new Healthbar();
        this._collisionHandler = new CollisionHandler(_state);
    }

    public async Task Draw(FrameEventArgs obj)
    {
        if (_state.IsShowingLevelTransition)
        {
            _textureManager.DrawLevelTransition(_state.LevelTransitionTimer);
            return;
        }

        if (!_state.IsGameStarted)
        {
            DrawMenu();
            return;
        }

        // Update damage cooldown
        _collisionHandler.UpdateCooldown(obj);

        //Draw Background First
        _textureManager.DrawBackground(_state.CurrentLevel);

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
            enemy.DrawEnemy(_textureManager._staticEnemy);
        }

        //Draw Heart
        foreach (Heart heart in _state.Hearts)
        {
            if (!heart.IsCollected)
            {
                heart.DrawHeart();
            }
        }

        //Draw Portal
        _state.Portal?.DrawPortal();

        //Draw Endboss
        if (_state.Endboss != null)
        {
            _state.Endboss.Draw(obj);
        }
        
        // Draw FlyingEnemy
        foreach (FlyingEnemy flyingEnemy in _state.FlyingEnemies)
        {
            flyingEnemy.DrawFlyingEnemy();
        }
        

        ErrorCode errorCode = GL.GetError();
        if (errorCode != ErrorCode.NoError)
            Console.WriteLine($"OpenGL Error: {errorCode}");
    }

    private void DrawMenu()
    {
        _textureManager.DrawMenuScreen();
    }


    public void Initialize()
    {
    }

    public Camera Camera { get; set; }
}