using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

public class EscenaPC : GameWindow
{
    private int shader;
    private PCEscritorio pc = null!;
    private Monitor monitor = null!;
    private Teclado teclado = null!;
    
    public EscenaPC() : base(GameWindowSettings.Default, 
        new NativeWindowSettings() { Size = new Vector2i(1200, 800), Title = "PC" })
    {
    }

    protected override void OnLoad()
    {
        GL.ClearColor(0.1f, 0.1f, 0.2f, 1.0f);
        GL.Enable(EnableCap.DepthTest);

        CrearShader();
        
        pc = new PCEscritorio();
        monitor = new Monitor();
        teclado = new Teclado();
        
        pc.Inicializar();
        monitor.Inicializar();
        teclado.Inicializar();
    }

    private void CrearShader()
    {
        string vertexShader = @"
            #version 330 core
            layout (location = 0) in vec3 pos;
            uniform mat4 mvp;
            void main() {
                gl_Position = mvp * vec4(pos, 1.0);
            }";

        string fragmentShader = @"
            #version 330 core
            uniform vec3 color;
            out vec4 fragColor;
            void main() {
                fragColor = vec4(color, 1.0);
            }";

        int vs = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vs, vertexShader);
        GL.CompileShader(vs);

        int fs = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fs, fragmentShader);
        GL.CompileShader(fs);

        shader = GL.CreateProgram();
        GL.AttachShader(shader, vs);
        GL.AttachShader(shader, fs);
        GL.LinkProgram(shader);

        GL.DeleteShader(vs);
        GL.DeleteShader(fs);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        GL.UseProgram(shader);

        Matrix4 view = Matrix4.LookAt(new Vector3(8, 8, 12), new Vector3(0, 1.5f, 0), Vector3.UnitY);
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(45), (float)Size.X / Size.Y, 0.1f, 100f);

        pc.Renderizar(shader, view, projection);
        monitor.Renderizar(shader, view, projection);
        teclado.Renderizar(shader, view, projection);

        SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        if (KeyboardState.IsKeyDown(Keys.Escape))
            Close();
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, Size.X, Size.Y);
    }

}