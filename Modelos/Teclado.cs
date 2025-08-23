using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Collections.Generic;

public class Teclado : Objeto3D
{
    private int VAO_teclas, VBO_teclas, EBO_teclas;
    private float[] vertices_teclas = null!;
    private uint[] indices_teclas = null!;

    protected override void DefinirGeometria()
    {
        // Cuerpo
        vertices = new float[] {
            // Base
            -3.0f,  0.0f,  1.2f,   // 0
             3.0f,  0.0f,  1.2f,   // 1
             3.0f,  0.1f,  1.2f,   // 2
            -3.0f,  0.1f,  1.2f,   // 3

            -3.0f,  0.0f, -1.2f,   // 4
             3.0f,  0.0f, -1.2f,   // 5
             3.0f,  0.1f, -1.2f,   // 6
            -3.0f,  0.1f, -1.2f    // 7
        };

        indices = new uint[] {
            // base
            0, 1, 2,  2, 3, 0,     // superior
            4, 7, 6,  6, 5, 4,     // inferior
            0, 4, 7,  7, 3, 0,     // izquierda
            1, 5, 6,  6, 2, 1,     // derecha
            0, 1, 5,  5, 4, 0,     // frontal
            3, 2, 6,  6, 7, 3      // trasera
        };

        GenerarTeclas();
        
        EstablecerPosicion(new Vector3(-2.0f, 0.05f, 2.0f));
    }

    private void GenerarTeclas()
    {
        var teclas = new List<float>();
        var indicesTeclas = new List<uint>();
        uint indiceBase = 0;

        // cuadrícula de teclas
        for (int fila = 0; fila < 4; fila++)
        {
            for (int col = 0; col < 12; col++)
            {
                float x = -2.5f + col * 0.4f;
                float z = 0.8f - fila * 0.4f;
                float y = 0.12f;

                // Vértices tecla
                teclas.AddRange(new float[] {
                    x - 0.15f, y,      z - 0.15f,  // 0
                    x + 0.15f, y,      z - 0.15f,  // 1
                    x + 0.15f, y + 0.08f, z - 0.15f,  // 2
                    x - 0.15f, y + 0.08f, z - 0.15f,  // 3

                    x - 0.15f, y,      z + 0.15f,  // 4
                    x + 0.15f, y,      z + 0.15f,  // 5
                    x + 0.15f, y + 0.08f, z + 0.15f,  // 6
                    x - 0.15f, y + 0.08f, z + 0.15f   // 7
                });

                // Índices para esta tecla
                uint baseIdx = indiceBase;
                indicesTeclas.AddRange(new uint[] {
                    // Cara superior
                    baseIdx + 3, baseIdx + 2, baseIdx + 6,  baseIdx + 6, baseIdx + 7, baseIdx + 3,
                    // Caras laterales
                    baseIdx + 0, baseIdx + 1, baseIdx + 2,  baseIdx + 2, baseIdx + 3, baseIdx + 0,
                    baseIdx + 4, baseIdx + 7, baseIdx + 6,  baseIdx + 6, baseIdx + 5, baseIdx + 4,
                    baseIdx + 0, baseIdx + 4, baseIdx + 7,  baseIdx + 7, baseIdx + 3, baseIdx + 0,
                    baseIdx + 1, baseIdx + 5, baseIdx + 6,  baseIdx + 6, baseIdx + 2, baseIdx + 1
                });

                indiceBase += 8;
            }
        }

        vertices_teclas = teclas.ToArray();
        indices_teclas = indicesTeclas.ToArray();
    }

    public override void Inicializar()
    {
        base.Inicializar();

        VAO_teclas = GL.GenVertexArray();
        VBO_teclas = GL.GenBuffer();
        EBO_teclas = GL.GenBuffer();

        GL.BindVertexArray(VAO_teclas);
        GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_teclas);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices_teclas.Length * sizeof(float), 
                     vertices_teclas, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO_teclas);
        GL.BufferData(BufferTarget.ElementArrayBuffer, indices_teclas.Length * sizeof(uint), 
                     indices_teclas, BufferUsageHint.StaticDraw);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }

    public override void Renderizar(int shader, Matrix4 view, Matrix4 projection, Vector3? color = null)
    {
        base.Renderizar(shader, view, projection, new Vector3(0.1f, 0.1f, 0.1f));

        Matrix4 model = Matrix4.CreateScale(escala) * 
                       Matrix4.CreateTranslation(posicion);
        Matrix4 mvp = model * view * projection;

        int mvpLocation = GL.GetUniformLocation(shader, "mvp");
        int colorLocation = GL.GetUniformLocation(shader, "color");

        GL.UniformMatrix4(mvpLocation, false, ref mvp);
        GL.Uniform3(colorLocation, new Vector3(0.9f, 0.9f, 0.9f)); // Teclas blancas

        GL.BindVertexArray(VAO_teclas);
        GL.DrawElements(PrimitiveType.Triangles, indices_teclas.Length, DrawElementsType.UnsignedInt, 0);
    }
}