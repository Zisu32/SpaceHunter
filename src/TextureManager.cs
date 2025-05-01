using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTKLib;
using Zenseless.OpenTK;
using OpenTK.Graphics.OpenGL;
using SpaceHunter.Models;


namespace SpaceHunter;

public class TextureManager
{
    // TODO, find better way to handle loaded Textures
    private Texture2D _background;
    private Texture2D _menuscreen;
    private Texture2D _healthbar;
    public Texture2D _portalTexture;
    private Texture2D _player_idle_r;
    private Texture2D _player_idle_l;
    private Texture2D _player_run_r;
    private Texture2D _player_run_l;
    private Texture2D _player_jump_r;
    private Texture2D _player_jump_l;
    private Texture2D _player_attack_r;
    private Texture2D _player_attack_l;
    private Texture2D _player_death;

    private Texture2D _blueEnemy;
    public Texture2D _flyingEnemy;
    
    static float clockCounter = 0;
    static uint spriteId = 0;
    static uint columns = 0;
    static uint rows = 1;
    
    private static readonly Vector4 redTint = new Vector4(1f, 0f, 0f, 1f);
    private static readonly Box2 DefaultBox = new Box2(0f, 0f, 1f, 1f);
    public static readonly Box2 PortalRectangle = new Box2(64f, 0f, 68f, 6f);
    public static readonly Box2 FlyingEnemyRectangle = new Box2(5f, 0f, 7f, 6f);

    // TODO, the aspect ratios of the background are different
    // bg 1 is 16:10 aspect ratio
    public static readonly Box2 BackgroundRectangle = new Box2(0f, 0f, 16*4.5f, 10*1.5f);
    public static readonly Box2 MenuRectangle = new Box2(0f, 0f, 4 * 3f, 4 * 3f);
    
    public void DrawBackground()
    {
        TextureHelper.DrawRectangularTexture(BackgroundRectangle, _background.Handle);
    }
    
    public void DrawMenuScreen()
    {
        TextureHelper.DrawRectangularTexture(MenuRectangle, _menuscreen.Handle);
    }

    public void DrawEnemy(Box2 position)
    {
        TextureHelper.DrawRectangularTexture(position, _blueEnemy.Handle);
    }

    public void DrawPlayerTex(Box2 position, PlayerState playerState, FrameEventArgs obj, bool isHurt)
    {
        Texture2D texture2D;
        bool playOnce = false;
        switch (playerState)
        {
            case PlayerState.idle_r:
                texture2D = _player_idle_r;
                columns = 4;
                break;
            case PlayerState.idle_l:
                texture2D = _player_idle_l;
                columns = 4;
                break;
            case PlayerState.run_r:
                texture2D = _player_run_r;
                columns = 6;
                break;
            case PlayerState.run_l:
                texture2D = _player_run_l;
                columns = 6;
                break;
            case PlayerState.jump_r:
                texture2D = _player_jump_r;
                columns = 6;
                break;
            case PlayerState.jump_l:
                texture2D = _player_jump_l;
                columns = 6;
                break;
            case PlayerState.attack_r:
                texture2D = _player_attack_r;
                columns = 8;
                break;
            case PlayerState.attack_l:
                texture2D = _player_attack_l;
                columns = 8;
                break;
            case PlayerState.death:
                texture2D = _player_death;
                playOnce = true;
                columns = 6;
                break;

            default:
                throw new InvalidOperationException("unknown player state:" + playerState);
        }

        // TODO, reset animation progress on start of new animation 
        // Attack looks weird without this
        // TODO, play Death anim (and others maybe) only once
        // use playOnce var

        // Zeitberechnung für Animation der Sprites
        float clock = (float)(obj.Time);
        clockCounter += clock;
        if (clockCounter > 0.25)
        {
            // Console.WriteLine("Col: " + columns);
            spriteId = (spriteId + 1) % columns;
            clockCounter = 0;
        }

        if (isHurt)
        {
            TextureHelper.DrawSprite(position, texture2D.Handle, spriteId, columns, rows, redTint);
        }
        else
        {
            TextureHelper.DrawSprite(position, texture2D.Handle, spriteId, columns, rows);
        }
    }


    public void Initialize()
    {
        TextureHelper.InitalizeOpenGLTextures();

        // Textures can only be loaded when a window is already being displayed (for some reason)
        _background = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Backgrounds.1-new.jpg");
        _menuscreen = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Screen.MainMenu.png");
        _portalTexture = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Portal.Portal-new.png");
        _player_idle_r = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.MainChar.Cyborg_idle_r_new.png");
        _player_idle_l = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.MainChar.Cyborg_idle_l_new.png");
        _player_run_r = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.MainChar.Cyborg_run_r_new.png");
        _player_run_l = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.MainChar.Cyborg_run_l_new.png");
        _player_jump_r =
            TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.MainChar.Cyborg_doublejump_r_new.png");
        _player_jump_l =
            TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.MainChar.Cyborg_doublejump_l_new.png");
        _player_attack_r =
            TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.MainChar.Cyborg_attack3_r_new.png");
        _player_attack_l =
            TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.MainChar.Cyborg_attack3_l_new.png");
        _player_death = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.MainChar.Cyborg_death_r_new.png");
        _blueEnemy = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Sprites.Enemies.blueEnemy.Sprite_test.png");
        _flyingEnemy = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.flyingEnemy.FlyingEnemy.png");
    }
}