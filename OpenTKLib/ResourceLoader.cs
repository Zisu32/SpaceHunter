using System.Reflection;
using Zenseless.OpenTK;
using Zenseless.Resources;

namespace OpenTKLib;

public static class ResourceLoader
{
    public static Texture2D LoadTexture(string name)
    {
        using Stream? stream = LoadAssemblyResource(name);

        if (stream == null)
        {
            throw new ArgumentException("Resource does not exist");
        }

        return stream.LoadTexture();
    }

    public static Stream? LoadAssemblyResource(string path)
    {
        Assembly? assembly = Assembly.GetEntryAssembly();

        if (assembly == null || assembly.GetName().Name == "OpenTKLib")
        {
            throw new ApplicationException("something is wrong with the assemblies");
        }

        return assembly.GetManifestResourceStream(path);
    }
}