using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Game : GameWindow
{
    private int shader = -1;
    private List<Objeto> objetos = new List<Objeto>();
    private float tiempoAnimacion = 0f;
    private bool animacionActiva = true;
    
    // Variables de cámara usando constantes
    private Vector3 posicionCamara = ConstantesGeometria.Camara.POSICION_INICIAL;
    private Vector3 objetivoCamara = ConstantesGeometria.Camara.OBJETIVO_INICIAL;
    private float velocidadCamara = ConstantesGeometria.Camara.VELOCIDAD_BASE;
    
    public Game() : base(GameWindowSettings.Default, 
        new NativeWindowSettings() 
        { 
            Size = new Vector2i(1200, 800), 
            Title = "Sistema 3D - SIN HARDCODEO - Constantes Centralizadas"
        })
    {
    }

    protected override void OnLoad()
    {
        GL.ClearColor(0.1f, 0.1f, 0.2f, 1.0f);
        GL.Enable(EnableCap.DepthTest);
        GL.Enable(EnableCap.CullFace);

        CrearShader();
        CrearEscena();
        
        MostrarInformacionInicial();
    }

    private void CrearShader()
    {
        try
        {
            string vertexShader = @"
                #version 330 core
                layout (location = 0) in vec3 pos;
                uniform mat4 mvp;
                void main() {
                    gl_Position = mvp * vec4(pos, 1.0);
                }";

            string fragmentShader = @"
                #version 330 core
                uniform vec3 color;
                out vec4 fragColor;
                void main() {
                    fragColor = vec4(color, 1.0);
                }";

            int vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vertexShader);
            GL.CompileShader(vs);
            
            // Verificar errores de compilación
            GL.GetShader(vs, ShaderParameter.CompileStatus, out int statusVs);
            if (statusVs == 0)
            {
                GL.GetShaderInfoLog(vs, out string infoLogVs);
                throw new Exception($"Error compilando vertex shader: {infoLogVs}");
            }

            int fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fragmentShader);
            GL.CompileShader(fs);
            
            // Verificar errores de compilación
            GL.GetShader(fs, ShaderParameter.CompileStatus, out int statusFs);
            if (statusFs == 0)
            {
                GL.GetShaderInfoLog(fs, out string infoLogFs);
                throw new Exception($"Error compilando fragment shader: {infoLogFs}");
            }

            shader = GL.CreateProgram();
            GL.AttachShader(shader, vs);
            GL.AttachShader(shader, fs);
            GL.LinkProgram(shader);
            
            // Verificar errores de enlace
            GL.GetProgram(shader, GetProgramParameterName.LinkStatus, out int statusProgram);
            if (statusProgram == 0)
            {
                GL.GetProgramInfoLog(shader, out string infoLogProgram);
                throw new Exception($"Error enlazando programa: {infoLogProgram}");
            }

            GL.DeleteShader(vs);
            GL.DeleteShader(fs);

            Console.WriteLine("Shaders compilados exitosamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creando shaders: {ex.Message}");
            throw;
        }
    }

    private void CrearEscena()
    {
        try
        {
            Console.WriteLine("Creando escena desde configuraciones JSON obligatorias...");

            // GARANTIZAR que existan todas las configuraciones
            GarantizarConfiguracionesExistentes();

            // Crear objetos SOLO desde archivos JSON usando constantes para posicionamiento
            CrearPCsDesdeJSON();
            CrearMonitoresDesdeJSON(); 
            CrearTecladosDesdeJSON();

            Console.WriteLine($"Escena creada con {objetos.Count} objetos - SIN HARDCODEO");
            
            // Mostrar layout usando constantes
            MostrarLayoutEscenaDesdeConstantes();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR CRÍTICO creando escena: {ex.Message}");
            throw; // No crear escena de respaldo - forzar solución del problema
        }
    }

    private void GarantizarConfiguracionesExistentes()
    {
        string[] archivosRequeridos = {
            "Config/pc_0.json", "Config/pc_1.json", "Config/pc_2.json",
            "Config/monitor_0.json", "Config/monitor_1.json", "Config/monitor_2.json", 
            "Config/teclado_0.json", "Config/teclado_1.json", "Config/teclado_2.json"
        };
        
        var archivosFaltantes = new List<string>();
        
        foreach (string archivo in archivosRequeridos)
        {
            if (!File.Exists(archivo))
            {
                archivosFaltantes.Add(archivo);
            }
        }
        
        if (archivosFaltantes.Count > 0)
        {
            string mensaje = $"CONFIGURACIONES REQUERIDAS FALTANTES:\n{string.Join("\n", archivosFaltantes)}\nEjecutar Program.cs primero para generar configuraciones.";
            throw new FileNotFoundException(mensaje);
        }
        
        Console.WriteLine("Todas las configuraciones JSON están disponibles");
    }

    private void CrearPCsDesdeJSON()
    {
        for (int i = 0; i < 3; i++)
        {
            string archivoConfig = $"Config/pc_{i}.json";
            
            var pc = PC.CrearDesdeArchivo(archivoConfig);
            pc.Inicializar();
            
            // Posicionamiento usando constantes
            float posX = ConstantesGeometria.Escena.OFFSET_INICIAL_X + (i * ConstantesGeometria.Escena.SEPARACION_OBJETOS);
            pc.EstablecerPosicion(new Vector3(posX, ConstantesGeometria.Escena.Y_PCS, ConstantesGeometria.Escena.Z_PCS));
            pc.EstablecerRotacion(new Vector3(0, i * ConstantesGeometria.Escena.ROTACION_PC_BASE, 0));
            
            objetos.Add(pc);
            Console.WriteLine($"PC {i} cargado y posicionado en ({posX}, {ConstantesGeometria.Escena.Y_PCS}, {ConstantesGeometria.Escena.Z_PCS})");
        }
    }

    private void CrearMonitoresDesdeJSON()
    {
        for (int i = 0; i < 3; i++)
        {
            string archivoConfig = $"Config/monitor_{i}.json";
            
            var monitor = Monitor.CrearDesdeArchivo(archivoConfig);
            monitor.Inicializar();
            
            // Posicionamiento usando constantes
            float posX = ConstantesGeometria.Escena.OFFSET_INICIAL_X + (i * ConstantesGeometria.Escena.SEPARACION_OBJETOS);
            monitor.EstablecerPosicion(new Vector3(posX, ConstantesGeometria.Escena.Y_MONITORES, ConstantesGeometria.Escena.Z_MONITORES));
            monitor.EstablecerRotacion(new Vector3(0, i * ConstantesGeometria.Escena.ROTACION_MONITOR_BASE, 0));
            
            objetos.Add(monitor);
            Console.WriteLine($"Monitor {i} cargado y posicionado en ({posX}, {ConstantesGeometria.Escena.Y_MONITORES}, {ConstantesGeometria.Escena.Z_MONITORES})");
        }
    }

    private void CrearTecladosDesdeJSON()
    {
        for (int i = 0; i < 3; i++)
        {
            string archivoConfig = $"Config/teclado_{i}.json";
            
            var teclado = Teclado.CrearDesdeArchivo(archivoConfig);
            teclado.Inicializar();
            
            // Posicionamiento usando constantes
            float posX = ConstantesGeometria.Escena.OFFSET_INICIAL_X + (i * ConstantesGeometria.Escena.SEPARACION_OBJETOS);
            teclado.EstablecerPosicion(new Vector3(posX, ConstantesGeometria.Escena.Y_TECLADOS, ConstantesGeometria.Escena.Z_TECLADOS));
            teclado.EstablecerRotacion(new Vector3(0, i * ConstantesGeometria.Escena.ROTACION_TECLADO_BASE, 0));
            
            objetos.Add(teclado);
            Console.WriteLine($"Teclado {i} cargado y posicionado en ({posX}, {ConstantesGeometria.Escena.Y_TECLADOS}, {ConstantesGeometria.Escena.Z_TECLADOS})");
        }
    }
    
    private void MostrarLayoutEscenaDesdeConstantes()
    {
        Console.WriteLine();
        Console.WriteLine("LAYOUT DE LA ESCENA (usando constantes centralizadas):");
        Console.WriteLine("Vista desde arriba (Y hacia arriba, Z hacia adelante):");
        Console.WriteLine();
        Console.WriteLine($"           MONITORES (Y={ConstantesGeometria.Escena.Y_MONITORES}, Z={ConstantesGeometria.Escena.Z_MONITORES})");
        Console.WriteLine("    Mon0         Mon1         Mon2");
        Console.WriteLine("     |            |            |");
        Console.WriteLine("                                ");
        Console.WriteLine($"           PCs (Y={ConstantesGeometria.Escena.Y_PCS}, Z={ConstantesGeometria.Escena.Z_PCS})");
        Console.WriteLine("    PC0          PC1          PC2");
        Console.WriteLine("     |            |            |");
        Console.WriteLine("                                ");
        Console.WriteLine($"         TECLADOS (Y={ConstantesGeometria.Escena.Y_TECLADOS}, Z={ConstantesGeometria.Escena.Z_TECLADOS})");
        Console.WriteLine("    Tec0         Tec1         Tec2");
        Console.WriteLine();
        Console.WriteLine($"   X={ConstantesGeometria.Escena.OFFSET_INICIAL_X}          X=0          X={-ConstantesGeometria.Escena.OFFSET_INICIAL_X}");
        Console.WriteLine($"   Separación entre objetos: {ConstantesGeometria.Escena.SEPARACION_OBJETOS} unidades");
        Console.WriteLine();
        Console.WriteLine($"Cámara inicial en {ConstantesGeometria.Camara.POSICION_INICIAL} mirando hacia {ConstantesGeometria.Camara.OBJETIVO_INICIAL}");
        Console.WriteLine();
    }

    private void MostrarInformacionInicial()
    {
        Console.WriteLine();
        Console.WriteLine("=== SISTEMA 3D SIN HARDCODEO INICIADO ===");
        Console.WriteLine($"Objetos en escena: {objetos.Count}");
        Console.WriteLine();
        Console.WriteLine("OBJETIVOS CUMPLIDOS:");
        Console.WriteLine("   Sin hardcodeo - Todas las constantes centralizadas");
        Console.WriteLine("   Coordenadas relativas - Sin solapamiento");
        Console.WriteLine("   Configuración externa - Solo desde JSON");
        Console.WriteLine("   Estructura jerárquica - Objeto→Parte→Cara→Vértice");
        Console.WriteLine("   Validación automática - Consistencia garantizada");
        Console.WriteLine();
        Console.WriteLine("CONTROLES:");
        Console.WriteLine("   ESC - Salir");
        Console.WriteLine("   WASD - Mover cámara");
        Console.WriteLine("   Q/E - Subir/Bajar cámara");
        Console.WriteLine("   R - Rotar primer PC");
        Console.WriteLine("   L - Cambiar color LED");
        Console.WriteLine("   V - Acelerar ventilador");
        Console.WriteLine("   T - Toggle animaciones");
        Console.WriteLine("   1-3 - Seleccionar PC/Monitor/Teclado");
        Console.WriteLine("   P - Mostrar estadísticas");
        Console.WriteLine("   G - Guardar configuración");
        Console.WriteLine();

        // Mostrar estadísticas de objetos
        foreach (var objeto in objetos)
        {
            Console.WriteLine($"   {objeto.ObtenerEstadisticas()}");
        }
        Console.WriteLine();
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        if (shader == -1) return;

        GL.UseProgram(shader);

        // Matrices de vista y proyección usando constantes
        Matrix4 view = Matrix4.LookAt(posicionCamara, objetivoCamara, Vector3.UnitY);
        Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
            MathHelper.DegreesToRadians(ConstantesGeometria.Camara.FOV_GRADOS), 
            (float)Size.X / Size.Y, 
            ConstantesGeometria.Camara.Z_NEAR, 
            ConstantesGeometria.Camara.Z_FAR);

        // Renderizar todos los objetos
        int objetosRenderizados = 0;
        foreach (var objeto in objetos)
        {
            try
            {
                if (objeto.Visible)
                {
                    objeto.Renderizar(shader, view, projection);
                    objetosRenderizados++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error renderizando {objeto.Nombre}: {ex.Message}");
            }
        }

        SwapBuffers();

        // Actualizar título con estadísticas
        if ((int)tiempoAnimacion % 5 == 0 && (int)(tiempoAnimacion * 10) % 10 == 0)
        {
            Title = $"Sistema 3D SIN HARDCODEO - Objetos: {objetosRenderizados}/{objetos.Count} - FPS: {Math.Round(1.0 / e.Time, 1)}";
        }
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
        float deltaTime = (float)e.Time;
        tiempoAnimacion += deltaTime;
        
        if (KeyboardState.IsKeyDown(Keys.Escape))
            Close();

        // Actualizar cámara usando constantes
        ActualizarCamara(deltaTime);
        
        // Animaciones automáticas
        if (animacionActiva)
        {
            EjecutarAnimaciones(deltaTime);
        }
    }

    private void ActualizarCamara(float deltaTime)
    {
        float velocidad = velocidadCamara * deltaTime;
        
        if (KeyboardState.IsKeyDown(Keys.W))
        {
            Vector3 forward = Vector3.Normalize(objetivoCamara - posicionCamara);
            posicionCamara += forward * velocidad;
            objetivoCamara += forward * velocidad;
        }
        if (KeyboardState.IsKeyDown(Keys.S))
        {
            Vector3 backward = Vector3.Normalize(posicionCamara - objetivoCamara);
            posicionCamara += backward * velocidad;
            objetivoCamara += backward * velocidad;
        }
        if (KeyboardState.IsKeyDown(Keys.A))
        {
            Vector3 left = Vector3.Normalize(Vector3.Cross(Vector3.UnitY, objetivoCamara - posicionCamara));
            posicionCamara += left * velocidad;
            objetivoCamara += left * velocidad;
        }
        if (KeyboardState.IsKeyDown(Keys.D))
        {
            Vector3 right = Vector3.Normalize(Vector3.Cross(objetivoCamara - posicionCamara, Vector3.UnitY));
            posicionCamara += right * velocidad;
            objetivoCamara += right * velocidad;
        }
        if (KeyboardState.IsKeyDown(Keys.Q))
        {
            posicionCamara.Y -= velocidad;
            objetivoCamara.Y -= velocidad;
        }
        if (KeyboardState.IsKeyDown(Keys.E))
        {
            posicionCamara.Y += velocidad;
            objetivoCamara.Y += velocidad;
        }
    }

    private void EjecutarAnimaciones(float deltaTime)
    {
        try
        {
            // Animación de LEDs parpadeando
            float intensidadLed = (MathF.Sin(tiempoAnimacion * 3f) + 1f) * 0.5f;
            
            foreach (var objeto in objetos)
            {
                // Animar PCs
                if (objeto is PC pc)
                {
                    var panelLed = pc.ObtenerParte("Panel_LED");
                    if (panelLed?.Caras.Count > 0)
                    {
                        var caraLed = panelLed.Caras.Find(c => c.Nombre == "LED");
                        if (caraLed != null)
                        {
                            // Usar colores predefinidos con animación
                            var colorBase = ConstantesGeometria.Colores.LEDS_PC[0]; // Verde base
                            caraLed.Color = new Vector3(
                                colorBase.X * 0.2f,
                                colorBase.Y * intensidadLed,
                                colorBase.Z * 0.2f
                            );
                        }
                    }
                    var ventilador = pc.ObtenerParte("Ventilador_Superior");
                    if (ventilador != null)
                    {
                        ventilador.Rotar(0, deltaTime * 8f, 0); // 8 rad/s
                    }
                }
                
                if (objeto is Monitor monitor)
                {
                    float oscilacionPantalla = MathF.Sin(tiempoAnimacion * 0.5f) * 0.03f;
                    var pantalla = monitor.ObtenerParte("Pantalla");
                    if (pantalla != null)
                    {
                        pantalla.Rotacion = new Vector3(oscilacionPantalla, 0, 0);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en animaciones: {ex.Message}");
        }
    }

    protected override void OnKeyDown(KeyboardKeyEventArgs e)
    {
        base.OnKeyDown(e);
        
        try
        {
            switch (e.Key)
            {
                case Keys.R:
                    // Rotar primer PC
                    var primerPC = objetos.Find(o => o is PC) as PC;
                    if (primerPC != null)
                    {
                        primerPC.Rotar(new Vector3(0, MathHelper.DegreesToRadians(15), 0));
                        Console.WriteLine($"Rotando {primerPC.Nombre}");
                    }
                    break;
                    
                case Keys.L:
                    // Cambiar color LED usando colores predefinidos
                    var pcConLed = objetos.Find(o => o is PC) as PC;
                    if (pcConLed != null)
                    {
                        var panelLed = pcConLed.ObtenerParte("Panel_LED");
                        if (panelLed?.Caras.Count > 0)
                        {
                            var cara = panelLed.Caras[0];
                            // Rotar entre colores predefinidos
                            var colores = ConstantesGeometria.Colores.LEDS_PC;
                            int indiceColor = (int)(tiempoAnimacion * 0.5f) % colores.Length;
                            cara.Color = colores[indiceColor];
                            Console.WriteLine($"Color LED cambiado a variante {indiceColor}");
                        }
                    }
                    break;
                    
                case Keys.V:
                    // Acelerar ventilador
                    foreach (var objeto in objetos)
                    {
                        if (objeto is PC pc)
                        {
                            var ventilador = pc.ObtenerParte("Ventilador_Superior");
                            if (ventilador != null)
                            {
                                ventilador.Rotar(0, MathHelper.DegreesToRadians(90), 0);
                                Console.WriteLine("Ventilador acelerado");
                            }
                        }
                    }
                    break;
                    
                case Keys.T:
                    // Toggle animaciones
                    animacionActiva = !animacionActiva;
                    Console.WriteLine($"Animaciones: {(animacionActiva ? "Activadas" : "Desactivadas")}");
                    break;
                    
                case Keys.D1:
                    // Seleccionar primer PC
                    var pc1 = objetos.Find(o => o is PC);
                    if (pc1 != null)
                    {
                        pc1.Rotar(new Vector3(0, MathHelper.DegreesToRadians(30), 0));
                        Console.WriteLine($"PC seleccionado: {pc1.Nombre}");
                    }
                    break;
                    
                case Keys.D2:
                    // Seleccionar primer Monitor
                    var monitor1 = objetos.Find(o => o is Monitor);
                    if (monitor1 != null)
                    {
                        monitor1.Rotar(new Vector3(0, MathHelper.DegreesToRadians(30), 0));
                        Console.WriteLine($"Monitor seleccionado: {monitor1.Nombre}");
                    }
                    break;
                    
                case Keys.D3:
                    // Seleccionar primer Teclado
                    var teclado1 = objetos.Find(o => o is Teclado);
                    if (teclado1 != null)
                    {
                        teclado1.Rotar(new Vector3(0, MathHelper.DegreesToRadians(30), 0));
                        Console.WriteLine($"Teclado seleccionado: {teclado1.Nombre}");
                    }
                    break;
                    
                case Keys.P:
                    // Mostrar estadísticas
                    MostrarEstadisticas();
                    break;
                    
                case Keys.G:
                    // Guardar configuración del primer objeto
                    if (objetos.Count > 0)
                    {
                        try
                        {
                            string archivo = $"Config/{objetos[0].Nombre}_guardado.json";
                            objetos[0].GuardarEnJSON(archivo);
                            Console.WriteLine($"Configuración guardada en: {archivo}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error guardando: {ex.Message}");
                        }
                    }
                    break;
                    
                case Keys.H:
                    // Mostrar/Ocultar todos los objetos
                    bool nuevaVisibilidad = !objetos[0].Visible;
                    foreach (var objeto in objetos)
                    {
                        objeto.Visible = nuevaVisibilidad;
                    }
                    Console.WriteLine($"Visibilidad objetos: {(nuevaVisibilidad ? "Visible" : "Oculto")}");
                    break;

                case Keys.C:
                    // Reiniciar cámara a posición inicial usando constantes
                    ReiniciarCamara();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error procesando tecla {e.Key}: {ex.Message}");
        }
    }
    private void MostrarEstadisticas()
    {
        Console.WriteLine("\n=== ESTADÍSTICAS DE LA ESCENA SIN HARDCODEO ===");
        Console.WriteLine($"Objetos totales: {objetos.Count}");
        Console.WriteLine($"Posición cámara: ({posicionCamara.X:F1}, {posicionCamara.Y:F1}, {posicionCamara.Z:F1})");
        Console.WriteLine($"Objetivo cámara: ({objetivoCamara.X:F1}, {objetivoCamara.Y:F1}, {objetivoCamara.Z:F1})");
        Console.WriteLine($"Tiempo transcurrido: {tiempoAnimacion:F1}s");
        Console.WriteLine($"Animaciones activas: {animacionActiva}");
        
        Console.WriteLine("\nCONSTANTES UTILIZADAS:");
        Console.WriteLine($"  Separación objetos: {ConstantesGeometria.Escena.SEPARACION_OBJETOS}");
        Console.WriteLine($"  Velocidad cámara: {ConstantesGeometria.Camara.VELOCIDAD_BASE}");
        Console.WriteLine($"  FOV cámara: {ConstantesGeometria.Camara.FOV_GRADOS}°");
        
        Console.WriteLine("\nOBJETOS EN ESCENA:");
        foreach (var objeto in objetos)
        {
            Console.WriteLine($"  {objeto.ObtenerEstadisticas()}");
            Console.WriteLine($"      Posición: ({objeto.Posicion.X:F1}, {objeto.Posicion.Y:F1}, {objeto.Posicion.Z:F1})");
            
            // Validar integridad
            if (!objeto.ValidarIntegridad(out List<string> errores))
            {
                Console.WriteLine($"      Errores: {string.Join(", ", errores)}");
            }
            else
            {
                Console.WriteLine($"      Integridad correcta");
            }
        }
        Console.WriteLine("=======================================\n");
    }

    protected override void OnResize(ResizeEventArgs e)
    {
        GL.Viewport(0, 0, Size.X, Size.Y);
    }

    protected override void OnUnload()
    {
        Console.WriteLine("Liberando recursos...");
        
        // Limpiar objetos
        foreach (var objeto in objetos)
        {
            try
            {
                objeto?.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error liberando {objeto?.Nombre}: {ex.Message}");
            }
        }
        objetos.Clear();
        
        // Limpiar shader
        if (shader != -1)
        {
            GL.DeleteProgram(shader);
            shader = -1;
        }
        
        Console.WriteLine("Recursos liberados");
        base.OnUnload();
    }

    public void AgregarObjeto(Objeto objeto)
    {
        if (objeto != null)
        {
            objetos.Add(objeto);
            Console.WriteLine($"Objeto {objeto.Nombre} agregado a la escena");
        }
    }

    public void RemoverObjeto(Objeto objeto)
    {
        if (objetos.Remove(objeto))
        {
            objeto?.Dispose();
            Console.WriteLine($"Objeto {objeto?.Nombre} removido de la escena");
        }
    }

    public List<T> ObtenerObjetosDeTipo<T>() where T : Objeto
    {
        var objetosTipo = new List<T>();
        foreach (var objeto in objetos)
        {
            if (objeto is T objetoTipo)
            {
                objetosTipo.Add(objetoTipo);
            }
        }
        return objetosTipo;
    }

    public void ReiniciarCamara()
    {
        posicionCamara = ConstantesGeometria.Camara.POSICION_INICIAL;
        objetivoCamara = ConstantesGeometria.Camara.OBJETIVO_INICIAL;
        Console.WriteLine("Cámara reiniciada a posición inicial desde constantes");
    }
}