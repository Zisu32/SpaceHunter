using System.Diagnostics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTKLib;

public class Keyboard
{
    private readonly GameWindow _window;
    private readonly OpenTKManager _openTkManager;
    private readonly List<Keys> _keyList = new(); // TODO, unused
    private readonly List<BufferedKeyGroup> _keyGroups = new();

    public Keyboard(OpenTKManager openTkManager, GameWindow window)
    {
        _openTkManager = openTkManager;
        _window = window;
    }

    public void AddKeyGroup(BufferedKeyGroup keyGroup)
    {
        _keyGroups.Add(keyGroup);
    }

    // TODO unregister Method
    public void RegisterKey(Keys key)
    {
        _keyList.Add(key);
    }

    internal void KeyDown(KeyboardKeyEventArgs keyEvent)
    {
        if (_keyList.Contains(keyEvent.Key))
        {
            KeyDownEvent?.Invoke(_openTkManager, keyEvent);
        }
        
        _keyGroups.ForEach(keyGroup => keyGroup.Update(keyEvent));
    }

    public bool CheckKeyDown(Keys key)
    {
        return _window.IsKeyDown(key);
    }

    public event EventHandler<KeyboardKeyEventArgs> KeyDownEvent;
}