using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

public class Cara
{
    public string Nombre { get; set; } = string.Empty;
    public List<Vertice> Vertices { get; set; } = new List<Vertice>();
    public List<uint> Indices { get; set; } = new List<uint>();
    public Vector3 Color { get; set; } = Vector3.One;
    public Vertice CentroMasa { get; set; } = new Vertice(0, 0, 0);

    public Cara() { }

    public Cara(string nombre)
    {
        Nombre = nombre;
    }

    public Cara(string nombre, Vector3 color)
    {
        Nombre = nombre;
        Color = color;
    }

    public List<Vertice> ObtenerVerticesTransformados(
        Vertice centroMasaParte, 
        Vector3 rotacionParte, 
        Vector3 escalaParte)
    {
        var verticesTransformados = new List<Vertice>();
        
        // Centro absoluto de la cara = centro cara + centro parte
        var centroCaraAbsoluto = CentroMasa.TransformarRelativoA(centroMasaParte);
        
        foreach (var vertice in Vertices)
        {
            // Los vértices están definidos relativos al centro de masa de la cara
            var verticeRelativoCara = vertice; // Ya está relativo a la cara
            
            var verticeTransformado = verticeRelativoCara.TransformarCompleto(
                CentroMasa, rotacionParte, escalaParte);
                
            var verticeFinal = verticeTransformado.TransformarRelativoA(centroMasaParte);
            
            verticesTransformados.Add(verticeFinal);
        }
        
        return verticesTransformados;
    }

    public void CalcularCentroMasa()
    {
        if (Vertices.Count == 0)
        {
            CentroMasa = new Vertice(0, 0, 0);
            return;
        }

        float sumaX = 0, sumaY = 0, sumaZ = 0;
        foreach (var vertice in Vertices)
        {
            sumaX += vertice.X;
            sumaY += vertice.Y;
            sumaZ += vertice.Z;
        }

        CentroMasa = new Vertice(
            sumaX / Vertices.Count,
            sumaY / Vertices.Count,
            sumaZ / Vertices.Count
        );
    }

    public void AgregarRectangulo(Vertice v1, Vertice v2, Vertice v3, Vertice v4)
    {
        uint indiceBase = (uint)Vertices.Count;
        
        // Los vértices se almacenan tal como se reciben (relativos al origen de la cara)
        Vertices.Add(v1.Clonar());
        Vertices.Add(v2.Clonar());
        Vertices.Add(v3.Clonar());
        Vertices.Add(v4.Clonar());
        
        // Dos triángulos para formar el rectángulo (orden correcto para normal hacia afuera)
        Indices.AddRange(new uint[] { 
            indiceBase, indiceBase + 1, indiceBase + 2, 
            indiceBase + 2, indiceBase + 3, indiceBase 
        });
        
    }

    public void AgregarTriangulo(Vertice v1, Vertice v2, Vertice v3)
    {
        uint indiceBase = (uint)Vertices.Count;
        
        Vertices.Add(v1.Clonar());
        Vertices.Add(v2.Clonar());
        Vertices.Add(v3.Clonar());
        
        // Orden para normal hacia afuera (sentido antihorario)
        Indices.AddRange(new uint[] { indiceBase, indiceBase + 1, indiceBase + 2 });
        
    }

    public uint AgregarVertice(Vertice vertice)
    {
        Vertices.Add(vertice.Clonar());
        return (uint)(Vertices.Count - 1);
    }

    public void AgregarTrianguloPorIndices(uint indice1, uint indice2, uint indice3)
    {
        if (indice1 < Vertices.Count && indice2 < Vertices.Count && indice3 < Vertices.Count)
        {
            Indices.AddRange(new uint[] { indice1, indice2, indice3 });
        }
        else
        {
            throw new ArgumentOutOfRangeException("Índices de vértices fuera de rango");
        }
    }

    public void CargarDesdeDatos(DatosCara datos)
    {
        Nombre = datos.Nombre;
        Color = new Vector3(datos.Color.X, datos.Color.Y, datos.Color.Z);
        CentroMasa = new Vertice(datos.CentroMasa.X, datos.CentroMasa.Y, datos.CentroMasa.Z);
        
        Vertices.Clear();
        foreach (var verticeData in datos.Vertices)
        {
            // Los vértices del JSON ya vienen relativos al origen de la cara
            Vertices.Add(new Vertice(verticeData.X, verticeData.Y, verticeData.Z));
        }
        
        Indices.Clear();
        Indices.AddRange(datos.Indices);
        
        // Validar que los índices sean válidos
        ValidarIndices();
    }

    public bool ValidarIndices()
    {
        if (Vertices.Count < ConstantesGeometria.Validacion.MIN_VERTICES_CARA)
        {
            Console.WriteLine($"WARNING: Cara '{Nombre}' tiene solo {Vertices.Count} vértices, mínimo {ConstantesGeometria.Validacion.MIN_VERTICES_CARA}");
            return false;
        }

        foreach (var indice in Indices)
        {
            if (indice >= Vertices.Count)
            {
                Console.WriteLine($"WARNING: Cara '{Nombre}' tiene índice inválido: {indice}");
                return false;
            }
        }
        return true;
    }

    public string ObtenerEstadisticas()
    {
        return $"Cara '{Nombre}': {Vertices.Count} vértices, {Indices.Count / 3} triángulos";
    }

    public void InvertirNormal()
    {
        for (int i = 0; i < Indices.Count; i += 3)
        {
            if (i + 2 < Indices.Count)
            {
                // Intercambiar segundo y tercer índice de cada triángulo
                (Indices[i + 1], Indices[i + 2]) = (Indices[i + 2], Indices[i + 1]);
            }
        }
    }
}