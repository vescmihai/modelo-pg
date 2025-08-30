using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

public class Parte : IDisposable
{
    public string Nombre { get; set; } = string.Empty;
    public List<Cara> Caras { get; set; } = new List<Cara>();
    
    // CORREGIDO: Centro de masa relativo al objeto padre
    public Vertice CentroMasa { get; set; } = new Vertice(0, 0, 0);
    
    public Vector3 Rotacion { get; set; } = Vector3.Zero;
    public Vector3 Escala { get; set; } = Vector3.One;
    public bool Visible { get; set; } = true;

    // Buffers OpenGL específicos para esta parte
    private int VAO = -1, VBO = -1, EBO = -1;
    private float[] verticesBuffer = Array.Empty<float>();
    private uint[] indicesBuffer = Array.Empty<uint>();
    private bool buffersConfigurados = false;

    public Parte() { }

    public Parte(string nombre)
    {
        Nombre = nombre;
    }

    public Parte(string nombre, Vertice centroMasa)
    {
        Nombre = nombre;
        CentroMasa = centroMasa;
    }

    public void GenerarBuffers(Vertice centroMasaObjeto)
    {
        var todosVertices = new List<float>();
        var todosIndices = new List<uint>();
        uint indiceBase = 0;

        var centroParteAbsoluto = CentroMasa.TransformarRelativoA(centroMasaObjeto);

        foreach (var cara in Caras)
        {
            // CORREGIDO: Pasar las transformaciones de la parte a la cara
            var verticesTransformados = cara.ObtenerVerticesTransformados(
                centroParteAbsoluto, Rotacion, Escala);
            
            // Convertir vértices a array de floats para OpenGL
            foreach (var vertice in verticesTransformados)
            {
                todosVertices.Add(vertice.X);
                todosVertices.Add(vertice.Y);
                todosVertices.Add(vertice.Z);
            }

            // Ajustar índices con el offset base
            foreach (var indice in cara.Indices)
            {
                todosIndices.Add(indice + indiceBase);
            }

            indiceBase += (uint)verticesTransformados.Count;
        }

        verticesBuffer = todosVertices.ToArray();
        indicesBuffer = todosIndices.ToArray();
    }

    public void ConfigurarBuffersOpenGL()
    {
        if (buffersConfigurados || verticesBuffer.Length == 0 || indicesBuffer.Length == 0) 
            return;

        try
        {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();

            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, verticesBuffer.Length * sizeof(float), 
                         verticesBuffer, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indicesBuffer.Length * sizeof(uint), 
                         indicesBuffer, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.BindVertexArray(0);
            buffersConfigurados = true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error configurando buffers OpenGL para parte {Nombre}: {ex.Message}");
        }
    }

    public void Renderizar(int shader, Matrix4 matrizObjeto, Matrix4 view, Matrix4 projection)
    {
        if (!Visible || !buffersConfigurados || VAO == -1) return;

        try
        {
            
            Matrix4 matrizParte = Matrix4.Identity;
            
            // Solo aplicar transformaciones adicionales si hay cambios dinámicos
            if (Escala != Vector3.One || Rotacion != Vector3.Zero)
            {
                matrizParte = Matrix4.CreateScale(Escala) * 
                             Matrix4.CreateRotationX(Rotacion.X) *
                             Matrix4.CreateRotationY(Rotacion.Y) *
                             Matrix4.CreateRotationZ(Rotacion.Z);
            }

            Matrix4 matrizFinal = matrizParte * matrizObjeto;
            Matrix4 mvp = matrizFinal * view * projection;

            int mvpLocation = GL.GetUniformLocation(shader, "mvp");
            GL.UniformMatrix4(mvpLocation, false, ref mvp);

            GL.BindVertexArray(VAO);

            // Renderizar cada cara con su color específico
            uint indiceOffset = 0;
            foreach (var cara in Caras)
            {
                int colorLocation = GL.GetUniformLocation(shader, "color");
                GL.Uniform3(colorLocation, cara.Color);

                GL.DrawElements(PrimitiveType.Triangles, cara.Indices.Count, 
                               DrawElementsType.UnsignedInt, (int)(indiceOffset * sizeof(uint)));
                
                indiceOffset += (uint)cara.Indices.Count;
            }

            GL.BindVertexArray(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error renderizando parte {Nombre}: {ex.Message}");
        }
    }

    public void Rotar(float deltaX, float deltaY, float deltaZ)
    {
        Rotacion += new Vector3(deltaX, deltaY, deltaZ);
        
    }

    public void EstablecerRotacion(float rotX, float rotY, float rotZ)
    {
        Rotacion = new Vector3(rotX, rotY, rotZ);
    }

    public void Escalar(float factorX, float factorY, float factorZ)
    {
        Escala = new Vector3(Escala.X * factorX, Escala.Y * factorY, Escala.Z * factorZ);
    }

    public void EstablecerEscala(float escalaX, float escalaY, float escalaZ)
    {
        Escala = new Vector3(escalaX, escalaY, escalaZ);
    }

    public void MostrarOcultar(bool visible)
    {
        Visible = visible;
    }

    public void CalcularCentroMasa()
    {
        if (Caras.Count == 0)
        {
            CentroMasa = new Vertice(0, 0, 0);
            return;
        }

        float sumaX = 0, sumaY = 0, sumaZ = 0;
        int totalVertices = 0;

        foreach (var cara in Caras)
        {
            foreach (var vertice in cara.Vertices)
            {
                // Los vértices están relativos a la cara, sumar el centro de masa de la cara
                var verticeAbsoluto = vertice.TransformarRelativoA(cara.CentroMasa);
                sumaX += verticeAbsoluto.X;
                sumaY += verticeAbsoluto.Y;
                sumaZ += verticeAbsoluto.Z;
                totalVertices++;
            }
        }

        if (totalVertices > 0)
        {
            CentroMasa = new Vertice(
                sumaX / totalVertices,
                sumaY / totalVertices,
                sumaZ / totalVertices
            );
        }
    }

    public Cara? ObtenerCara(string nombreCara)
    {
        return Caras.Find(c => c.Nombre.Equals(nombreCara, StringComparison.OrdinalIgnoreCase));
    }

    public void CargarDesdeDatos(DatosParte datos)
    {
        Nombre = datos.Nombre;
        CentroMasa = new Vertice(datos.CentroMasa.X, datos.CentroMasa.Y, datos.CentroMasa.Z);
        Rotacion = new Vector3(datos.Rotacion.X, datos.Rotacion.Y, datos.Rotacion.Z);
        Escala = new Vector3(datos.Escala.X, datos.Escala.Y, datos.Escala.Z);
        Visible = datos.Visible;

        Caras.Clear();
        foreach (var datosCara in datos.Caras)
        {
            var cara = new Cara();
            cara.CargarDesdeDatos(datosCara);
            Caras.Add(cara);
        }
    }

    public DatosParte ConvertirADatos()
    {
        var datos = new DatosParte
        {
            Nombre = Nombre,
            CentroMasa = new DatosVertice { X = CentroMasa.X, Y = CentroMasa.Y, Z = CentroMasa.Z },
            Rotacion = new DatosColor { X = Rotacion.X, Y = Rotacion.Y, Z = Rotacion.Z },
            Escala = new DatosColor { X = Escala.X, Y = Escala.Y, Z = Escala.Z },
            Visible = Visible
        };

        foreach (var cara in Caras)
        {
            var datosCara = new DatosCara
            {
                Nombre = cara.Nombre,
                CentroMasa = new DatosVertice { X = cara.CentroMasa.X, Y = cara.CentroMasa.Y, Z = cara.CentroMasa.Z },
                Color = new DatosColor { X = cara.Color.X, Y = cara.Color.Y, Z = cara.Color.Z }
            };

            foreach (var vertice in cara.Vertices)
            {
                datosCara.Vertices.Add(new DatosVertice { X = vertice.X, Y = vertice.Y, Z = vertice.Z });
            }

            datosCara.Indices.AddRange(cara.Indices);
            datos.Caras.Add(datosCara);
        }

        return datos;
    }

    public string ObtenerEstadisticas()
    {
        int totalVertices = 0;
        int totalTriangulos = 0;
        
        foreach (var cara in Caras)
        {
            totalVertices += cara.Vertices.Count;
            totalTriangulos += cara.Indices.Count / 3;
        }
        
        return $"Parte '{Nombre}': {Caras.Count} caras, {totalVertices} vértices, {totalTriangulos} triángulos";
    }

    public bool ValidarIntegridad(out List<string> errores)
    {
        errores = new List<string>();

        if (string.IsNullOrEmpty(Nombre))
            errores.Add("La parte no tiene nombre válido");

        if (Caras.Count < ConstantesGeometria.Validacion.MIN_CARAS_PARTE)
            errores.Add($"La parte necesita al menos {ConstantesGeometria.Validacion.MIN_CARAS_PARTE} caras");

        foreach (var cara in Caras)
        {
            if (!cara.ValidarIndices())
                errores.Add($"La cara '{cara.Nombre}' tiene índices inválidos");
        }

        return errores.Count == 0;
    }

    public void Dispose()
    {
        LiberarRecursos();
        GC.SuppressFinalize(this);
    }

    private void LiberarRecursos()
    {
        if (buffersConfigurados)
        {
            try
            {
                if (VBO != -1) GL.DeleteBuffer(VBO);
                if (EBO != -1) GL.DeleteBuffer(EBO);
                if (VAO != -1) GL.DeleteVertexArray(VAO);
                
                VBO = EBO = VAO = -1;
                buffersConfigurados = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error liberando recursos de parte {Nombre}: {ex.Message}");
            }
        }
    }

    ~Parte()
    {
        LiberarRecursos();
    }
}