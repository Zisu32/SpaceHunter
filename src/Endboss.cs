using System.Drawing;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using Zenseless.OpenTK;
using OpenTKLib;
using SpaceHunter.Models;
using SpaceHunter.Models.Enums;

namespace SpaceHunter;

public class Endboss
{
    private readonly GameState _state;
    private readonly TextureManager _textures;
    private Box2 _position;

    private int _health;
    private double _invincibleTimer = 0;

    private EndbossState _currentState = EndbossState.idle_l;
    private int _frame = 0;
    private float _animationTimer = 0f;

    private const float FrameDuration = 0.15f;
    private const float FollowSpeed = 4f;
    private const float AttackRange = 1.5f;
    private const float ShootRange = 6f;
    private const float LaserSpeed = 5f;

    private float _meleeCooldownTimer = 0f;
    private const float MeleeAttackCooldown = 5f;

    public event EventHandler? OnDeath;
    public readonly List<(Box2 box, bool movingLeft, float startX)> LaserBeams = new();

    public Box2 Bounds => _position;
    private bool _isDying = false;
    private bool _isHurt = false;

    public Endboss(GameState state, Box2 startPos, TextureManager textures)
    {
        _state = state;
        _textures = textures;
        _position = startPos;
        _health = ConstantBalancingValues.EndbossHealth;
    }

    public int Health
    {
        get => _health;
        set
        {
            if (_invincibleTimer > 0 || _health <= 0) return;

            _health = value;
            _invincibleTimer = ConstantBalancingValues.InvincibleDuration;

            _isHurt = true;
            _frame = 0;
            _animationTimer = 0;

            if (_health <= 0)
            {
                _currentState = EndbossState.death;
                _frame = 0;
                _animationTimer = 0;
                _isDying = true;
            }
            else
            {
                _currentState = _currentState.ToString().EndsWith("_r") ? EndbossState.hurt_r : EndbossState.hurt_l;
            }
        }
    }

    private void ShootLaser(bool left)
    {
        var laser = new Box2(
            left ? _position.Min.X - 0.25f : _position.Max.X,
            _position.Center.Y + 1.15f,
            left ? _position.Min.X : _position.Max.X + 0.25f,
            _position.Center.Y + 1.2f
        );

        LaserBeams.Add((laser, left, _position.Center.X));
    }

    private void UpdateLasers(float deltaTime)
    {
        for (int i = LaserBeams.Count - 1; i >= 0; i--)
        {
            var (box, left, startX) = LaserBeams[i];
            float dir = left ? -1f : 1f;
            var moved = new Box2(
                box.Min.X + dir * LaserSpeed * deltaTime,
                box.Min.Y,
                box.Max.X + dir * LaserSpeed * deltaTime,
                box.Max.Y
            );

            if (Math.Abs(moved.Center.X - startX) > 30f)
            {
                LaserBeams.RemoveAt(i);
            }
            else
            {
                LaserBeams[i] = (moved, left, startX);
            }
        }
    }

    public void Update(float deltaTime, Box2 playerBox)
    {
        UpdateLasers(deltaTime);

        if (_invincibleTimer > 0)
            _invincibleTimer -= deltaTime;

        if (_meleeCooldownTimer > 0)
            _meleeCooldownTimer -= deltaTime;

        if (_isDying)
        {
            Animate(deltaTime);
            if (_frame >= GetFrameCount(_currentState) - 1)
                OnDeath?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (_isHurt)
        {
            Animate(deltaTime);
            if (_frame >= GetFrameCount(_currentState) - 1)
                _isHurt = false;
            return;
        }

        float dx = playerBox.Center.X - _position.Center.X;
        bool movingLeft = dx < 0;
        float distance = Math.Abs(dx);

        if (distance <= AttackRange)
        {
            _currentState = movingLeft ? EndbossState.attack_l : EndbossState.attack_r;

            if (_meleeCooldownTimer <= 0f)
            {
                _state.PlayerHealth -= ConstantBalancingValues.EndbossDamage;
                _meleeCooldownTimer = MeleeAttackCooldown;
            }
        }
        else if (distance <= ShootRange)
        {
            _currentState = movingLeft ? EndbossState.shoot_l : EndbossState.shoot_r;

            if (_frame == 0)
                ShootLaser(movingLeft);
        }
        else
        {
            _currentState = movingLeft ? EndbossState.walk_l : EndbossState.walk_r;
            float dir = Math.Sign(dx);
            var move = new Vector2(dir * FollowSpeed * deltaTime, 0);
            _position = new Box2(_position.Min + move, _position.Max + move);
        }

        Animate(deltaTime);
    }

    private void Animate(float deltaTime)
    {
        _animationTimer += deltaTime;
        if (_animationTimer >= FrameDuration)
        {
            _animationTimer = 0f;
            _frame++;

            int frameCount = GetFrameCount(_currentState);

            if (_frame >= frameCount)
                _frame = (_isDying || _isHurt) ? frameCount - 1 : 0;
        }
    }

    private int GetFrameCount(EndbossState state)
    {
        return state switch
        {
            EndbossState.idle_l or EndbossState.idle_r => 2,
            EndbossState.walk_l or EndbossState.walk_r => 6,
            EndbossState.shoot_l or EndbossState.shoot_r => 7,
            EndbossState.attack_l or EndbossState.attack_r => 3,
            EndbossState.hurt_l or EndbossState.hurt_r => 4,
            EndbossState.death => 5,
            _ => 1
        };
    }

    public void Draw(FrameEventArgs args)
    {
        Texture2D texture = _currentState switch
        {
            EndbossState.idle_l => _textures._endbossIdleL,
            EndbossState.idle_r => _textures._endbossIdleR,
            EndbossState.walk_l => _textures._endbossWalkL,
            EndbossState.walk_r => _textures._endbossWalkR,
            EndbossState.shoot_l => _textures._endbossShootL,
            EndbossState.shoot_r => _textures._endbossShootR,
            EndbossState.attack_l => _textures._endbossAttackL,
            EndbossState.attack_r => _textures._endbossAttackR,
            EndbossState.hurt_l => _textures._endbossHurtL,
            EndbossState.hurt_r => _textures._endbossHurtR,
            EndbossState.death => _textures._endbossDeath,
            _ => _textures._endbossIdleL
        };

        TextureHelper.DrawSprite(_position, texture.Handle, (uint)_frame, (uint)GetFrameCount(_currentState), 1);
        DrawHealthBar();

        foreach (var (box, _, _) in LaserBeams)
        {
            DebugDrawHelper.DrawRectangle(box, Color.Cyan);
        }

    }

    private void DrawHealthBar()
    {
        float width = _position.Size.X * 0.5f;
        float height = 0.05f;
        var barMin = new Vector2(_position.Center.X - width / 2f, _position.Max.Y + 0.2f);
        var barMax = new Vector2(barMin.X + width, barMin.Y + height);
        DebugDrawHelper.DrawRectangle(new Box2(barMin, barMax), Color.Black);

        float fill = Math.Clamp(_health / (float)ConstantBalancingValues.EndbossHealth, 0, 1);
        var fillMax = new Vector2(barMin.X + width * fill, barMax.Y);
        Color color = Color.FromArgb((int)(255 * (1 - fill)), (int)(255 * fill), 0);
        DebugDrawHelper.DrawRectangle(new Box2(barMin, fillMax), color);
    }
}