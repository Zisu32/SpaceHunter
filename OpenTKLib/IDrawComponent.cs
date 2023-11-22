using OpenTK.Windowing.Common;

namespace RWU_Snakes;

public interface IDrawComponent
{
    public Task Draw(FrameEventArgs obj);
    void Initialize();
}