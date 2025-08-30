using OpenTK.Mathematics;


public static class ConstantesGeometria
{
    // Monitor - Dimensiones estandarizadas CORREGIDAS
    public static class Monitor
    {
        public const float ANCHO_MARCO = 1.6f;
        public const float ALTO_MARCO = 0.9f;
        public const float PROFUNDIDAD_MARCO = 0.05f;
        
        public const float ANCHO_DISPLAY = 1.4f;
        public const float ALTO_DISPLAY = 0.7f;
        public const float PROFUNDIDAD_DISPLAY = 0.02f; // REDUCIDO para mejor proporción
        
        public const float RADIO_BASE = 0.6f;
        public const int SEGMENTOS_BASE = 8;
        
        public const float ANCHO_SOPORTE = 0.2f;
        public const float ALTO_SOPORTE = 1.2f;
        public const float PROFUNDIDAD_SOPORTE = 0.2f;
        
        // Posiciones relativas
        public static readonly Vertice CENTRO_PANTALLA = new(0, 0.5f, 0);
        public static readonly Vertice CENTRO_BASE = new(0, -1.0f, 0);
        public static readonly Vertice CENTRO_SOPORTE = new(0, -0.2f, -0.2f);

        public static void ValidarProporcionesMonitor()
        {
            if (ANCHO_DISPLAY >= ANCHO_MARCO || ALTO_DISPLAY >= ALTO_MARCO)
            {
                throw new InvalidOperationException(
                    $"Display ({ANCHO_DISPLAY}x{ALTO_DISPLAY}) debe ser menor que Marco ({ANCHO_MARCO}x{ALTO_MARCO})"
                );
            }
            
            if (PROFUNDIDAD_DISPLAY > PROFUNDIDAD_MARCO)
            {
                throw new InvalidOperationException(
                    $"Profundidad Display ({PROFUNDIDAD_DISPLAY}) debe ser menor que Marco ({PROFUNDIDAD_MARCO})"
                );
            }
        }
    }
    
    // PC - Dimensiones estandarizadas
    public static class PC
    {
        public const float ANCHO_CARCASA = 1.0f;
        public const float ALTO_CARCASA = 2.0f;
        public const float PROFUNDIDAD_CARCASA = 1.6f;
        
        public const float RADIO_VENTILADOR = 0.3f;
        public const int ASPAS_VENTILADOR = 6;
        
        public const float ANCHO_LED = 0.4f;
        public const float ALTO_LED = 0.08f;
        public const float PROFUNDIDAD_LED = 0.01f;
        
        public const float ANCHO_PUERTO = 0.1f;
        public const float ALTO_PUERTO = 0.06f;
        public const float ESPACIADO_PUERTOS = 0.2f;
        
        // Posiciones relativas
        public static readonly Vertice CENTRO_CARCASA = new(0, 0, 0);
        public static readonly Vertice CENTRO_PANEL_LED = new(0, 0.8f, 0.9f);
        public static readonly Vertice CENTRO_VENTILADOR = new(0, 1.2f, 0);
        public static readonly Vertice CENTRO_PUERTOS = new(0, -0.8f, 0.9f);
    }
    
    // Teclado - Dimensiones estandarizadas
    public static class Teclado
    {
        public const float ANCHO_BASE = 4.0f;
        public const float ALTO_BASE = 0.1f;
        public const float PROFUNDIDAD_BASE = 1.6f;
        
        public const float ANCHO_TECLA = 0.3f;
        public const float ALTO_TECLA = 0.08f;
        public const float PROFUNDIDAD_TECLA = 0.3f;
        
        public const int FILAS_TECLAS = 4;
        public const int COLUMNAS_TECLAS = 10;
        public const float ESPACIADO_TECLAS = 0.4f;
        public const float INICIO_X_TECLAS = -1.8f;
        public const float INICIO_Z_TECLAS = 0.6f;
        
        // Posiciones relativas
        public static readonly Vertice CENTRO_BASE = new(0, 0, 0);
        public static readonly Vertice CENTRO_TECLAS = new(0, 0.15f, 0);
    }
    
    // Colores predefinidos - Arrays para variantes
    public static class Colores
    {
        public static readonly Vector3[] CARCASAS_PC = {
            new(0.2f, 0.2f, 0.25f),    // Gris azulado
            new(0.1f, 0.1f, 0.15f),    // Negro azulado
            new(0.25f, 0.15f, 0.15f)   // Gris rojizo
        };
        
        public static readonly Vector3[] LEDS_PC = {
            new(0.1f, 0.8f, 0.1f),     // Verde brillante
            new(0.8f, 0.1f, 0.1f),     // Rojo brillante
            new(0.1f, 0.1f, 0.8f)      // Azul brillante
        };
        
        public static readonly Vector3[] MARCOS_MONITOR = {
            new(0.1f, 0.1f, 0.1f),     // Negro
            new(0.15f, 0.12f, 0.1f),   // Marrón oscuro
            new(0.12f, 0.12f, 0.15f)   // Azul oscuro
        };
        
        public static readonly Vector3[] DISPLAYS_MONITOR = {
            new(0.1f, 0.2f, 0.4f),     // Azul pantalla
            new(0.2f, 0.1f, 0.3f),     // Púrpura pantalla
            new(0.1f, 0.3f, 0.2f)      // Verde pantalla
        };
        
        public static readonly Vector3[] BASES_TECLADO = {
            new(0.2f, 0.2f, 0.2f),     // Gris oscuro
            new(0.15f, 0.15f, 0.2f),   // Azul oscuro
            new(0.2f, 0.15f, 0.15f)    // Rojizo oscuro
        };
        
        public static readonly Vector3[] TECLAS_LATERAL = {
            new(0.85f, 0.85f, 0.85f),  // Gris claro
            new(0.8f, 0.85f, 0.9f),    // Azul claro
            new(0.9f, 0.85f, 0.8f)     // Rojizo claro
        };
        
        public static readonly Vector3[] TECLAS_SUPERIOR = {
            new(0.95f, 0.95f, 0.95f),  // Blanco
            new(0.9f, 0.95f, 1.0f),    // Azul muy claro
            new(1.0f, 0.95f, 0.9f)     // Rojizo muy claro
        };
        
        // Colores generales
        public static readonly Vector3 VENTILADOR = new(0.3f, 0.3f, 0.3f);
        public static readonly Vector3 PUERTO_USB = new(0f, 0f, 0f);
        public static readonly Vector3 BASE_MONITOR = new(0.6f, 0.6f, 0.7f);
        public static readonly Vector3 SOPORTE_MONITOR = new(0.3f, 0.3f, 0.3f);
        
        public static void ValidarConsistenciaColores()
        {
            int longitudReferencia = CARCASAS_PC.Length;
            
            if (LEDS_PC.Length != longitudReferencia)
                throw new InvalidOperationException($"LEDS_PC tiene {LEDS_PC.Length} elementos, esperados {longitudReferencia}");
                
            if (MARCOS_MONITOR.Length != longitudReferencia)
                throw new InvalidOperationException($"MARCOS_MONITOR tiene {MARCOS_MONITOR.Length} elementos, esperados {longitudReferencia}");
                
            if (DISPLAYS_MONITOR.Length != longitudReferencia)
                throw new InvalidOperationException($"DISPLAYS_MONITOR tiene {DISPLAYS_MONITOR.Length} elementos, esperados {longitudReferencia}");
                
            if (BASES_TECLADO.Length != longitudReferencia)
                throw new InvalidOperationException($"BASES_TECLADO tiene {BASES_TECLADO.Length} elementos, esperados {longitudReferencia}");
                
            if (TECLAS_LATERAL.Length != longitudReferencia)
                throw new InvalidOperationException($"TECLAS_LATERAL tiene {TECLAS_LATERAL.Length} elementos, esperados {longitudReferencia}");
                
            if (TECLAS_SUPERIOR.Length != longitudReferencia)
                throw new InvalidOperationException($"TECLAS_SUPERIOR tiene {TECLAS_SUPERIOR.Length} elementos, esperados {longitudReferencia}");
        }
    }
    
    // Posiciones en la escena - Layout espacial
    public static class Escena
    {
        public const float SEPARACION_OBJETOS = 6.0f;
        public const float OFFSET_INICIAL_X = -6.0f;
        
        // Posiciones Y por tipo de objeto
        public const float Y_PCS = 0.0f;
        public const float Y_MONITORES = 1.2f;
        public const float Y_TECLADOS = -0.05f;
        
        // Posiciones Z por tipo de objeto
        public const float Z_PCS = -3.0f;
        public const float Z_MONITORES = -5.5f;
        public const float Z_TECLADOS = -0.5f;
        
        // Rotaciones base
        public const float ROTACION_PC_BASE = 0.1f;
        public const float ROTACION_MONITOR_BASE = 0.05f;
        public const float ROTACION_TECLADO_BASE = 0.0f;
    }
    
    // Configuración de cámara
    public static class Camara
    {
        public static readonly Vector3 POSICION_INICIAL = new(0, 6f, 8f);
        public static readonly Vector3 OBJETIVO_INICIAL = new(0, 1f, -2f);
        public const float VELOCIDAD_BASE = 5f;
        public const float FOV_GRADOS = 45f;
        public const float Z_NEAR = 0.1f;
        public const float Z_FAR = 100f;
    }
    
    // Validación y tolerancias
    public static class Validacion
    {
        public const float TOLERANCIA_POSICION = 0.001f;
        public const int MIN_VERTICES_CARA = 3;
        public const int MIN_CARAS_PARTE = 1;
        public const int MIN_PARTES_OBJETO = 1;
    }

    public static void ValidarTodasLasConstantes()
    {
        Monitor.ValidarProporcionesMonitor();
        Colores.ValidarConsistenciaColores();
    }
}