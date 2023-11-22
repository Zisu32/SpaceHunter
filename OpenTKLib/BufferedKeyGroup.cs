using System.Collections.ObjectModel;
using System.Data;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace OpenTKLib;

public class BufferedKeyGroup
{
    private readonly IReadOnlyList<Keys> _keysList;

    public BufferedKeyGroup(IList<Keys> keysList)
    {
        _keysList = new ReadOnlyCollection<Keys>(keysList);
    }

    public Keys? LastPressed
    {
        get; set;
    }

    internal void Update(KeyboardKeyEventArgs keyEvent)
    {
        if (_keysList.Contains(keyEvent.Key))
        {
            LastPressed = keyEvent.Key;
        }
    }
}