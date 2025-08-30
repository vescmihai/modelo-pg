using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

public abstract class Objeto : IDisposable
{
    public string Nombre { get; set; } = string.Empty;
    public List<Parte> Partes { get; set; } = new List<Parte>();
    public Vertice CentroMasa { get; set; } = new Vertice(0, 0, 0);
    public Vector3 Posicion { get; set; } = Vector3.Zero;
    public Vector3 Rotacion { get; set; } = Vector3.Zero;
    public Vector3 Escala { get; set; } = Vector3.One;
    public bool Visible { get; set; } = true;

    protected bool inicializado = false;
    protected bool cargadoDesdeArchivo = false;

    protected Objeto() { }

    protected Objeto(string nombre)
    {
        Nombre = nombre;
    }

    protected Objeto(string nombre, Vertice centroMasa)
    {
        Nombre = nombre;
        CentroMasa = centroMasa;
    }

    protected virtual void DefinirGeometria()
    {
        if (!cargadoDesdeArchivo)
        {
            throw new InvalidOperationException($"ERROR CRÍTICO: Objeto '{Nombre}' no puede definir geometría hardcodeada. Debe cargarse desde archivo JSON.");
        }
    }

    protected abstract void CargarDesdeArchivo(string rutaArchivo);

    protected void CargarDesdeJSON(string rutaArchivo)
    {
        try
        {
            if (!File.Exists(rutaArchivo))
            {
                throw new FileNotFoundException($"ARCHIVO REQUERIDO NO ENCONTRADO: {rutaArchivo}");
            }

            string contenidoJson = File.ReadAllText(rutaArchivo);
            var opciones = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };

            var datos = JsonSerializer.Deserialize<DatosObjeto>(contenidoJson, opciones);
            if (datos == null)
            {
                throw new InvalidOperationException($"ERROR DESERIALIZANDO: {rutaArchivo} contiene datos inválidos");
            }

            // Aplicar configuración
            AplicarConfiguracion(datos);
            cargadoDesdeArchivo = true;

            Console.WriteLine($"✅ Objeto {Nombre} cargado desde {rutaArchivo} con {Partes.Count} partes");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ ERROR CRÍTICO cargando {rutaArchivo}: {ex.Message}");
            throw;
        }
    }

    private void AplicarConfiguracion(DatosObjeto datos)
    {
        // Actualizar propiedades del objeto
        Nombre = datos.Nombre;
        CentroMasa = new Vertice(datos.CentroMasa.X, datos.CentroMasa.Y, datos.CentroMasa.Z);

        // Limpiar partes existentes
        foreach (var parte in Partes)
        {
            parte.Dispose();
        }
        Partes.Clear();

        // Crear partes desde los datos
        foreach (var datosParte in datos.Partes)
        {
            var parte = new Parte();
            parte.CargarDesdeDatos(datosParte);
            Partes.Add(parte);
        }

        // Validar integridad inmediatamente después de cargar
        ValidarIntegridadCompleta();
    }

    public virtual void Inicializar()
    {
        if (inicializado) return;

        if (!cargadoDesdeArchivo)
        {
            throw new InvalidOperationException($"ERROR: Objeto '{Nombre}' no puede inicializarse sin cargar desde archivo JSON");
        }

        try
        {
            // Ya no llamar DefinirGeometria() - todo viene del JSON
            
            foreach (var parte in Partes)
            {
                parte.GenerarBuffers(CentroMasa);
                parte.ConfigurarBuffersOpenGL();
            }

            inicializado = true;
            Console.WriteLine($"✅ Objeto {Nombre} inicializado con {Partes.Count} partes");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error inicializando {Nombre}: {ex.Message}");
            throw;
        }
    }

    public virtual void Renderizar(int shader, Matrix4 view, Matrix4 projection)
    {
        if (!Visible || !inicializado) return;

        try
        {
            // Matriz de transformación del objeto completo
            Matrix4 matrizObjeto = Matrix4.CreateScale(Escala) * 
                                   Matrix4.CreateRotationX(Rotacion.X) *
                                   Matrix4.CreateRotationY(Rotacion.Y) *
                                   Matrix4.CreateRotationZ(Rotacion.Z) *
                                   Matrix4.CreateTranslation(Posicion);

            // Renderizar cada parte
            foreach (var parte in Partes)
            {
                parte.Renderizar(shader, matrizObjeto, view, projection);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error renderizando {Nombre}: {ex.Message}");
        }
    }

    public Parte? ObtenerParte(string nombreParte)
    {
        return Partes.FirstOrDefault(p => p.Nombre.Equals(nombreParte, StringComparison.OrdinalIgnoreCase));
    }

    public void RotarParte(string nombreParte, float deltaX, float deltaY, float deltaZ)
    {
        var parte = ObtenerParte(nombreParte);
        parte?.Rotar(deltaX, deltaY, deltaZ);
    }

    public void EscalarParte(string nombreParte, float factorX, float factorY, float factorZ)
    {
        var parte = ObtenerParte(nombreParte);
        parte?.Escalar(factorX, factorY, factorZ);
    }

    public void MostrarOcultarParte(string nombreParte, bool visible)
    {
        var parte = ObtenerParte(nombreParte);
        if (parte != null) parte.Visible = visible;
    }

    // Métodos de transformación del objeto completo
    public void EstablecerPosicion(Vector3 pos) => Posicion = pos;
    public void EstablecerRotacion(Vector3 rot) => Rotacion = rot;
    public void EstablecerEscala(Vector3 esc) => Escala = esc;
    public void Mover(Vector3 delta) => Posicion += delta;
    public void Rotar(Vector3 delta) => Rotacion += delta;

    public void GuardarEnJSON(string rutaArchivo)
    {
        try
        {
            var datos = new DatosObjeto
            {
                Nombre = Nombre,
                CentroMasa = new DatosVertice { X = CentroMasa.X, Y = CentroMasa.Y, Z = CentroMasa.Z }
            };

            foreach (var parte in Partes)
            {
                datos.Partes.Add(parte.ConvertirADatos());
            }

            // Crear directorio si no existe
            string? directorio = Path.GetDirectoryName(rutaArchivo);
            if (!string.IsNullOrEmpty(directorio) && !Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            string json = JsonSerializer.Serialize(datos, opciones);
            File.WriteAllText(rutaArchivo, json);

            Console.WriteLine($"💾 Objeto {Nombre} guardado en {rutaArchivo}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error guardando {rutaArchivo}: {ex.Message}");
        }
    }

    public string ObtenerEstadisticas()
    {
        int totalCaras = Partes.Sum(p => p.Caras.Count);
        int totalVertices = Partes.Sum(p => p.Caras.Sum(c => c.Vertices.Count));
        int totalTriangulos = Partes.Sum(p => p.Caras.Sum(c => c.Indices.Count)) / 3;

        return $"{Nombre}: {Partes.Count} partes, {totalCaras} caras, {totalVertices} vértices, {totalTriangulos} triángulos";
    }

    public bool ValidarIntegridad(out List<string> errores)
    {
        errores = new List<string>();

        if (string.IsNullOrEmpty(Nombre))
        {
            errores.Add("El objeto no tiene nombre válido");
        }

        if (Partes.Count < ConstantesGeometria.Validacion.MIN_PARTES_OBJETO)
        {
            errores.Add($"El objeto necesita al menos {ConstantesGeometria.Validacion.MIN_PARTES_OBJETO} partes");
        }

        foreach (var parte in Partes)
        {
            if (string.IsNullOrEmpty(parte.Nombre))
            {
                errores.Add($"Parte sin nombre válido");
            }

            if (parte.Caras.Count < ConstantesGeometria.Validacion.MIN_CARAS_PARTE)
            {
                errores.Add($"La parte '{parte.Nombre}' necesita al menos {ConstantesGeometria.Validacion.MIN_CARAS_PARTE} caras");
            }

            foreach (var cara in parte.Caras)
            {
                if (cara.Vertices.Count < ConstantesGeometria.Validacion.MIN_VERTICES_CARA)
                {
                    errores.Add($"La cara '{cara.Nombre}' necesita al menos {ConstantesGeometria.Validacion.MIN_VERTICES_CARA} vértices");
                }

                if (cara.Indices.Count % 3 != 0)
                {
                    errores.Add($"La cara '{cara.Nombre}' no tiene índices válidos para triángulos");
                }

                foreach (var indice in cara.Indices)
                {
                    if (indice >= cara.Vertices.Count)
                    {
                        errores.Add($"La cara '{cara.Nombre}' tiene índice inválido: {indice}");
                    }
                }
            }
        }

        return errores.Count == 0;
    }

    private void ValidarIntegridadCompleta()
    {
        if (ValidarIntegridad(out List<string> errores))
        {
            Console.WriteLine($"✅ Objeto '{Nombre}' pasa validación de integridad");
        }
        else
        {
            Console.WriteLine($"⚠️ Objeto '{Nombre}' tiene problemas:");
            foreach (var error in errores)
            {
                Console.WriteLine($"   - {error}");
            }
        }
    }

    public virtual void Dispose()
    {
        foreach (var parte in Partes)
        {
            parte?.Dispose();
        }
        Partes.Clear();
        inicializado = false;
        cargadoDesdeArchivo = false;
        GC.SuppressFinalize(this);
    }

    ~Objeto()
    {
        Dispose();
    }
}

public class PC : Objeto
{
    public PC() : base("PC_Escritorio", new Vertice(0, 0, 0)) { }

    protected override void DefinirGeometria()
    {
        // ELIMINADO: Sin geometría hardcodeada
        throw new InvalidOperationException("PC no debe definir geometría hardcodeada. Debe cargarse desde JSON.");
    }

    protected override void CargarDesdeArchivo(string rutaArchivo)
    {
        if (string.IsNullOrEmpty(rutaArchivo))
        {
            throw new ArgumentException("Ruta de archivo requerida para PC");
        }

        CargarDesdeJSON(rutaArchivo);
        
        // Validaciones específicas de PC
        ValidarEstructuraPC();
    }

    private void ValidarEstructuraPC()
    {
        var partesRequeridas = new[] { "Carcasa", "Panel_LED", "Ventilador_Superior", "Puertos_USB" };
        var partesEncontradas = Partes.Select(p => p.Nombre).ToList();
        
        foreach (var parteRequerida in partesRequeridas)
        {
            if (!partesEncontradas.Contains(parteRequerida))
            {
                Console.WriteLine($"⚠️ PC '{Nombre}': Falta parte requerida '{parteRequerida}'");
            }
        }

        // Validar que el ventilador tenga el número correcto de aspas
        var ventilador = ObtenerParte("Ventilador_Superior");
        if (ventilador != null)
        {
            var aspas = ventilador.ObtenerCara("Aspas");
            if (aspas != null)
            {
                int verticesEsperados = ConstantesGeometria.PC.ASPAS_VENTILADOR + 1; // +1 por el centro
                if (aspas.Vertices.Count != verticesEsperados)
                {
                    Console.WriteLine($"⚠️ PC '{Nombre}': Ventilador tiene {aspas.Vertices.Count} vértices, esperados {verticesEsperados}");
                }
            }
        }
    }

    public static PC CrearDesdeArchivo(string rutaArchivo)
    {
        var pc = new PC();
        pc.CargarDesdeArchivo(rutaArchivo);
        return pc;
    }
}

public class Monitor : Objeto
{
    public Monitor() : base("Monitor", new Vertice(0, 0, 0)) { }

    protected override void DefinirGeometria()
    {
        // ELIMINADO: Sin geometría hardcodeada
        throw new InvalidOperationException("Monitor no debe definir geometría hardcodeada. Debe cargarse desde JSON.");
    }

    protected override void CargarDesdeArchivo(string rutaArchivo)
    {
        if (string.IsNullOrEmpty(rutaArchivo))
        {
            throw new ArgumentException("Ruta de archivo requerida para Monitor");
        }

        CargarDesdeJSON(rutaArchivo);
        
        // Validaciones específicas de Monitor
        ValidarEstructuraMonitor();
    }

    private void ValidarEstructuraMonitor()
    {
        var partesRequeridas = new[] { "Pantalla", "Base", "Soporte" };
        var partesEncontradas = Partes.Select(p => p.Nombre).ToList();
        
        foreach (var parteRequerida in partesRequeridas)
        {
            if (!partesEncontradas.Contains(parteRequerida))
            {
                Console.WriteLine($"⚠️ Monitor '{Nombre}': Falta parte requerida '{parteRequerida}'");
            }
        }

        // Validar dimensiones de marco y display
        var pantalla = ObtenerParte("Pantalla");
        if (pantalla != null)
        {
            var marco = pantalla.ObtenerCara("Marco");
            var display = pantalla.ObtenerCara("Display");
            
            if (marco != null && display != null)
            {
                // Validar que el display sea más pequeño que el marco
                if (marco.Vertices.Count >= 4 && display.Vertices.Count >= 4)
                {
                    var anchoMarco = Math.Abs(marco.Vertices[1].X - marco.Vertices[0].X);
                    var anchoDisplay = Math.Abs(display.Vertices[1].X - display.Vertices[0].X);
                    
                    if (anchoDisplay >= anchoMarco)
                    {
                        Console.WriteLine($"⚠️ Monitor '{Nombre}': Display ({anchoDisplay}) debe ser menor que Marco ({anchoMarco})");
                    }
                }
            }
        }

        // Validar base circular
        var baseParte = ObtenerParte("Base");
        if (baseParte != null)
        {
            var baseCircular = baseParte.ObtenerCara("Base_Circular");
            if (baseCircular != null)
            {
                int verticesEsperados = ConstantesGeometria.Monitor.SEGMENTOS_BASE + 1; // +1 por el centro
                if (baseCircular.Vertices.Count != verticesEsperados)
                {
                    Console.WriteLine($"⚠️ Monitor '{Nombre}': Base tiene {baseCircular.Vertices.Count} vértices, esperados {verticesEsperados}");
                }
            }
        }
    }

    public static Monitor CrearDesdeArchivo(string rutaArchivo)
    {
        var monitor = new Monitor();
        monitor.CargarDesdeArchivo(rutaArchivo);
        return monitor;
    }
}

public class Teclado : Objeto
{
    public Teclado() : base("Teclado", new Vertice(0, 0, 0)) { }

    protected override void DefinirGeometria()
    {
        // ELIMINADO: Sin geometría hardcodeada
        throw new InvalidOperationException("Teclado no debe definir geometría hardcodeada. Debe cargarse desde JSON.");
    }

    protected override void CargarDesdeArchivo(string rutaArchivo)
    {
        if (string.IsNullOrEmpty(rutaArchivo))
        {
            throw new ArgumentException("Ruta de archivo requerida para Teclado");
        }

        CargarDesdeJSON(rutaArchivo);
        
        // Validaciones específicas de Teclado
        ValidarEstructuraTeclado();
    }

    private void ValidarEstructuraTeclado()
    {
        var partesRequeridas = new[] { "Base", "Teclas" };
        var partesEncontradas = Partes.Select(p => p.Nombre).ToList();
        
        foreach (var parteRequerida in partesRequeridas)
        {
            if (!partesEncontradas.Contains(parteRequerida))
            {
                Console.WriteLine($"⚠️ Teclado '{Nombre}': Falta parte requerida '{parteRequerida}'");
            }
        }

        // Validar número de teclas
        var teclasParte = ObtenerParte("Teclas");
        if (teclasParte != null)
        {
            int teclasLaterales = teclasParte.Caras.Count(c => c.Nombre.StartsWith("Tecla_Lateral_"));
            int teclasSuperiores = teclasParte.Caras.Count(c => c.Nombre.StartsWith("Tecla_Superior_"));
            
            int teclasEsperadas = ConstantesGeometria.Teclado.FILAS_TECLAS * ConstantesGeometria.Teclado.COLUMNAS_TECLAS;
            
            if (teclasLaterales != teclasEsperadas)
            {
                Console.WriteLine($"⚠️ Teclado '{Nombre}': {teclasLaterales} teclas laterales, esperadas {teclasEsperadas}");
            }
            
            if (teclasSuperiores != teclasEsperadas)
            {
                Console.WriteLine($"⚠️ Teclado '{Nombre}': {teclasSuperiores} teclas superiores, esperadas {teclasEsperadas}");
            }
        }
    }

    public static Teclado CrearDesdeArchivo(string rutaArchivo)
    {
        var teclado = new Teclado();
        teclado.CargarDesdeArchivo(rutaArchivo);
        return teclado;
    }
}