using System.Collections.ObjectModel;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTKLib;

public class KeyGroup
{
    private readonly IReadOnlyList<Keys> _keysList;
    private readonly List<Keys> _ignoredKeys = new();

    public KeyGroup(IList<Keys> keysList)
    {
        _keysList = new ReadOnlyCollection<Keys>(keysList);
    }

    public HashSet<Keys> PressedKeys { get; } = new();

    public void RemovePressed(Keys key)
    {
        PressedKeys.Remove(key);
        _ignoredKeys.Add(key);
    }

    internal void KeyDown(KeyboardKeyEventArgs keyEvent)
    {
        if (_keysList.Contains(keyEvent.Key)
            // the key should not be added to pressed if it has been manually removed
            && !_ignoredKeys.Contains(keyEvent.Key))
        {
            PressedKeys.Add(keyEvent.Key);
            Console.WriteLine($"KeyAdd: {keyEvent.Key}");
        }
    }

    internal void KeyUp(KeyboardKeyEventArgs keyEvent)
    {
        if (_keysList.Contains(keyEvent.Key))
        {
            PressedKeys.Remove(keyEvent.Key);

            // after a key has been released it is cleared form ignored
            _ignoredKeys.Remove(keyEvent.Key);
            Console.WriteLine($"KeyRemove: {keyEvent.Key}");
        }
    }
}