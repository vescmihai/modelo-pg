using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public abstract class Objeto3D
{
    protected int VAO, VBO, EBO;
    protected float[] vertices = null!;
    protected uint[] indices = null!;
    protected Vector3 posicion = Vector3.Zero;
    protected Vector3 escala = Vector3.One;
    protected Vector3 rotacion = Vector3.Zero;

    public virtual void Inicializar()
    {
        DefinirGeometria();
        ConfigurarBuffers();
    }

    protected abstract void DefinirGeometria();

    protected virtual void ConfigurarBuffers()
    {
        VAO = GL.GenVertexArray();
        VBO = GL.GenBuffer();
        EBO = GL.GenBuffer();

        GL.BindVertexArray(VAO);

        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), 
                     vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), 
                     indices, BufferUsageHint.StaticDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    public virtual void Renderizar(int shader, Matrix4 view, Matrix4 projection, Vector3? color = null)
    {
        Matrix4 model = Matrix4.CreateScale(escala) * 
                       Matrix4.CreateRotationX(rotacion.X) *
                       Matrix4.CreateRotationY(rotacion.Y) *
                       Matrix4.CreateRotationZ(rotacion.Z) *
                       Matrix4.CreateTranslation(posicion);

        Matrix4 mvp = model * view * projection;

        int mvpLocation = GL.GetUniformLocation(shader, "mvp");
        GL.UniformMatrix4(mvpLocation, false, ref mvp);

        Vector3 colorFinal = color ?? Vector3.One;
        int colorLocation = GL.GetUniformLocation(shader, "color");
        GL.Uniform3(colorLocation, colorFinal);

        GL.BindVertexArray(VAO);
        GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
    }

    public void EstablecerPosicion(Vector3 pos) => posicion = pos;
    public void EstablecerEscala(Vector3 esc) => escala = esc;
    public void EstablecerRotacion(Vector3 rot) => rotacion = rot;
}