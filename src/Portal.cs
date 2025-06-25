using OpenTK.Mathematics;
using Zenseless.OpenTK;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

public class Portal
{
    private readonly GameState _state;
    private readonly Texture2D _texture;
    private readonly Box2 _position;
    private float _animationTimer;
    private uint _currentFrame;
    private readonly uint _frameCount;
    private readonly uint _columns;
    private readonly uint _rows;
    public Box2 Bounds => _position;
    public bool IsVisible { get; set; }

    public Portal(GameState state, Box2 position, Texture2D texture, uint columns = 8, uint rows = 1)
    {
        _state = state;
        _texture = texture;
        _position = position;
        _frameCount = columns * rows;
        _columns = columns;
        _rows = rows;
        _animationTimer = 0f;
        _currentFrame = 0;
        IsVisible = false;
    }


    public void Update(float deltaTime, IReadOnlyCollection<Enemy> enemies, IReadOnlyCollection<FlyingEnemy> flyingEnemies, Box2 playerBox)
    {
        IsVisible = !enemies.Any() && !flyingEnemies.Any() && _state.CurrentLevel == 1;
        // Console.WriteLine($"Portal visible: {IsVisible}");

        // Animate
        _animationTimer += deltaTime;
        if (_animationTimer > 0.1f)
        {
            _currentFrame = (_currentFrame + 1) % _frameCount;
            _animationTimer = 0f;
        }

        // Check player collision
        if (IsVisible && CollisionHandler.TwoBoxCollisionCheck(playerBox, Bounds))
        {
            Console.WriteLine("Enter Portal");
            _state.IsShowingLevelTransition = true;
            _state.PlayerBox = new Box2(0, 0, TextureSizes.PlayerSizeX, TextureSizes.PlayerSizeY); // Spieler zurücksetzen
            _state.LevelTransitionTimer = 5.0;
            PlayerEntered = true;
            //_state.NextLevel();
        }
        else
        {
            PlayerEntered = false;
        }
    }

    // Expose collision result
    public bool PlayerEntered { get; private set; }
    

    public void DrawPortal()
    {
        if (!IsVisible) return;
        TextureHelper.DrawSprite(_position, _texture.Handle, _currentFrame, _columns, _rows);
    }
}