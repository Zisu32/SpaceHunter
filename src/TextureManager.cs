using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTKLib;
using Zenseless.OpenTK;
using OpenTK.Graphics.OpenGL;
using SpaceHunter.Models;


namespace SpaceHunter;

public class TextureManager
{
    // In TextureManager.cs
    private List<Texture2D> _transitionTextures = new();

    // TODO, find better way to handle loaded Textures
    private Texture2D _background;
    private Texture2D _background2;
    private Texture2D _background_transition;
    private Texture2D _menuscreen;
    private Texture2D _victoryscreen;
    private Texture2D _gameoverscreen;
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
    public Texture2D _heart;

    public Texture2D _staticEnemy;
    public Texture2D _flyingEnemy;

    public Texture2D _endbossIdleL;
    public Texture2D _endbossIdleR;
    public Texture2D _endbossWalkL;
    public Texture2D _endbossWalkR;
    public Texture2D _endbossAttackL;
    public Texture2D _endbossAttackR;
    public Texture2D _endbossShootL;
    public Texture2D _endbossShootR;
    public Texture2D _endbossHurtL;    
    public Texture2D _endbossHurtR;    
    public Texture2D _endbossDeath;
    
    static float clockCounter = 0;
    static uint spriteId = 0;
    static uint columns = 0;
    static uint rows = 1;
    
    private static readonly Vector4 redTint = new Vector4(1f, 0f, 0f, 1f);
    public static readonly Box2 PortalRectangle = new Box2(64f, 0f, 66f, 6f);
    public static readonly Box2 EndbossRectangle = new Box2(45f, 0f, 48f, 7f);
    public static readonly Box2 FlyingEnemyRectangle = new Box2(3f, 0f, 4f, 4f);
    public static readonly Box2 StaticEnemyRectangle = new Box2(3f, 0f, 4f, 3f);
    public static readonly Box2 BackgroundRectangle = new Box2(0f, 0f, 16*4.5f, 10*1.5f);
    public static readonly Box2 MenuRectangle = new Box2(0f, 0f, 4 * 3f, 4 * 3f);
    public static readonly Box2 EndScreenRectangle = new Box2(0f, 0f, 12f, 12f);
    public static readonly Box2 VictoryScreenRectangle = new Box2(0f, 0f, 12f, 12f);

    public void DrawBackground(int CurrentLevel)
    {
        if (CurrentLevel == 1)
        {
            TextureHelper.DrawRectangularTexture(BackgroundRectangle, _background.Handle);
        }
        else
        {
            TextureHelper.DrawRectangularTexture(BackgroundRectangle, _background2.Handle);
        }
    }

    public void DrawLevelTransition(double transitionTimer)
    {
        if (_transitionTextures.Count == 0) return;

        // Berechne den Frame basierend auf der verbleibenden Zeit (5 Sekunden gesamt)
        double progress = 1 - (transitionTimer / 5.0);
        int frameIndex = (int)(progress * 120); // 0-120 (da 121 Frames)
        frameIndex = Math.Clamp(frameIndex, 0, _transitionTextures.Count - 1);

        TextureHelper.DrawRectangularTexture(BackgroundRectangle, _transitionTextures[frameIndex].Handle);
    }

    public void DrawMenuScreen()
    {
        TextureHelper.DrawRectangularTexture(MenuRectangle, _menuscreen.Handle);
    }
    
    public void DrawGameOver()
    {
        TextureHelper.DrawRectangularTexture(EndScreenRectangle, _gameoverscreen.Handle);
    }
    public void DrawVictoryScreen()
    {
        TextureHelper.DrawRectangularTexture(VictoryScreenRectangle, _victoryscreen.Handle);
    }

    public void DrawEnemy(Box2 position)
    {
        TextureHelper.DrawRectangularTexture(position, _staticEnemy.Handle);
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
        if (clockCounter > 0.15)
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
        _background = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Backgrounds.1.jpg");
        _background2 = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Backgrounds.2.png");
        _background_transition = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.VideoFrames.001.png");
        _menuscreen = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Screen.MainMenu.png");
        _victoryscreen = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Screen.VictoryScreen.png");
        _gameoverscreen = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Screen.GameOver.png");
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
        _staticEnemy = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.staticEnemy.staticEnemy.png");        
        _flyingEnemy = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.flyingEnemy.FlyingEnemy.png");
        _endbossIdleL = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Attack-l-resized.png");
        _endbossIdleR = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Idol-r-resized.png");
        _endbossWalkL = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Walk-l-resized.png");
        _endbossWalkR = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Walk-r-resized.png");
        _endbossAttackL = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Attack-l-resized.png");
        _endbossAttackR = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Attack-r-resized.png");
        _endbossShootL = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Shot-l-resized.png");
        _endbossShootR = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Shot-r-resized.png");
        _endbossHurtL = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Hurt-l-resized.png");
        _endbossHurtR = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Hurt-r-resized.png");
        _endbossDeath = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Enemy.endboss.Infantryman.Death-l-resized.png");
        _heart = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Heart.heart.png");

        // Initialize all VideoFrames
        _transitionTextures.Clear();
        for (int i = 1; i <= 121; i++)
        {
            Texture2D frame = TextureHelper.LoadNonFilteringTexture($"SpaceHunter.Assets.VideoFrames.{i.ToString("D3")}.png");
            _transitionTextures.Add(frame);
        }

    }
}