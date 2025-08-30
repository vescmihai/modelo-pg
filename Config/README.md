# SISTEMA 3D SIN HARDCODEO - COORDENADAS RELATIVAS

## OBJETIVOS CUMPLIDOS

### 1. Eliminación Completa de Hardcodeo
- Todas las constantes centralizadas en `ConstantesGeometria.cs`
- Configuraciones generadas desde constantes únicamente
- Validación automática de consistencia entre objetos
- Sin valores mágicos en el código

### 2. Coordenadas Relativas
- Sin solapamiento: Cada objeto mantiene geometría independiente
- Centro de masa (0,0,0): Todos los vértices relativos al origen del objeto  
- Múltiples instancias: Un objeto se renderiza en N posiciones diferentes
- Sin redefinición: La geometría se reutiliza completamente

### 3. Configuración Externa Robusta
- Sin hardcodeo: Geometría, colores y propiedades en archivos JSON
- Carga dinámica: Sistema robusto de lectura/escritura
- Validación automática: Verificación de consistencia
- Flexibilidad total: Crear objetos nuevos modificando solo constantes

### 4. Estructura Jerárquica Completa
```
Objeto → Partes → Caras → Vértices
```
- Centro de masa jerárquico
- Transformaciones relativas
- Animación independiente por partes
- Todas las dimensiones desde constantes

## CONSTANTES CENTRALIZADAS

### Monitor
- Dimensiones: Marco 1.6x0.9, Display 1.4x0.7
- Base: Radio 0.6, 8 segmentos
- Colores: 3 variantes predefinidas

### PC
- Carcasa: 1.0x2.0x1.6 unidades
- Ventilador: Radio 0.3, 6 aspas
- LED: 0.4x0.08 unidades
- Colores: 3 variantes predefinidas

### Teclado  
- Base: 4.0x0.1x1.6 unidades
- Teclas: 4 filas x 10 columnas
- Espaciado: 0.4 unidades
- Colores: 3 variantes predefinidas

## CONTROLES
- ESC - Salir
- WASD - Mover cámara  
- Q/E - Subir/Bajar cámara
- R - Rotar primer PC
- L - Cambiar color LED
- V - Acelerar ventilador  
- T - Toggle animaciones
- 1-3 - Seleccionar PC/Monitor/Teclado
- P - Mostrar estadísticas
- G - Guardar configuración
- C - Reiniciar cámara

## ARCHIVOS MODIFICADOS
- `ConstantesGeometria.cs` - NUEVO: Todas las constantes centralizadas
- `Program.cs` - MODIFICADO: Generación desde constantes únicamente
- `Objeto.cs` - MODIFICADO: Sin hardcodeo, carga obligatoria desde JSON
- `Game.cs` - MODIFICADO: Posicionamiento desde constantes de escena
- `Parte.cs` - MODIFICADO: Validación mejorada
- `Cara.cs` - MODIFICADO: Validación con constantes
- `Vertice.cs` - SIN CAMBIOS: Ya funcionaba correctamente
- `DatosJSON.cs` - SIN CAMBIOS: Estructuras correctas

## VALIDACIÓN AUTOMÁTICA
El sistema valida automáticamente:
- Consistencia dimensional entre objetos del mismo tipo
- Número correcto de vértices, caras y partes  
- Integridad de índices y geometría
- Tolerancia de posición: ±0.001 unidades

## PARA CAMBIAR DIMENSIONES
Modificar solo el archivo `ConstantesGeometria.cs` y regenerar configuraciones.
