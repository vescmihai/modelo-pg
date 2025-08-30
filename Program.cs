using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {


        try
        {
            // Inicializar ambiente
            InicializarSistema();
            
            // AGREGAR: Validación de constantes antes de generar configuraciones
            ValidarConstantesConsistencia();
            
            // Generar configuraciones de ejemplo usando SOLO constantes
            GenerarConfiguracionesDesdeConstantes();
            
            // Validar todas las configuraciones generadas
            ValidarTodasLasConfiguraciones();
            
            // Mostrar información del sistema
            MostrarInformacionSistema();
            
            // Iniciar aplicación OpenTK
            Console.WriteLine("🚀 Iniciando motor de renderizado...");
            Console.WriteLine();
            
            using var game = new Game();
            game.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n💥 ERROR CRÍTICO: {ex.Message}");
            Console.WriteLine($"📋 Detalles: {ex.StackTrace}");
            Console.WriteLine("\n🔍 Diagnosticando problema...");
            DiagnosticarProblemas();
            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }

    private static void InicializarSistema()
    {
        Console.WriteLine("⚙️ Inicializando sistema...");
        
        // Crear directorios necesarios
        string[] directorios = { "Config", "Exports", "Logs" };
        
        foreach (var directorio in directorios)
        {
            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
                Console.WriteLine($"   📁 Directorio creado: {directorio}/");
            }
        }
        
        Console.WriteLine("✅ Sistema inicializado\n");
    }


    private static void ValidarConstantesConsistencia()
    {
        Console.WriteLine("Validando consistencia de constantes...");
        
        try
        {
            // Validar todas las constantes del sistema
            ConstantesGeometria.ValidarTodasLasConstantes();
            
            // Mostrar información de validación
            Console.WriteLine($"   ✅ Monitor: Marco ({ConstantesGeometria.Monitor.ANCHO_MARCO}x{ConstantesGeometria.Monitor.ALTO_MARCO}) > Display ({ConstantesGeometria.Monitor.ANCHO_DISPLAY}x{ConstantesGeometria.Monitor.ALTO_DISPLAY})");
            Console.WriteLine($"   ✅ Colores: {ConstantesGeometria.Colores.MARCOS_MONITOR.Length} variantes consistentes");
            Console.WriteLine($"   ✅ PC: Ventilador con {ConstantesGeometria.PC.ASPAS_VENTILADOR} aspas, radio {ConstantesGeometria.PC.RADIO_VENTILADOR}");
            Console.WriteLine($"   ✅ Teclado: {ConstantesGeometria.Teclado.FILAS_TECLAS}x{ConstantesGeometria.Teclado.COLUMNAS_TECLAS} teclas");
            
            Console.WriteLine("✅ Todas las constantes son consistentes\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   ❌ ERROR: {ex.Message}");
            throw;
        }
    }

    private static void GenerarConfiguracionesDesdeConstantes()
    {
        Console.WriteLine("📝 Generando configuraciones desde constantes centralizadas...");

        // Eliminar configuraciones existentes para forzar regeneración
        EliminarConfiguracionesAnteriores();

        // Generar 3 PCs usando solo constantes
        for (int i = 0; i < 3; i++)
        {
            string archivoPC = $"Config/pc_{i}.json";
            var configPC = CrearConfiguracionPCDesdeConstantes($"PC_Gaming_{i}", i);
            GuardarConfiguracion(configPC, archivoPC);
            Console.WriteLine($"   ✨ PC creado: {archivoPC}");
        }

        // Generar 3 monitores usando solo constantes CORREGIDO
        for (int i = 0; i < 3; i++)
        {
            string archivoMonitor = $"Config/monitor_{i}.json";
            var configMonitor = CrearConfiguracionMonitorDesdeConstantes($"Monitor_Gaming_{i}", i);
            GuardarConfiguracion(configMonitor, archivoMonitor);
            Console.WriteLine($"   ✨ Monitor creado (CORREGIDO): {archivoMonitor}");
        }

        // Generar 3 teclados usando solo constantes
        for (int i = 0; i < 3; i++)
        {
            string archivoTeclado = $"Config/teclado_{i}.json";
            var configTeclado = CrearConfiguracionTecladoDesdeConstantes($"Teclado_Gaming_{i}", i);
            GuardarConfiguracion(configTeclado, archivoTeclado);
            Console.WriteLine($"   ✨ Teclado creado: {archivoTeclado}");
        }

        // Crear documentación actualizada
        CrearDocumentacionActualizada();
        
        Console.WriteLine("✅ Configuraciones generadas desde constantes\n");
    }
    
    private static void EliminarConfiguracionesAnteriores()
    {
        if (Directory.Exists("Config"))
        {
            string[] archivos = Directory.GetFiles("Config", "*.json");
            foreach (string archivo in archivos)
            {
                try
                {
                    File.Delete(archivo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"   ⚠️ No se pudo eliminar {archivo}: {ex.Message}");
                }
            }
            
            if (archivos.Length > 0)
            {
                Console.WriteLine($"   🗑️ Eliminadas {archivos.Length} configuraciones anteriores");
            }
        }
    }

    // MÉTODOS DE CREACIÓN DE PCs
    private static DatosObjeto CrearConfiguracionPCDesdeConstantes(string nombre, int variante)
    {
        int colorIndex = variante % ConstantesGeometria.Colores.CARCASAS_PC.Length;

        return new DatosObjeto
        {
            Nombre = nombre,
            CentroMasa = VerticeADatos(ConstantesGeometria.PC.CENTRO_CARCASA),
            Partes = new List<DatosParte>
            {
                CrearCarcasaPCDesdeConstantes(colorIndex),
                CrearPanelLEDDesdeConstantes(colorIndex),
                CrearVentiladorDesdeConstantes(),
                CrearPuertosDesdeConstantes()
            }
        };
    }

    private static DatosParte CrearCarcasaPCDesdeConstantes(int colorIndex)
    {
        var carcasa = new DatosParte
        {
            Nombre = "Carcasa",
            CentroMasa = VerticeADatos(ConstantesGeometria.PC.CENTRO_CARCASA),
            Rotacion = new DatosColor { X = 0, Y = 0, Z = 0 },
            Escala = new DatosColor { X = 1, Y = 1, Z = 1 },
            Visible = true,
            Caras = new List<DatosCara>()
        };

        // Definir todas las caras de la carcasa usando constantes
        var definicionesCaras = new[]
        {
            new {
                Nombre = "Frontal",
                Vertices = new[] {
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 }
                }
            },
            new {
                Nombre = "Trasera",
                Vertices = new[] {
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 }
                }
            },
            new {
                Nombre = "Superior",
                Vertices = new[] {
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 }
                }
            },
            new {
                Nombre = "Inferior",
                Vertices = new[] {
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 }
                }
            },
            new {
                Nombre = "Lateral_Izquierdo",
                Vertices = new[] {
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = -ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 }
                }
            },
            new {
                Nombre = "Lateral_Derecho",
                Vertices = new[] {
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = -ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = -ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 },
                    new { X = ConstantesGeometria.PC.ANCHO_CARCASA/2, Y = ConstantesGeometria.PC.ALTO_CARCASA/2, Z = ConstantesGeometria.PC.PROFUNDIDAD_CARCASA/2 }
                }
            }
        };

        // Crear caras desde las definiciones
        foreach (var def in definicionesCaras)
        {
            var cara = new DatosCara
            {
                Nombre = def.Nombre,
                Color = VectorADatosColor(ConstantesGeometria.Colores.CARCASAS_PC[colorIndex]),
                Vertices = new List<DatosVertice>(),
                Indices = new List<uint> { 0, 1, 2, 2, 3, 0 }
            };

            foreach (var v in def.Vertices)
            {
                cara.Vertices.Add(new DatosVertice { X = (float)v.X, Y = (float)v.Y, Z = (float)v.Z });
            }

            carcasa.Caras.Add(cara);
        }

        return carcasa;
    }

    private static DatosParte CrearPanelLEDDesdeConstantes(int colorIndex)
    {
        return new DatosParte
        {
            Nombre = "Panel_LED",
            CentroMasa = VerticeADatos(ConstantesGeometria.PC.CENTRO_PANEL_LED),
            Rotacion = new DatosColor { X = 0, Y = 0, Z = 0 },
            Escala = new DatosColor { X = 1, Y = 1, Z = 1 },
            Visible = true,
            Caras = new List<DatosCara>
            {
                new DatosCara
                {
                    Nombre = "LED",
                    Color = VectorADatosColor(ConstantesGeometria.Colores.LEDS_PC[colorIndex]),
                    CentroMasa = new DatosVertice { X = 0, Y = 0, Z = 0 },
                    Vertices = new List<DatosVertice>
                    {
                        new() { X = -ConstantesGeometria.PC.ANCHO_LED/2, Y = -ConstantesGeometria.PC.ALTO_LED/2, Z = 0 },
                        new() { X = ConstantesGeometria.PC.ANCHO_LED/2, Y = -ConstantesGeometria.PC.ALTO_LED/2, Z = 0 },
                        new() { X = ConstantesGeometria.PC.ANCHO_LED/2, Y = ConstantesGeometria.PC.ALTO_LED/2, Z = 0 },
                        new() { X = -ConstantesGeometria.PC.ANCHO_LED/2, Y = ConstantesGeometria.PC.ALTO_LED/2, Z = 0 }
                    },
                    Indices = new List<uint> { 0, 1, 2, 2, 3, 0 }
                }
            }
        };
    }

    private static DatosParte CrearVentiladorDesdeConstantes()
    {
        var ventilador = new DatosParte
        {
            Nombre = "Ventilador_Superior",
            CentroMasa = VerticeADatos(ConstantesGeometria.PC.CENTRO_VENTILADOR),
            Rotacion = new DatosColor { X = 0, Y = 0, Z = 0 },
            Escala = new DatosColor { X = 1, Y = 1, Z = 1 },
            Visible = true,
            Caras = new List<DatosCara>()
        };

        var aspas = new DatosCara
        {
            Nombre = "Aspas",
            Color = VectorADatosColor(ConstantesGeometria.Colores.VENTILADOR),
            CentroMasa = new DatosVertice { X = 0, Y = 0, Z = 0 },
            Vertices = new List<DatosVertice> { new() { X = 0, Y = 0, Z = 0 } }, // Centro
            Indices = new List<uint>()
        };

        // Generar aspas usando constantes
        for (int i = 0; i < ConstantesGeometria.PC.ASPAS_VENTILADOR; i++)
        {
            float angulo = 2.0f * MathF.PI * i / ConstantesGeometria.PC.ASPAS_VENTILADOR;
            aspas.Vertices.Add(new DatosVertice
            {
                X = ConstantesGeometria.PC.RADIO_VENTILADOR * MathF.Cos(angulo),
                Y = 0,
                Z = ConstantesGeometria.PC.RADIO_VENTILADOR * MathF.Sin(angulo)
            });
        }

        // Conectar triángulos
        for (uint i = 1; i <= ConstantesGeometria.PC.ASPAS_VENTILADOR; i++)
        {
            uint siguiente = (i % (uint)ConstantesGeometria.PC.ASPAS_VENTILADOR) + 1;
            aspas.Indices.AddRange(new uint[] { 0, i, siguiente });
        }

        ventilador.Caras.Add(aspas);
        return ventilador;
    }

    private static DatosParte CrearPuertosDesdeConstantes()
    {
        var puertos = new DatosParte
        {
            Nombre = "Puertos_USB",
            CentroMasa = VerticeADatos(ConstantesGeometria.PC.CENTRO_PUERTOS),
            Rotacion = new DatosColor { X = 0, Y = 0, Z = 0 },
            Escala = new DatosColor { X = 1, Y = 1, Z = 1 },
            Visible = true,
            Caras = new List<DatosCara>()
        };

        // Crear 2 puertos usando constantes
        for (int i = 0; i < 2; i++)
        {
            float offsetX = (i - 0.5f) * ConstantesGeometria.PC.ESPACIADO_PUERTOS;
            
            var puerto = new DatosCara
            {
                Nombre = $"Puerto_{i + 1}",
                Color = VectorADatosColor(ConstantesGeometria.Colores.PUERTO_USB),
                CentroMasa = new DatosVertice { X = offsetX, Y = 0, Z = 0 },
                Vertices = new List<DatosVertice>
                {
                    new() { X = -ConstantesGeometria.PC.ANCHO_PUERTO/2, Y = -ConstantesGeometria.PC.ALTO_PUERTO/2, Z = 0 },
                    new() { X = ConstantesGeometria.PC.ANCHO_PUERTO/2, Y = -ConstantesGeometria.PC.ALTO_PUERTO/2, Z = 0 },
                    new() { X = ConstantesGeometria.PC.ANCHO_PUERTO/2, Y = ConstantesGeometria.PC.ALTO_PUERTO/2, Z = 0 },
                    new() { X = -ConstantesGeometria.PC.ANCHO_PUERTO/2, Y = ConstantesGeometria.PC.ALTO_PUERTO/2, Z = 0 }
                },
                Indices = new List<uint> { 0, 1, 2, 2, 3, 0 }
            };
            puertos.Caras.Add(puerto);
        }

        return puertos;
    }

    // MÉTODOS DE CREACIÓN DE MONITORES
    private static DatosObjeto CrearConfiguracionMonitorDesdeConstantes(string nombre, int variante)
    {
        int colorIndex = variante % ConstantesGeometria.Colores.MARCOS_MONITOR.Length;
        
        return new DatosObjeto
        {
            Nombre = nombre,
            CentroMasa = new DatosVertice { X = 0, Y = 0, Z = 0 },
            Partes = new List<DatosParte>
            {
                CrearPartePantallaDesdeConstantes(colorIndex),
                CrearParteBaseDesdeConstantes(),
                CrearParteSoporteDesdeConstantes()
            }
        };
    }

    private static DatosParte CrearPartePantallaDesdeConstantes(int colorIndex)
    {
        var pantalla = new DatosParte
        {
            Nombre = "Pantalla",
            CentroMasa = VerticeADatos(ConstantesGeometria.Monitor.CENTRO_PANTALLA),
            Rotacion = new DatosColor { X = 0, Y = 0, Z = 0 },
            Escala = new DatosColor { X = 1, Y = 1, Z = 1 },
            Visible = true,
            Caras = new List<DatosCara>()
        };
        
        // CORRECCIÓN: Usar dimensiones normalizadas consistentes
        float mitadAnchoMarco = ConstantesGeometria.Monitor.ANCHO_MARCO / 2f;
        float mitadAltoMarco = ConstantesGeometria.Monitor.ALTO_MARCO / 2f;
        float mitadAnchoDisplay = ConstantesGeometria.Monitor.ANCHO_DISPLAY / 2f;
        float mitadAltoDisplay = ConstantesGeometria.Monitor.ALTO_DISPLAY / 2f;
        
        // Marco con dimensiones exactas desde constantes
        var marco = new DatosCara
        {
            Nombre = "Marco",
            Color = VectorADatosColor(ConstantesGeometria.Colores.MARCOS_MONITOR[colorIndex]),
            CentroMasa = new DatosVertice { X = 0, Y = 0, Z = 0 },
            Vertices = new List<DatosVertice>
            {
                new() { X = -mitadAnchoMarco, Y = -mitadAltoMarco, Z = 0 },
                new() { X = mitadAnchoMarco, Y = -mitadAltoMarco, Z = 0 },
                new() { X = mitadAnchoMarco, Y = mitadAltoMarco, Z = 0 },
                new() { X = -mitadAnchoMarco, Y = mitadAltoMarco, Z = 0 }
            },
            Indices = new List<uint> { 0, 1, 2, 2, 3, 0 }
        };
        
        // Display con dimensiones exactas desde constantes
        var display = new DatosCara
        {
            Nombre = "Display",
            Color = VectorADatosColor(ConstantesGeometria.Colores.DISPLAYS_MONITOR[colorIndex]),
            CentroMasa = new DatosVertice { X = 0, Y = 0, Z = ConstantesGeometria.Monitor.PROFUNDIDAD_DISPLAY },
            Vertices = new List<DatosVertice>
            {
                new() { X = -mitadAnchoDisplay, Y = -mitadAltoDisplay, Z = 0 },
                new() { X = mitadAnchoDisplay, Y = -mitadAltoDisplay, Z = 0 },
                new() { X = mitadAnchoDisplay, Y = mitadAltoDisplay, Z = 0 },
                new() { X = -mitadAnchoDisplay, Y = mitadAltoDisplay, Z = 0 }
            },
            Indices = new List<uint> { 0, 1, 2, 2, 3, 0 }
        };
        
        pantalla.Caras.Add(marco);
        pantalla.Caras.Add(display);
        return pantalla;
    }

    private static DatosParte CrearParteBaseDesdeConstantes()
    {
        var baseParte = new DatosParte
        {
            Nombre = "Base",
            CentroMasa = VerticeADatos(ConstantesGeometria.Monitor.CENTRO_BASE),
            Rotacion = new DatosColor { X = 0, Y = 0, Z = 0 },
            Escala = new DatosColor { X = 1, Y = 1, Z = 1 },
            Visible = true,
            Caras = new List<DatosCara>()
        };
        
        var baseCircular = new DatosCara
        {
            Nombre = "Base_Circular",
            Color = VectorADatosColor(ConstantesGeometria.Colores.BASE_MONITOR),
            CentroMasa = new DatosVertice { X = 0, Y = 0, Z = 0 },
            Vertices = new List<DatosVertice> { new() { X = 0, Y = 0, Z = 0 } }, // Centro
            Indices = new List<uint>()
        };
        
        // Generar polígono usando constantes exactas
        for (int i = 0; i < ConstantesGeometria.Monitor.SEGMENTOS_BASE; i++)
        {
            float angulo = 2.0f * MathF.PI * i / ConstantesGeometria.Monitor.SEGMENTOS_BASE;
            baseCircular.Vertices.Add(new DatosVertice
            {
                X = ConstantesGeometria.Monitor.RADIO_BASE * MathF.Cos(angulo),
                Y = 0,
                Z = ConstantesGeometria.Monitor.RADIO_BASE * MathF.Sin(angulo)
            });
        }
        
        // Conectar triángulos desde el centro
        for (uint i = 1; i <= ConstantesGeometria.Monitor.SEGMENTOS_BASE; i++)
        {
            uint siguiente = (i % (uint)ConstantesGeometria.Monitor.SEGMENTOS_BASE) + 1;
            baseCircular.Indices.AddRange(new uint[] { 0, i, siguiente });
        }
        
        baseParte.Caras.Add(baseCircular);
        return baseParte;
    }
    private static DatosParte CrearParteSoporteDesdeConstantes()
    {
        return new DatosParte
        {
            Nombre = "Soporte",
            CentroMasa = VerticeADatos(ConstantesGeometria.Monitor.CENTRO_SOPORTE),
            Rotacion = new DatosColor { X = 0, Y = 0, Z = 0 },
            Escala = new DatosColor { X = 1, Y = 1, Z = 1 },
            Visible = true,
            Caras = new List<DatosCara>
            {
                new DatosCara
                {
                    Nombre = "Columna_Soporte",
                    Color = VectorADatosColor(ConstantesGeometria.Colores.SOPORTE_MONITOR),
                    CentroMasa = new DatosVertice { X = 0, Y = 0, Z = 0 },
                    Vertices = new List<DatosVertice>
                    {
                        new() { X = -ConstantesGeometria.Monitor.ANCHO_SOPORTE/2, Y = -ConstantesGeometria.Monitor.ALTO_SOPORTE/2, Z = 0 },
                        new() { X = ConstantesGeometria.Monitor.ANCHO_SOPORTE/2, Y = -ConstantesGeometria.Monitor.ALTO_SOPORTE/2, Z = 0 },
                        new() { X = ConstantesGeometria.Monitor.ANCHO_SOPORTE/2, Y = ConstantesGeometria.Monitor.ALTO_SOPORTE/2, Z = 0 },
                        new() { X = -ConstantesGeometria.Monitor.ANCHO_SOPORTE/2, Y = ConstantesGeometria.Monitor.ALTO_SOPORTE/2, Z = 0 }
                    },
                    Indices = new List<uint> { 0, 1, 2, 2, 3, 0 }
                }
            }
        };
    }

    // MÉTODOS DE CREACIÓN DE TECLADOS
    private static DatosObjeto CrearConfiguracionTecladoDesdeConstantes(string nombre, int variante)
    {
        int colorIndex = variante % ConstantesGeometria.Colores.BASES_TECLADO.Length;
        
        return new DatosObjeto
        {
            Nombre = nombre,
            CentroMasa = VerticeADatos(ConstantesGeometria.Teclado.CENTRO_BASE),
            Partes = new List<DatosParte>
            {
                CrearBaseTecladoDesdeConstantes(colorIndex),
                CrearTeclasDesdeConstantes(colorIndex)
            }
        };
    }

    private static DatosParte CrearBaseTecladoDesdeConstantes(int colorIndex)
    {
        return new DatosParte
        {
            Nombre = "Base",
            CentroMasa = VerticeADatos(ConstantesGeometria.Teclado.CENTRO_BASE),
            Rotacion = new DatosColor { X = 0, Y = 0, Z = 0 },
            Escala = new DatosColor { X = 1, Y = 1, Z = 1 },
            Visible = true,
            Caras = new List<DatosCara>
            {
                new DatosCara
                {
                    Nombre = "Base_Superior",
                    Color = VectorADatosColor(ConstantesGeometria.Colores.BASES_TECLADO[colorIndex]),
                    CentroMasa = new DatosVertice { X = 0, Y = ConstantesGeometria.Teclado.ALTO_BASE/2, Z = 0 },
                    Vertices = new List<DatosVertice>
                    {
                        new() { X = -ConstantesGeometria.Teclado.ANCHO_BASE/2, Y = 0, Z = -ConstantesGeometria.Teclado.PROFUNDIDAD_BASE/2 },
                        new() { X = ConstantesGeometria.Teclado.ANCHO_BASE/2, Y = 0, Z = -ConstantesGeometria.Teclado.PROFUNDIDAD_BASE/2 },
                        new() { X = ConstantesGeometria.Teclado.ANCHO_BASE/2, Y = 0, Z = ConstantesGeometria.Teclado.PROFUNDIDAD_BASE/2 },
                        new() { X = -ConstantesGeometria.Teclado.ANCHO_BASE/2, Y = 0, Z = ConstantesGeometria.Teclado.PROFUNDIDAD_BASE/2 }
                    },
                    Indices = new List<uint> { 0, 1, 2, 2, 3, 0 }
                },
                new DatosCara
                {
                    Nombre = "Base_Frontal",
                    Color = VectorADatosColor(new OpenTK.Mathematics.Vector3(
                        ConstantesGeometria.Colores.BASES_TECLADO[colorIndex].X * 0.8f,
                        ConstantesGeometria.Colores.BASES_TECLADO[colorIndex].Y * 0.8f,
                        ConstantesGeometria.Colores.BASES_TECLADO[colorIndex].Z * 0.8f)),
                    CentroMasa = new DatosVertice { X = 0, Y = ConstantesGeometria.Teclado.ALTO_BASE/4, Z = ConstantesGeometria.Teclado.PROFUNDIDAD_BASE/2 },
                    Vertices = new List<DatosVertice>
                    {
                        new() { X = -ConstantesGeometria.Teclado.ANCHO_BASE/2, Y = -ConstantesGeometria.Teclado.ALTO_BASE/2, Z = 0 },
                        new() { X = ConstantesGeometria.Teclado.ANCHO_BASE/2, Y = -ConstantesGeometria.Teclado.ALTO_BASE/2, Z = 0 },
                        new() { X = ConstantesGeometria.Teclado.ANCHO_BASE/2, Y = ConstantesGeometria.Teclado.ALTO_BASE/2, Z = 0 },
                        new() { X = -ConstantesGeometria.Teclado.ANCHO_BASE/2, Y = ConstantesGeometria.Teclado.ALTO_BASE/2, Z = 0 }
                    },
                    Indices = new List<uint> { 0, 1, 2, 2, 3, 0 }
                }
            }
        };
    }

    private static DatosParte CrearTeclasDesdeConstantes(int colorIndex)
    {
        var teclasParte = new DatosParte
        {
            Nombre = "Teclas",
            CentroMasa = VerticeADatos(ConstantesGeometria.Teclado.CENTRO_TECLAS),
            Rotacion = new DatosColor { X = 0, Y = 0, Z = 0 },
            Escala = new DatosColor { X = 1, Y = 1, Z = 1 },
            Visible = true,
            Caras = new List<DatosCara>()
        };

        // Generar teclas usando constantes
        for (int fila = 0; fila < ConstantesGeometria.Teclado.FILAS_TECLAS; fila++)
        {
            for (int col = 0; col < ConstantesGeometria.Teclado.COLUMNAS_TECLAS; col++)
            {
                float x = ConstantesGeometria.Teclado.INICIO_X_TECLAS + col * ConstantesGeometria.Teclado.ESPACIADO_TECLAS;
                float z = ConstantesGeometria.Teclado.INICIO_Z_TECLAS - fila * ConstantesGeometria.Teclado.ESPACIADO_TECLAS;
                
                // Lateral de la tecla
                var teclaLateral = new DatosCara
                {
                    Nombre = $"Tecla_Lateral_{fila}_{col}",
                    Color = VectorADatosColor(ConstantesGeometria.Colores.TECLAS_LATERAL[colorIndex]),
                    CentroMasa = new DatosVertice { X = x, Y = ConstantesGeometria.Teclado.ALTO_TECLA/2, Z = z },
                    Vertices = new List<DatosVertice>
                    {
                        new() { X = -ConstantesGeometria.Teclado.ANCHO_TECLA/2, Y = -ConstantesGeometria.Teclado.ALTO_TECLA/2, Z = -ConstantesGeometria.Teclado.PROFUNDIDAD_TECLA/2 },
                        new() { X = ConstantesGeometria.Teclado.ANCHO_TECLA/2, Y = -ConstantesGeometria.Teclado.ALTO_TECLA/2, Z = -ConstantesGeometria.Teclado.PROFUNDIDAD_TECLA/2 },
                        new() { X = ConstantesGeometria.Teclado.ANCHO_TECLA/2, Y = ConstantesGeometria.Teclado.ALTO_TECLA/2, Z = -ConstantesGeometria.Teclado.PROFUNDIDAD_TECLA/2 },
                        new() { X = -ConstantesGeometria.Teclado.ANCHO_TECLA/2, Y = ConstantesGeometria.Teclado.ALTO_TECLA/2, Z = -ConstantesGeometria.Teclado.PROFUNDIDAD_TECLA/2 }
                    },
                    Indices = new List<uint> { 0, 1, 2, 2, 3, 0 }
                };
                
                // Superficie superior de la tecla
                var teclaSuperior = new DatosCara
                {
                    Nombre = $"Tecla_Superior_{fila}_{col}",
                    Color = VectorADatosColor(ConstantesGeometria.Colores.TECLAS_SUPERIOR[colorIndex]),
                    CentroMasa = new DatosVertice { X = x, Y = ConstantesGeometria.Teclado.ALTO_TECLA, Z = z },
                    Vertices = new List<DatosVertice>
                    {
                        new() { X = -ConstantesGeometria.Teclado.ANCHO_TECLA/2 * 0.8f, Y = 0, Z = -ConstantesGeometria.Teclado.PROFUNDIDAD_TECLA/2 * 0.8f },
                        new() { X = ConstantesGeometria.Teclado.ANCHO_TECLA/2 * 0.8f, Y = 0, Z = -ConstantesGeometria.Teclado.PROFUNDIDAD_TECLA/2 * 0.8f },
                        new() { X = ConstantesGeometria.Teclado.ANCHO_TECLA/2 * 0.8f, Y = 0, Z = ConstantesGeometria.Teclado.PROFUNDIDAD_TECLA/2 * 0.8f },
                        new() { X = -ConstantesGeometria.Teclado.ANCHO_TECLA/2 * 0.8f, Y = 0, Z = ConstantesGeometria.Teclado.PROFUNDIDAD_TECLA/2 * 0.8f }
                    },
                    Indices = new List<uint> { 0, 1, 2, 2, 3, 0 }
                };
                
                teclasParte.Caras.Add(teclaLateral);
                teclasParte.Caras.Add(teclaSuperior);
            }
        }

        return teclasParte;
    }

    // MÉTODOS DE VALIDACIÓN
    private static void ValidarTodasLasConfiguraciones()
    {
        Console.WriteLine("🔍 Validando consistencia de todas las configuraciones...");
        
        string[] tiposObjetos = { "pc", "monitor", "teclado" };
        bool todoValido = true;
        
        foreach (string tipo in tiposObjetos)
        {
            var configuraciones = new List<DatosObjeto>();
            
            for (int i = 0; i < 3; i++)
            {
                string archivo = $"Config/{tipo}_{i}.json";
                if (File.Exists(archivo))
                {
                    try
                    {
                        string json = File.ReadAllText(archivo);
                        var configuracion = JsonSerializer.Deserialize<DatosObjeto>(json, new JsonSerializerOptions 
                        { 
                            PropertyNameCaseInsensitive = true 
                        });
                        
                        if (configuracion != null)
                        {
                            configuraciones.Add(configuracion);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   ❌ Error validando {archivo}: {ex.Message}");
                        todoValido = false;
                    }
                }
            }
            
            if (configuraciones.Count > 1)
            {
                bool tipoConsistente = ValidarConsistenciaTipo(configuraciones, tipo);
                if (tipoConsistente)
                {
                    Console.WriteLine($"   ✅ {tipo.ToUpper()}: Todas las configuraciones son consistentes");
                    
                    // Validación específica para monitores
                    if (tipo == "monitor")
                    {
                        ValidarDimensionesMonitoresEspecifica(configuraciones);
                    }
                }
                else
                {
                    Console.WriteLine($"   ❌ {tipo.ToUpper()}: Configuraciones inconsistentes detectadas");
                    todoValido = false;
                }
            }
        }
        
        if (todoValido)
        {
            Console.WriteLine("✅ Todas las configuraciones son válidas y consistentes\n");
        }
        else
        {
            Console.WriteLine("❌ Se detectaron problemas de consistencia\n");
        }
    }
    
    private static void ValidarDimensionesMonitoresEspecifica(List<DatosObjeto> monitores)
    {
        foreach (var monitor in monitores)
        {
            var pantalla = monitor.Partes.Find(p => p.Nombre == "Pantalla");
            if (pantalla != null)
            {
                var marco = pantalla.Caras.Find(c => c.Nombre == "Marco");
                var display = pantalla.Caras.Find(c => c.Nombre == "Display");
                
                if (marco != null && display != null && marco.Vertices.Count >= 4 && display.Vertices.Count >= 4)
                {
                    // Validar dimensiones exactas del marco
                    float anchoMarco = Math.Abs(marco.Vertices[1].X - marco.Vertices[0].X);
                    float altoMarco = Math.Abs(marco.Vertices[2].Y - marco.Vertices[1].Y);
                    
                    // Validar dimensiones exactas del display
                    float anchoDisplay = Math.Abs(display.Vertices[1].X - display.Vertices[0].X);
                    float altoDisplay = Math.Abs(display.Vertices[2].Y - display.Vertices[1].Y);
                    
                    // Verificar contra constantes exactas
                    float anchoMarcoEsperado = ConstantesGeometria.Monitor.ANCHO_MARCO;
                    float altoMarcoEsperado = ConstantesGeometria.Monitor.ALTO_MARCO;
                    float anchoDisplayEsperado = ConstantesGeometria.Monitor.ANCHO_DISPLAY;
                    float altoDisplayEsperado = ConstantesGeometria.Monitor.ALTO_DISPLAY;
                    
                    if (Math.Abs(anchoMarco - anchoMarcoEsperado) > ConstantesGeometria.Validacion.TOLERANCIA_POSICION ||
                        Math.Abs(altoMarco - altoMarcoEsperado) > ConstantesGeometria.Validacion.TOLERANCIA_POSICION)
                    {
                        Console.WriteLine($"   ⚠️ Monitor '{monitor.Nombre}': Marco {anchoMarco:F3}x{altoMarco:F3}, esperado {anchoMarcoEsperado}x{altoMarcoEsperado}");
                    }
                    
                    if (Math.Abs(anchoDisplay - anchoDisplayEsperado) > ConstantesGeometria.Validacion.TOLERANCIA_POSICION ||
                        Math.Abs(altoDisplay - altoDisplayEsperado) > ConstantesGeometria.Validacion.TOLERANCIA_POSICION)
                    {
                        Console.WriteLine($"   ⚠️ Monitor '{monitor.Nombre}': Display {anchoDisplay:F3}x{altoDisplay:F3}, esperado {anchoDisplayEsperado}x{altoDisplayEsperado}");
                    }
                    
                    // Validar que display sea menor que marco
                    if (anchoDisplay >= anchoMarco || altoDisplay >= altoMarco)
                    {
                        Console.WriteLine($"   ❌ Monitor '{monitor.Nombre}': Display debe ser menor que marco");
                    }
                }
            }
        }
        
        Console.WriteLine($"   📏 Monitores validados: Marco {ConstantesGeometria.Monitor.ANCHO_MARCO}x{ConstantesGeometria.Monitor.ALTO_MARCO}, Display {ConstantesGeometria.Monitor.ANCHO_DISPLAY}x{ConstantesGeometria.Monitor.ALTO_DISPLAY}");
    }

    private static bool ValidarConsistenciaTipo(List<DatosObjeto> configuraciones, string tipo)
    {
        if (configuraciones.Count < 2) return true;

        var referencia = configuraciones[0];

        for (int i = 1; i < configuraciones.Count; i++)
        {
            var actual = configuraciones[i];

            // Validar número de partes
            if (referencia.Partes.Count != actual.Partes.Count)
            {
                Console.WriteLine($"     {tipo} {i}: Número de partes diferente ({actual.Partes.Count} vs {referencia.Partes.Count})");
                return false;
            }

            // Validar cada parte
            for (int j = 0; j < referencia.Partes.Count; j++)
            {
                var parteRef = referencia.Partes[j];
                var parteActual = actual.Partes[j];

                if (parteRef.Caras.Count != parteActual.Caras.Count)
                {
                    Console.WriteLine($"     {tipo} {i}, parte {j}: Número de caras diferente");
                    return false;
                }

                // Validar geometría de caras principales
                for (int k = 0; k < Math.Min(parteRef.Caras.Count, 3); k++) // Solo validar primeras 3 caras para eficiencia
                {
                    var caraRef = parteRef.Caras[k];
                    var caraActual = parteActual.Caras[k];

                    if (caraRef.Vertices.Count != caraActual.Vertices.Count)
                    {
                        Console.WriteLine($"     {tipo} {i}, cara {k}: Número de vértices diferente");
                        return false;
                    }

                    // Validar dimensiones usando tolerancia
                    for (int v = 0; v < Math.Min(caraRef.Vertices.Count, 4); v++) // Solo validar primeros 4 vértices
                    {
                        var vRef = caraRef.Vertices[v];
                        var vActual = caraActual.Vertices[v];

                        if (Math.Abs(vRef.X - vActual.X) > ConstantesGeometria.Validacion.TOLERANCIA_POSICION ||
                            Math.Abs(vRef.Y - vActual.Y) > ConstantesGeometria.Validacion.TOLERANCIA_POSICION ||
                            Math.Abs(vRef.Z - vActual.Z) > ConstantesGeometria.Validacion.TOLERANCIA_POSICION)
                        {
                            Console.WriteLine($"     {tipo} {i}, vértice {v}: Posición diferente");
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    // MÉTODOS AUXILIARES Y DE UTILIDAD
    private static void GuardarConfiguracion(DatosObjeto datos, string rutaArchivo)
    {
        try
        {
            var opciones = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNameCaseInsensitive = true
            };

            string json = JsonSerializer.Serialize(datos, opciones);
            File.WriteAllText(rutaArchivo, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error guardando {rutaArchivo}: {ex.Message}");
        }
    }

    private static void CrearDocumentacionActualizada()
    {
        string archivoReadme = "Config/README.md";
        string documentacion = @"# SISTEMA 3D SIN HARDCODEO - COORDENADAS RELATIVAS

## OBJETIVOS
";

        try
        {
            File.WriteAllText(archivoReadme, documentacion);
            Console.WriteLine($"   Documentación actualizada: {archivoReadme}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Error creando documentación: {ex.Message}");
        }
    }

    private static void MostrarInformacionSistema()
    {
        Console.WriteLine("INFORMACIÓN DEL SISTEMA SIN HARDCODEO:");
        Console.WriteLine($"   OpenTK: 4.8.2");
        Console.WriteLine($"   Directorio: {Environment.CurrentDirectory}");
        Console.WriteLine($"   Inicio: {DateTime.Now:HH:mm:ss}");
        
        int archivosConfig = Directory.Exists("Config") ? Directory.GetFiles("Config", "*.json").Length : 0;
        Console.WriteLine($"   Configuraciones JSON: {archivosConfig}");
        
        Console.WriteLine($"   Constantes cargadas: {typeof(ConstantesGeometria).GetNestedTypes().Length} categorías");
        Console.WriteLine($"   Variantes de colores: {ConstantesGeometria.Colores.CARCASAS_PC.Length}");
        
        Console.WriteLine();
    }

    private static void DiagnosticarProblemas()
    {
        Console.WriteLine("DIAGNÓSTICO AUTOMÁTICO:");
        
        // Verificar OpenTK
        try
        {
            var testVector = new OpenTK.Mathematics.Vector3(1, 2, 3);
            Console.WriteLine("   OpenTK disponible");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Problema OpenTK: {ex.Message}");
        }
        
        // Verificar constantes
        try
        {
            float testConstante = ConstantesGeometria.Monitor.ANCHO_MARCO;
            Console.WriteLine($"   Constantes cargadas (Monitor.ANCHO_MARCO = {testConstante})");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Error cargando constantes: {ex.Message}");
        }
        
        // Verificar archivos de configuración
        string[] archivosRequeridos = {
            "Config/pc_0.json", "Config/pc_1.json", "Config/pc_2.json",
            "Config/monitor_0.json", "Config/monitor_1.json", "Config/monitor_2.json",
            "Config/teclado_0.json", "Config/teclado_1.json", "Config/teclado_2.json"
        };
        
        int archivosExistentes = 0;
        foreach (string archivo in archivosRequeridos)
        {
            if (File.Exists(archivo)) archivosExistentes++;
        }
        
        if (archivosExistentes == archivosRequeridos.Length)
        {
            Console.WriteLine("   Todas las configuraciones JSON disponibles");
        }
        else
        {
            Console.WriteLine($"   Configuraciones faltantes: {archivosRequeridos.Length - archivosExistentes}/{archivosRequeridos.Length}");
        }
        
        // Verificar memoria
        try
        {
            long memoria = GC.GetTotalMemory(false) / (1024 * 1024);
            Console.WriteLine($"   Memoria en uso: {memoria} MB");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Error verificando memoria: {ex.Message}");
        }
        
        Console.WriteLine();
    }

    // Métodos auxiliares para conversión
    private static DatosVertice VerticeADatos(Vertice vertice)
    {
        return new DatosVertice { X = vertice.X, Y = vertice.Y, Z = vertice.Z };
    }

    private static DatosColor VectorADatosColor(OpenTK.Mathematics.Vector3 vector)
    {
        return new DatosColor { X = vector.X, Y = vector.Y, Z = vector.Z };
    }

    private static bool ValidarConfiguracionJSON(string rutaArchivo)
    {
        try
        {
            if (!File.Exists(rutaArchivo)) return false;
            
            string contenido = File.ReadAllText(rutaArchivo);
            var datos = JsonSerializer.Deserialize<DatosObjeto>(contenido);
            
            if (datos == null || string.IsNullOrEmpty(datos.Nombre) || datos.Partes.Count == 0)
                return false;
                
            foreach (var parte in datos.Partes)
            {
                if (string.IsNullOrEmpty(parte.Nombre) || parte.Caras.Count == 0)
                    return false;
                    
                foreach (var cara in parte.Caras)
                {
                    if (cara.Vertices.Count < ConstantesGeometria.Validacion.MIN_VERTICES_CARA || 
                        cara.Indices.Count % 3 != 0)
                        return false;
                }
            }
            
            return true;
        }
        catch
        {
            return false;
        }
    }
}