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
    private readonly TextureManager _textureManager;
    private Box2 _position;
    private EndbossState _currentState = EndbossState.idle_l;
    private float _animationTimer;
    private uint _currentFrame;
    private uint _columns;
    private uint _rows;
    private int _health = ConstantBalancingValues.EndbossHealth;
    private double _invincibleTimer = 0;
    private const double InvincibleDuration = 2;
    private double _attackCooldown = 0;
    private double _hurtTimer = 0;
    private const double HurtDuration = 0.5;
    private const float AttackRange = 2.5f;
    private const float ShootRange = 7f;
    private const float FollowSpeed = 5f;

    private double _animationLockTimer = 0;
    private bool IsLocked => _animationLockTimer > 0;
    private const float FrameDuration = 0.15f;

    // Laser
    public readonly List<(Box2 box, bool movingLeft, float startX)> LaserBeams = new();
    private float _laserCooldown = 5f;
    private const float LaserCooldownTime = 5f;
    private const float LaserSpeed = 10f;

    private bool _movingLeft = true;
    public Box2 Bounds => _position;

    public event EventHandler? OnDeath;
    public Endboss(GameState state, Box2 position, TextureManager textureManager)
    {
        _state = state;
        _position = position;
        _textureManager = textureManager;
        SetAnimationState(EndbossState.idle_l);
    }
    public int Health
    {
        get => _health;
        set
        {
            if (_invincibleTimer > 0 || _health <= 0) return; // ignore damage if invincible or dead

            if (value < _health) // took damage
            {
                _hurtTimer = HurtDuration;
                _invincibleTimer = InvincibleDuration;
                SetAnimationState(_position.Center.X > _state.PlayerBox.Center.X ? EndbossState.hurt_l : EndbossState.hurt_r);
            }

            _health = value;

            Console.WriteLine($"Endboss new health: {_health}");

            if (_health <= 0 && _currentState != EndbossState.death)
            {
                SetAnimationState(EndbossState.death);
                _hurtTimer = 0;
            }
        }
    }

    private void SetAnimationState(EndbossState state)
    {
        _currentState = state;
        _currentFrame = 0;
        _animationTimer = 0;

        switch (state)
        {
            case EndbossState.idle_r:
            case EndbossState.idle_l:
                _columns = 2;
                _rows = 1;
                break;
            case EndbossState.walk_r:
            case EndbossState.walk_l:
                _columns = 6;
                _rows = 1;
                break;
            case EndbossState.attack_r:
            case EndbossState.attack_l:
                _columns = 3;
                _rows = 1;
                break;
            case EndbossState.hurt_r:
            case EndbossState.hurt_l:
                _columns = 4;
                _rows = 1;
                break;
            case EndbossState.shoot_r:
            case EndbossState.shoot_l:
                _columns = 7;
                _rows = 1;
                break;
            case EndbossState.death:
                _columns = 5;
                _rows = 1;
                break;
        }

        // Lock duration for all animations to allow them to finish
        _animationLockTimer = FrameDuration * _columns;
    }

    private void Animate(float deltaTime)
    {
        _animationTimer += deltaTime;

        if (_animationTimer > FrameDuration)
        {
            _animationTimer = 0f;

            if (_currentState == EndbossState.death)
            {
                // Play death animation only once
                if (_currentFrame < _columns - 1)
                {
                    _currentFrame++;
                }
                // Don't reset to 0 — stay on last frame
            }
            else
            {
                // Loop other animations
                _currentFrame = (_currentFrame + 1) % _columns;
            }
        }
    }


    private void ShootLaser(bool toLeft)
    {
        var laser = new Box2(
            toLeft ? _position.Min.X - 0.5f : _position.Max.X,
            _position.Center.Y + 1.1f,
            toLeft ? _position.Min.X : _position.Max.X + 0.5f,
            _position.Center.Y + 1.2f
        );

        LaserBeams.Add((laser, toLeft, _position.Center.X));
        Console.WriteLine("Endboss Fired laser!");
    }

    private void UpdateLasers(float deltaTime)
    {
        for (int i = LaserBeams.Count - 1; i >= 0; i--)
        {
            var (box, movingLeft, startX) = LaserBeams[i];
            float speed = LaserSpeed * deltaTime;
            float direction = movingLeft ? -1f : 1f;

            var moved = new Box2(
                box.Min.X + speed * direction,
                box.Min.Y,
                box.Max.X + speed * direction,
                box.Max.Y
            );

            if (Math.Abs(moved.Center.X - startX) > 30f)
            {
                LaserBeams.RemoveAt(i);
            }
            else
            {
                LaserBeams[i] = (moved, movingLeft, startX);
            }
        }
    }


    public void Update(float deltaTime, Box2 playerBox)
    {
        if (_invincibleTimer > 0)
        {
            _invincibleTimer -= deltaTime;
        }
        
        if (_currentState == EndbossState.death)
        {
            if (!IsLocked)
            {
                OnDeath?.Invoke(this, EventArgs.Empty);
            }

            Animate(deltaTime);
            _animationLockTimer -= deltaTime;
            return;
        }

        if (_hurtTimer > 0)
        {
            _hurtTimer -= deltaTime;
            if (_hurtTimer <= 0)
            {
                var walkState = _movingLeft ? EndbossState.walk_l : EndbossState.walk_r;
                SetAnimationState(walkState);
            }

            Animate(deltaTime);
            return;
        }

        if (IsLocked)
        {
            _animationLockTimer -= deltaTime;
            Animate(deltaTime);
            return;
        }

        _attackCooldown -= deltaTime;
        _laserCooldown -= deltaTime;

        float playerX = playerBox.Center.X;
        float bossX = _position.Center.X;
        float distance = Math.Abs(playerX - bossX);
        bool playerLeft = playerX < bossX;
        bool performedAction = false;

        if (distance <= AttackRange && _attackCooldown <= 0f)
        {
            SetAnimationState(playerLeft ? EndbossState.attack_l : EndbossState.attack_r);
            _attackCooldown = 3f;
            _state.PlayerHealth -= 20;
            _state.IsPlayerHurt = true;
            _state.PlayerHurtTimer = 1.0;
            Console.WriteLine("Endboss Melee attack!");
            performedAction = true;
        }
        else if (distance <= ShootRange && _laserCooldown <= 0f)
        {
            SetAnimationState(playerLeft ? EndbossState.shoot_l : EndbossState.shoot_r);
            ShootLaser(playerLeft);
            _laserCooldown = LaserCooldownTime;
            Console.WriteLine("Endboss Shooting laser!");
            performedAction = true;
        }

        if (!performedAction && distance > AttackRange)
        {
            float direction = Math.Sign(playerX - bossX);
            _movingLeft = direction < 0;

            float move = direction * FollowSpeed * deltaTime;
            Vector2 movement = new Vector2(move, 0);
            _position = new Box2(_position.Min + movement, _position.Max + movement);

            var walkState = _movingLeft ? EndbossState.walk_l : EndbossState.walk_r;
            if (_currentState != walkState)
            {
                SetAnimationState(walkState);
            }
        }

        Animate(deltaTime);
        UpdateLasers(deltaTime);
    }
    private void DrawHealthBarAboveHead(Box2 bossPosition, int currentHealth, int maxHealth)
    {
        float barWidth = bossPosition.Size.X * 0.5f;
        float barHeight = 0.1f;
        
        float barX = bossPosition.Center.X - (barWidth / 2f); // Center the health bar above the boss
        float barY = bossPosition.Max.Y + 0.2f; // vertical offset above head

        Vector2 barMin = new Vector2(barX, barY);
        Vector2 barMax = new Vector2(barX + barWidth, barY + barHeight);
        Box2 backgroundBar = new Box2(barMin, barMax);

        // Draw background
        DebugDrawHelper.DrawRectangle(backgroundBar, Color.Black);

        // Calculate health percent
        float healthPercent = Math.Clamp((float)currentHealth / maxHealth, 0f, 1f);
        float fillWidth = barWidth * healthPercent;

        Vector2 fillMax = new Vector2(barMin.X + fillWidth, barMax.Y);
        Box2 filledBar = new Box2(barMin, fillMax);

        // Color interpolation from green to red
        int r = (int)(255 * (1 - healthPercent));
        int g = (int)(255 * healthPercent);
        int b = 0;

        Color healthColor = Color.FromArgb(r, g, b);

        // Draw filled bar
        DebugDrawHelper.DrawRectangle(filledBar, healthColor);
    }



    public void Draw(FrameEventArgs args)
    {
        Texture2D tex = _currentState switch
        {
            EndbossState.idle_l => _textureManager._endbossIdleL,
            EndbossState.idle_r => _textureManager._endbossIdleR,
            EndbossState.walk_l => _textureManager._endbossWalkL,
            EndbossState.walk_r => _textureManager._endbossWalkR,
            EndbossState.attack_l => _textureManager._endbossAttackL,
            EndbossState.attack_r => _textureManager._endbossAttackR,
            EndbossState.shoot_l => _textureManager._endbossShootL,
            EndbossState.shoot_r => _textureManager._endbossShootR,
            EndbossState.hurt_l => _textureManager._endbossHurtL,
            EndbossState.hurt_r => _textureManager._endbossHurtR,
            EndbossState.death => _textureManager._endbossDeath,
            _ => throw new Exception("Invalid endboss state")
        };

        TextureHelper.DrawSprite(_position, tex.Handle, _currentFrame, _columns, _rows);
        DebugDrawHelper.DrawRectangle(_position, Color.Cyan);
        DrawHealthBarAboveHead(_position, _health, ConstantBalancingValues.EndbossHealth);
        
        foreach (var (box, _, _) in LaserBeams)
        {
            DebugDrawHelper.DrawRectangle(box, Color.Cyan);
        }
    }
}
