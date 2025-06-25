using System.Drawing;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTKLib;

public class OpenTKManager
{
    private readonly GameWindow _window;
    private bool _windowRunning;
    private readonly IDrawComponent _drawComponent;

    public OpenTKManager(IDrawComponent drawComponent)
    {
        _drawComponent = drawComponent;
        
        _drawComponent.Camera = this.Camera;
        _window = new GameWindow(GameWindowSettings.Default, new NativeWindowSettings
        {
            Profile = ContextProfile.Compatability, Flags = ContextFlags.Default
        })
        {
            VSync = VSyncMode.On
        };

        // window resize event
        _window.Resize += resizeArgs =>
        {
            GL.Viewport(0, 0, resizeArgs.Width, resizeArgs.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit);
        };

        // close window on Escape   
        _window.KeyDown += keyDownArgs =>
        {
            if (Keys.Escape == keyDownArgs.Key) _window.Close();
        };

        // Event to update GameState
        _window.UpdateFrame += Update;

        // Event to draw on Screen
        // _window.RenderFrame += Draw;

        // KeyDown handler
        Keyboard = new Keyboard(this, _window);
        _window.KeyDown += Keyboard.KeyDown;
        _window.KeyUp += Keyboard.KeyUp;
    }

    public Camera Camera { get; } = new Camera();

    public Keyboard Keyboard { get; }


    public bool ClearScreenBeforeDraw { get; set; } = true;

    public void DisplayWindow()
    {
        if (_windowRunning)
        {
            return;
        }

        _windowRunning = true;

        _drawComponent.Initialize();
        _window.RenderFrame += Draw;

        _window.Run();
    }

    public event EventHandler<FrameEventArgs> GameStateUpdateEvent;

    private void Draw(FrameEventArgs frameArgs)
    {
        Matrix4 cameraCameraMatrix = Camera.CameraMatrix;
        GL.LoadMatrix(ref cameraCameraMatrix);

        if (ClearScreenBeforeDraw)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        _drawComponent.Draw(frameArgs);

        // Swap displayed buffer
        _window.SwapBuffers();
    }

    private void Update(FrameEventArgs frameArgs)
    {
        GameStateUpdateEvent?.Invoke(this, frameArgs);
    }
}