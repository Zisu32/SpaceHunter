using OpenTK.Windowing.Common;

namespace OpenTKLib;

public interface IDrawComponent
{
    public Task Draw(FrameEventArgs obj);
    void Initialize();
    public Camera Camera { get; set; }
}