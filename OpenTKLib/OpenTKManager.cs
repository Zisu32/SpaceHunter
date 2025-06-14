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
    private readonly double _targetFrametime;
    private double _timeSinceLastGameUpdate = 0;

    public OpenTKManager(IDrawComponent drawComponent, int framerate)
    {
        _drawComponent = drawComponent;
        _targetFrametime = 1.0 / framerate;
        _drawComponent.Camera = this.Camera;
        _window = new GameWindow(GameWindowSettings.Default, new NativeWindowSettings
        {
            Profile = ContextProfile.Compatability, Flags = ContextFlags.Default
        })
        {
            VSync = VSyncMode.Off
        };

        // window resize event
        _window.Resize += resizeArgs =>
        {
            GL.Viewport(0, 0, resizeArgs.Width, resizeArgs.Height);
            ClearWindow();
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

    // TODO, can probably be inlined
    private static void ClearWindow()
    {
        GL.Clear(ClearBufferMask.ColorBufferBit);
    }

    public Camera Camera { get; } = new Camera();

    public Keyboard Keyboard { get; }

    public double GameUpdateDelay { get; set; }

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
            ClearWindow();
        }

        _drawComponent.Draw(frameArgs);

        // Swap displayed buffer
        _window.SwapBuffers();


        // frame limiter

        double sleepTime = _targetFrametime - frameArgs.Time;

        if (sleepTime > 0)
        {
            Thread.Sleep((int)(sleepTime * 1000)); // Convert seconds to milliseconds
        }
        else
        {
            Console.WriteLine("frame took too long");
        }
    }

    private void Update(FrameEventArgs frameArgs)
    {
        _timeSinceLastGameUpdate += frameArgs.Time;

        if (!(_timeSinceLastGameUpdate > GameUpdateDelay))
        {
            return;
        }

        // Console.WriteLine($"Running gameCycle after {_timeSinceLastGameUpdate}s");
        _timeSinceLastGameUpdate = 0;

        GameStateUpdateEvent?.Invoke(this, frameArgs);
    }
}