using OpenTK.Graphics.OpenGL;

namespace SpaceHunter;

public class Healthbar
{
        public void DrawHealthBar(float health, float maxHealth)
    {
        if (health <= 0) return;

        float barWidth = 100f;  
        float barHeight = 15f;  
        float healthPercentage = health / maxHealth;

        float x = 10f; 
        float y = 10f;

        // Save previous OpenGL state
        GL.PushAttrib(AttribMask.AllAttribBits);

        // Set up 2D projection
        GL.MatrixMode(MatrixMode.Projection);
        GL.PushMatrix();
        GL.LoadIdentity();
        GL.Ortho(-10, 640, 360, 0, -1, 1);

        GL.MatrixMode(MatrixMode.Modelview);
        GL.PushMatrix();
        GL.LoadIdentity();

        // Disable unwanted states
        GL.Disable(EnableCap.DepthTest);
        GL.Disable(EnableCap.Lighting);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

        //Draw Background (black)
        GL.Color3(0.0f, 0.0f, 0.0f);
        GL.Begin(PrimitiveType.Quads);
        GL.Vertex2(x, y);
        GL.Vertex2(x + barWidth, y);
        GL.Vertex2(x + barWidth, y + barHeight);
        GL.Vertex2(x, y + barHeight);
        GL.End();

        //Dynamic Health Color (Green to Red)
        float green = healthPercentage;
        float red = 1.0f - healthPercentage;
        GL.Color3(red, green, 0.0f);

        //Draw Health Foreground
        GL.Begin(PrimitiveType.Quads);
        GL.Vertex2(x, y);
        GL.Vertex2(x + (barWidth * healthPercentage), y);
        GL.Vertex2(x + (barWidth * healthPercentage), y + barHeight);
        GL.Vertex2(x, y + barHeight);
        GL.End();

        //Draw Border (black)
        GL.Color3(0.0f, 0.0f, 0.0f);
        GL.LineWidth(3);
        GL.Begin(PrimitiveType.LineLoop);
        GL.Vertex2(x, y);
        GL.Vertex2(x + barWidth, y);
        GL.Vertex2(x + barWidth, y + barHeight);
        GL.Vertex2(x, y + barHeight);
        GL.End();

        GL.PopMatrix();
        GL.MatrixMode(MatrixMode.Projection);
        GL.PopMatrix();
        GL.MatrixMode(MatrixMode.Modelview);
        GL.PopAttrib();  // Restore OpenGL settings
    }
}