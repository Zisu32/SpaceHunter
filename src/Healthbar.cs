using OpenTK.Graphics.OpenGL;

namespace SpaceHunter;

public class Healthbar
{
    public void DrawHealthBar(float health, float maxHealth)
    {
        if (health <= 0) return;

        float barWidth = 100f;
        float barHeight = 15f;
        float healthPercentage = Math.Clamp(health / maxHealth, 0f, 1f);

        float x = 10f; // screen position
        float y = 10f;

        // Save OpenGL state
        GL.PushAttrib(AttribMask.AllAttribBits);

        // Setup 2D orthographic projection
        GL.MatrixMode(MatrixMode.Projection);
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.Ortho(0, 640, 360, 0, -1, 1); // match screen size

        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();
        GL.LoadIdentity();

        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.Texture2D);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        // Background (black)
        GL.Color4(0f, 0f, 0f, 1f);
        GL.Begin(PrimitiveType.Quads);
        GL.Vertex2(x, y);
        GL.Vertex2(x + barWidth, y);
        GL.Vertex2(x + barWidth, y + barHeight);
        GL.Vertex2(x, y + barHeight);
        GL.End();

        // Fill color (green → red)
        float r = 1f - healthPercentage;
        float g = healthPercentage;
        GL.Color4(r, g, 0f, 1f);

        GL.Begin(PrimitiveType.Quads);
        GL.Vertex2(x, y);
        GL.Vertex2(x + (barWidth * healthPercentage), y);
        GL.Vertex2(x + (barWidth * healthPercentage), y + barHeight);
        GL.Vertex2(x, y + barHeight);
        GL.End();

        // Border (black)
        GL.Color4(0f, 0f, 0f, 1f);
        GL.LineWidth(2f);
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex2(x, y);
        GL.Vertex2(x + barWidth, y);
        GL.Vertex2(x + barWidth, y + barHeight);
        GL.Vertex2(x, y + barHeight);
        GL.End();

        // Restore OpenGL state
        GL.PopMatrix(); // modelview
        GL.MatrixMode(MatrixMode.Projection);
        GL.PopMatrix();
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PopAttrib();
    }
}
