using OpenTK.Mathematics;
using Zenseless.OpenTK;
using OpenTKLib;
using SpaceHunter.Models;

namespace SpaceHunter;

public class FlyingEnemy
{
    private GameState _state;
    private readonly Texture2D _texture;
    private readonly Box2 _position;
    private float _animationTimer;
    private uint _currentFrame;
    private readonly uint _frameCount;
    private readonly uint _columns;
    private readonly uint _rows;
    private int _health = ConstantBalancingValues.EnemyHealth;
    public Box2 Bounds => _position;
    
    //ENemy Health
    public int Health
    {
        get => _health;
        set
        {
            _health = value;
            if (_health <= 0)
            {
                OnDeath?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    // Enemy Death
    public event EventHandler? OnDeath;

    public FlyingEnemy(Box2 position, Texture2D texture, uint columns = 5, uint rows = 1)
    {
        _texture = texture;
        _position = position;
        _frameCount = columns * rows;
        _columns = columns;
        _rows = rows;
        _animationTimer = 0f;
        _currentFrame = 0;
    }
    
    public void Update(float deltaTime, Box2 playerBox)
    {
        // Animate
        _animationTimer += deltaTime;
        if (_animationTimer > 0.1f)
        {
            _currentFrame = (_currentFrame + 1) % _frameCount;
            _animationTimer = 0f;
        }
    }
    public void DrawFlyingEnemy()
    {
        TextureHelper.DrawSprite(_position, _texture.Handle, _currentFrame, _columns, _rows);
    }
}