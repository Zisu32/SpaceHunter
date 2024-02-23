using OpenTK.Mathematics;
using Zenseless.OpenTK;

namespace OpenTKLib;

// TODO aspect ratio functionality
public class Camera
{
    private Vector2 _center = Vector2.Zero;
    private float _rotation;
    private float _scale = 1;

    public Matrix4 CameraMatrix { get; private set; } = Matrix4.Identity; // Einheits-Matrix


    public Vector2 Center
    {
        get => _center;
        set
        {
            _center = value;
            UpdateMatrix();
        }
    }

    public float Rotation
    {
        get => _rotation;
        set
        {
            _rotation = value;
            UpdateMatrix();
        }
    }

    public float Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            UpdateMatrix();
        }
    }

    // 1f / Scale is our standard Scale calculation
    // 1f is the width of one screen half thus multiply it by 2
    public float ScreenWidth => 1f / (1f / Scale) * 2;

    private void UpdateMatrix()
    {
        Matrix4 translation = Transformation2d.Translate(_center);
        Matrix4 rotation = Transformation2d.Rotation(MathHelper.DegreesToRadians(_rotation));
        Matrix4 scale = Transformation2d.Scale(1f / Scale);

        // the default translate moves the camera, so that the bottom left Corner is 0,0
        Matrix4 defaultTranslate = Transformation2d.Translate(-1 * Scale, -1 * Scale);

        CameraMatrix = Transformation2d.Combine(defaultTranslate, translation, scale, rotation);
    }
}