using OpenTK.Mathematics;
using System;

public class Vertice
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public Vertice() { }

    public Vertice(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3 ToVector3() => new Vector3(X, Y, Z);

    public Vertice TransformarRelativoA(Vertice nuevoOrigen)
    {
        return new Vertice(X + nuevoOrigen.X, Y + nuevoOrigen.Y, Z + nuevoOrigen.Z);
    }

    public Vertice TransformarCompleto(Vertice origen, Vector3 rotacion, Vector3 escala)
    {
        // 1. Aplicar escala local
        float xEscalado = X * escala.X;
        float yEscalado = Y * escala.Y;
        float zEscalado = Z * escala.Z;

        // 2. Aplicar rotación (orden: X, Y, Z)
        Vector3 puntoRotado = new Vector3(xEscalado, yEscalado, zEscalado);
        
        // Rotación en X
        if (rotacion.X != 0)
        {
            float cos = MathF.Cos(rotacion.X);
            float sin = MathF.Sin(rotacion.X);
            float nuevaY = puntoRotado.Y * cos - puntoRotado.Z * sin;
            float nuevaZ = puntoRotado.Y * sin + puntoRotado.Z * cos;
            puntoRotado.Y = nuevaY;
            puntoRotado.Z = nuevaZ;
        }

        // Rotación en Y
        if (rotacion.Y != 0)
        {
            float cos = MathF.Cos(rotacion.Y);
            float sin = MathF.Sin(rotacion.Y);
            float nuevaX = puntoRotado.X * cos + puntoRotado.Z * sin;
            float nuevaZ = -puntoRotado.X * sin + puntoRotado.Z * cos;
            puntoRotado.X = nuevaX;
            puntoRotado.Z = nuevaZ;
        }

        // Rotación en Z
        if (rotacion.Z != 0)
        {
            float cos = MathF.Cos(rotacion.Z);
            float sin = MathF.Sin(rotacion.Z);
            float nuevaX = puntoRotado.X * cos - puntoRotado.Y * sin;
            float nuevaY = puntoRotado.X * sin + puntoRotado.Y * cos;
            puntoRotado.X = nuevaX;
            puntoRotado.Y = nuevaY;
        }

        // 3. Aplicar traslación al origen
        return new Vertice(
            puntoRotado.X + origen.X,
            puntoRotado.Y + origen.Y,
            puntoRotado.Z + origen.Z
        );
    }

    public float DistanciaA(Vertice otroVertice)
    {
        float dx = X - otroVertice.X;
        float dy = Y - otroVertice.Y;
        float dz = Z - otroVertice.Z;
        return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    public static Vertice operator +(Vertice a, Vertice b)
        => new Vertice(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vertice operator -(Vertice a, Vertice b)
        => new Vertice(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vertice operator *(Vertice v, float escalar)
        => new Vertice(v.X * escalar, v.Y * escalar, v.Z * escalar);

    public override string ToString() => $"({X:F2}, {Y:F2}, {Z:F2})";

    public Vertice Clonar() => new Vertice(X, Y, Z);
}