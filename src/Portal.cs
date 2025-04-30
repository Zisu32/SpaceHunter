using OpenTK.Mathematics;
using Zenseless.OpenTK;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

public class Portal
{
    private readonly Texture2D _texture;
    private readonly Box2 _position;
    private float _animationTimer;
    private uint _currentFrame;
    private readonly uint _frameCount;
    private readonly uint _columns;
    private readonly uint _rows;
    public Box2 Bounds => _position;


    public bool IsVisible { get; set; }

    public Portal(Box2 position, Texture2D texture, uint columns = 8, uint rows = 1)
    {
        _texture = texture;
        _position = position;
        _frameCount = columns * rows;
        _columns = columns;
        _rows = rows;
        _animationTimer = 0f;
        _currentFrame = 0;
        IsVisible = false;
    }

    public void Update(float deltaTime, IReadOnlyCollection<Enemy> enemies, Box2 playerBox)
    {
        // Update visibility
        IsVisible = !enemies.Any();
        Console.WriteLine($"Portal visible: {IsVisible}");

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
            PlayerEntered = true;
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