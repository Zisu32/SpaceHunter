using OpenTK.Mathematics;
using OpenTKLib;
using Zenseless.OpenTK;

namespace SpaceHunter;

public class TextureManager
{
    private Texture2D _background;
    private Texture2D _player;
    private static readonly Box2 DefaultBox = new Box2(0f, 0f, 1f, 1f);
    public static readonly Box2 BackgroundRectangle = new Box2(0f, 0f, 20f, 20f);

    public void DrawBackground()
    {
        TextureHelper.DrawRectangularTexture(BackgroundRectangle, _background.Handle);
    }

    public void DrawPlayer(Box2 position)
    {
        TextureHelper.DrawRectangularTexture(position, _player.Handle);
    }

    public void Initialize()
    {
        TextureHelper.InitalizeOpenGLTextures();

        // Textures can only be loaded when a window is already being displayed (for some reason)
        _background = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.BG-1.jpg");
        _player = TextureHelper.LoadNonFilteringTexture("SpaceHunter.Assets.Character_Single.png");
    }
}