using System.Diagnostics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTKLib;

public class Keyboard
{
    private readonly GameWindow _window;
    private readonly OpenTKManager _openTkManager;
    private readonly List<KeyGroup> _keyGroups = new();

    public Keyboard(OpenTKManager openTkManager, GameWindow window)
    {
        _openTkManager = openTkManager;
        _window = window;
    }

    public void AddKeyGroup(KeyGroup keyGroup)
    {
        _keyGroups.Add(keyGroup);
    }

    internal void KeyDown(KeyboardKeyEventArgs keyEvent)
    {
        _keyGroups.ForEach(keyGroup => keyGroup.KeyDown(keyEvent));
    }

    internal void KeyUp(KeyboardKeyEventArgs keyEvent)
    {
        _keyGroups.ForEach(keyGroup => keyGroup.KeyUp(keyEvent));
    }

    public bool CheckKeyDown(Keys key)
    {
        return _window.IsKeyDown(key);
    }
}