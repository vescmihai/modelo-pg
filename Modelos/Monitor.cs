using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Monitor : Objeto3D
{
    private int VAO_pantalla, VBO_pantalla, EBO_pantalla;
    private int VAO_base, VBO_base, EBO_base;
    private float[] vertices_pantalla = null!;
    private float[] vertices_base = null!;
    private uint[] indices_pantalla = null!;
    private uint[] indices_base = null!;

    protected override void DefinirGeometria()
    {
        // Pantalla 
        vertices_pantalla = new float[] {
            // marco
            -2.5f,  0.8f,  0.15f,  // 0 
             2.5f,  0.8f,  0.15f,  // 1 
             2.5f,  3.2f,  0.15f,  // 2 
            -2.5f,  3.2f,  0.15f,  // 3 

            // Parte trasera 
            -2.5f,  0.8f, -0.15f,  // 4
             2.5f,  0.8f, -0.15f,  // 5
             2.5f,  3.2f, -0.15f,  // 6
            -2.5f,  3.2f, -0.15f,  // 7

            // Pantalla
            -2.2f,  1.1f,  0.16f,  // 8
             2.2f,  1.1f,  0.16f,  // 9
             2.2f,  2.9f,  0.16f,  // 10
            -2.2f,  2.9f,  0.16f   // 11
        };

        indices_pantalla = new uint[] {
            // Cara frontal del marco
            0, 1, 2,  2, 3, 0,
            // Cara trasera
            4, 7, 6,  6, 5, 4,
            // Lados
            0, 4, 7,  7, 3, 0,  // izquierda
            1, 5, 6,  6, 2, 1,  // derecha
            3, 2, 6,  6, 7, 3,  // superior
            0, 1, 5,  5, 4, 0,  // inferior
            
            // display
            8, 9, 10,  10, 11, 8
        };

        // Base del monitor
        vertices_base = new float[] {
            // Soporte vertical
            -0.1f,  0.0f,  0.1f,   // 0
             0.1f,  0.0f,  0.1f,   // 1
             0.1f,  0.8f,  0.1f,   // 2
            -0.1f,  0.8f,  0.1f,   // 3

            -0.1f,  0.0f, -0.1f,   // 4
             0.1f,  0.0f, -0.1f,   // 5
             0.1f,  0.8f, -0.1f,   // 6
            -0.1f,  0.8f, -0.1f,   // 7

            // Base circular (aproximada con octágono)
            -0.8f,  0.0f,  0.0f,   // 8
            -0.6f,  0.0f,  0.6f,   // 9
             0.0f,  0.0f,  0.8f,   // 10
             0.6f,  0.0f,  0.6f,   // 11
             0.8f,  0.0f,  0.0f,   // 12
             0.6f,  0.0f, -0.6f,   // 13
             0.0f,  0.0f, -0.8f,   // 14
            -0.6f,  0.0f, -0.6f    // 15
        };

        indices_base = new uint[] {
            // Soporte vertical
            0, 1, 2,  2, 3, 0,  // frontal
            4, 7, 6,  6, 5, 4,  // trasera
            0, 4, 7,  7, 3, 0,  // izquierda
            1, 5, 6,  6, 2, 1,  // derecha

            // Base circular (triángulos desde el centro)
            0, 8, 9,   // usamos vértice 0 como centro aproximado
            0, 9, 10,
            0, 10, 11,
            0, 11, 12,
            0, 12, 13,
            0, 13, 14,
            0, 14, 15,
            0, 15, 8
        };
    }

    public override void Inicializar()
    {
        DefinirGeometria();
        
        // pantalla
        VAO_pantalla = GL.GenVertexArray();
        VBO_pantalla = GL.GenBuffer();
        EBO_pantalla = GL.GenBuffer();

        GL.BindVertexArray(VAO_pantalla);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_pantalla);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices_pantalla.Length * sizeof(float), 
                     vertices_pantalla, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO_pantalla);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices_pantalla.Length * sizeof(uint), 
                     indices_pantalla, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);

        // base
        VAO_base = GL.GenVertexArray();
        VBO_base = GL.GenBuffer();
        EBO_base = GL.GenBuffer();

        GL.BindVertexArray(VAO_base);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_base);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices_base.Length * sizeof(float), 
                     vertices_base, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO_base);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices_base.Length * sizeof(uint), 
                     indices_base, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    public override void Renderizar(int shader, Matrix4 view, Matrix4 projection, Vector3? color = null)
    {
        Matrix4 model = Matrix4.CreateTranslation(new Vector3(0f, 0.8f, -0.5f));
        Matrix4 mvp = model * view * projection;

        int mvpLocation = GL.GetUniformLocation(shader, "mvp");
        int colorLocation = GL.GetUniformLocation(shader, "color");

        // marco negro
        GL.UniformMatrix4(mvpLocation, false, ref mvp);
        GL.Uniform3(colorLocation, new Vector3(0.1f, 0.1f, 0.1f));
        GL.BindVertexArray(VAO_pantalla);
        GL.DrawElements(PrimitiveType.Triangles, indices_pantalla.Length - 6, DrawElementsType.UnsignedInt, 0);

        // display 
        GL.Uniform3(colorLocation, new Vector3(0.1f, 0.2f, 0.4f));
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, (indices_pantalla.Length - 6) * sizeof(uint));

        // base
        GL.Uniform3(colorLocation, new Vector3(0.6f, 0.6f, 0.7f));
        GL.BindVertexArray(VAO_base);
        GL.DrawElements(PrimitiveType.Triangles, indices_base.Length, DrawElementsType.UnsignedInt, 0);
    }
}