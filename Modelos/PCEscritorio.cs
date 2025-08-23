using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class PCEscritorio : Objeto3D
{
    protected override void DefinirGeometria()
    {
        vertices = new float[] {
            // Cara frontal
            -0.6f, -1.5f,  0.8f,  // 0
             0.6f, -1.5f,  0.8f,  // 1
             0.6f,  1.5f,  0.8f,  // 2
            -0.6f,  1.5f,  0.8f,  // 3

            // Cara trasera
            -0.6f, -1.5f, -0.8f,  // 4
             0.6f, -1.5f, -0.8f,  // 5
             0.6f,  1.5f, -0.8f,  // 6
            -0.6f,  1.5f, -0.8f,  // 7

            // Panel 
            -0.25f,  0.9f,  0.85f, // 8  
             0.25f,  0.9f,  0.85f, // 9
             0.25f,  1.0f,  0.85f, // 10
            -0.25f,  1.0f,  0.85f, // 11

            // Rejillas
             0.65f, -0.8f, -0.6f, // 12 
             0.65f, -0.8f,  0.6f, // 13
             0.65f,  0.8f,  0.6f, // 14
             0.65f,  0.8f, -0.6f, // 15

            // Rejilla 
            -0.4f,  1.55f, -0.4f, // 16
             0.4f,  1.55f, -0.4f, // 17
             0.4f,  1.55f,  0.4f, // 18
            -0.4f,  1.55f,  0.4f, // 19

            // Puerto 
            -0.15f, -0.2f,  0.85f, // 20
             0.15f, -0.2f,  0.85f, // 21
             0.15f, -0.1f,  0.85f, // 22
            -0.15f, -0.1f,  0.85f  // 23
        };

        indices = new uint[] {
            // Cara frontal
            0, 1, 2,  2, 3, 0,
            // Cara trasera
            4, 7, 6,  6, 5, 4,
            // Cara izquierda
            4, 0, 3,  3, 7, 4,
            // Cara derecha
            1, 5, 6,  6, 2, 1,
            // Cara superior
            3, 2, 6,  6, 7, 3,
            // Cara inferior
            4, 5, 1,  1, 0, 4,
            
            // Panel
            8, 9, 10,  10, 11, 8,
            
            // Rejilla 
            12, 13, 14,  14, 15, 12,
            
            // Rejilla 
            16, 17, 18,  18, 19, 16,
            
            // Puertos
            20, 21, 22,  22, 23, 20
        };

        EstablecerPosicion(new Vector3(3.0f, 1.5f, -2.0f));
    }

    public override void Renderizar(int shader, Matrix4 view, Matrix4 projection, Vector3? color = null)
    {
        Matrix4 model = Matrix4.CreateScale(escala) * 
                       Matrix4.CreateRotationX(rotacion.X) *
                       Matrix4.CreateRotationY(rotacion.Y) *
                       Matrix4.CreateRotationZ(rotacion.Z) *
                       Matrix4.CreateTranslation(posicion);

        Matrix4 mvp = model * view * projection;

        int mvpLocation = GL.GetUniformLocation(shader, "mvp");
        GL.UniformMatrix4(mvpLocation, false, ref mvp);
        int colorLocation = GL.GetUniformLocation(shader, "color");

        GL.BindVertexArray(VAO);

        GL.Uniform3(colorLocation, new Vector3(0.2f, 0.2f, 0.25f)); 
        GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);

        GL.Uniform3(colorLocation, new Vector3(0.1f, 0.8f, 0.1f)); 
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 36 * sizeof(uint));

        GL.Uniform3(colorLocation, new Vector3(0.1f, 0.1f, 0.1f)); 
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 42 * sizeof(uint));

        GL.Uniform3(colorLocation, new Vector3(0.1f, 0.1f, 0.1f));
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 48 * sizeof(uint));

        GL.Uniform3(colorLocation, new Vector3(0.0f, 0.0f, 0.0f)); 
        GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, 54 * sizeof(uint));
    }
}