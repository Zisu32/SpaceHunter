using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTKLib;

namespace SpaceHunter;

public class KeyMapper
{
    private readonly Keyboard _keyboard;

    private Dictionary<KeyFunctions, Keys> _mappingDictionary = new();

    public KeyMapper(Keyboard keyboard)
    {
        _keyboard = keyboard;
    }

    public void RegisterKey(Keys key, KeyFunctions function)
    {
        _mappingDictionary.Add(function, key);
    }

    public bool IsPressed(KeyFunctions function)
    {
        return _keyboard.CheckKeyDown(_mappingDictionary[function]);
    }
}