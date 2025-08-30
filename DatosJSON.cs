using System.Collections.Generic;

public class DatosVertice
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}

public class DatosColor
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
}

public class DatosCara
{
    public string Nombre { get; set; } = string.Empty;
    public List<DatosVertice> Vertices { get; set; } = new List<DatosVertice>();
    public List<uint> Indices { get; set; } = new List<uint>();
    public DatosColor Color { get; set; } = new DatosColor { X = 1, Y = 1, Z = 1 };
    public DatosVertice CentroMasa { get; set; } = new DatosVertice();
}

public class DatosParte
{
    public string Nombre { get; set; } = string.Empty;
    public DatosVertice CentroMasa { get; set; } = new DatosVertice();
    public DatosColor Rotacion { get; set; } = new DatosColor();
    public DatosColor Escala { get; set; } = new DatosColor { X = 1, Y = 1, Z = 1 };
    public bool Visible { get; set; } = true;
    public List<DatosCara> Caras { get; set; } = new List<DatosCara>();
}

public class DatosObjeto
{
    public string Nombre { get; set; } = string.Empty;
    public DatosVertice CentroMasa { get; set; } = new DatosVertice();
    public List<DatosParte> Partes { get; set; } = new List<DatosParte>();
}