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

    static float clockCounter = 0;
    static uint spriteId = 0;
    static uint columns = 0;
    static uint rows = 1;
    
    private static readonly Box2 DefaultBox = new Box2(0f, 0f, 1f, 1f);
    public static readonly Box2 BackgroundRectangle = new Box2(0f, 0f, 20f, 20f);

    public void DrawBackground()
    {
        TextureHelper.DrawRectangularTexture(BackgroundRectangle, _background.Handle);
    }

    public void DrawPlayerTex(Box2 position, PlayerState playerState, FrameEventArgs obj)
    {
        Texture2D texture2D = null;
        switch (playerState)
        {
            case PlayerState.idle:
                texture2D = _player_idle;
                columns = 4;
                break;
            case PlayerState.run:
                texture2D = _player_run;
                columns = 6;
                break;
            case PlayerState.jump:
                texture2D = _player_jump;
                columns = 6;
                break;
            case PlayerState.attack:
                texture2D= _player_attack;
                columns = 8;
                break;
            case PlayerState.hurt:
                texture2D=_player_hurt;
                columns = 2;
                break;
            case PlayerState.death:
                texture2D = _player_death;
                columns = 6;
                break;
        }
        
        // Zeitberechnung für Animation der Sprites
        float clock = (float)(obj.Time);
        clockCounter += clock;
        // Console.WriteLine("clockCounter: " + clockCounter);
        if (clockCounter > 0.25)
        {
            // Console.WriteLine("Col: " + columns);
            spriteId = (spriteId + 1) % columns;
            clockCounter = 0;
        }
        // Console.WriteLine("spriteID: " + spriteId);
        
        TextureHelper.DrawSprite(position, texture2D.Handle, spriteId, columns, rows);
        
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