using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTKLib;
using Zenseless.OpenTK;

namespace SpaceHunter;

public class TextureManager
{
    private Texture2D _background;
    private Texture2D _player_idle;
    private Texture2D _player_run;
    private Texture2D _player_jump;
    private Texture2D _player_attack;
    private Texture2D _player_hurt;
    private Texture2D _player_death;

    private static readonly Box2 DefaultBox = new Box2(0f, 0f, 1f, 1f);

    public void DrawBackground()
    {
        TextureHelper.DrawRectangularTexture(new Box2(0f, 0f, 16f, 16f), _background.Handle);
    }

    public void DrawPlayerTex(Box2 position, PlayerState playerState, FrameEventArgs obj)
    {
        Texture2D texture2D = null;
        switch (playerState)
        {
            case PlayerState.idle:
                texture2D = _player_idle;
                break;
            case PlayerState.run:
                texture2D = _player_run;
                break;
            case PlayerState.jump:
                texture2D = _player_jump;
                break;
            case PlayerState.attack:
                texture2D= _player_attack;
                break;
            case PlayerState.hurt:
                texture2D=_player_hurt;
                break;
            case PlayerState.death:
                texture2D = _player_death;
                break;
        }

        TextureHelper.DrawSprite4Col(position, texture2D.Handle, obj);
        
    }

    public void Initialize()
    {
        TextureHelper.InitalizeOpenGLTextures();

        // Textures can only be loaded when a window is already being displayed (for some reason)
        _background = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.BG-1.jpg");
        _player_idle = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Cyborg_idle.png");
        _player_run = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Cyborg_run.png");
        _player_jump = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Cyborg_doublejump.png");
        _player_attack = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Cyborg_attack3.png");
        _player_hurt = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Cyborg_hurt.png");
        _player_death = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Cyborg_death.png");

    }
}