# Especificaciones de Impresora Térmica SPRT 80mm

## Información Básica

- **Modelo:** 80mm Series Printer
- **Ancho de papel:** 80mm (3.15 pulgadas)
- **Ancho de impresión efectivo:** 576 puntos (72mm) @ 203 DPI
- **Interfaces:** USB (Virtual COM Port o Printer Driver)
- **Comandos:** Compatible ESC/POS

## Configuración de Hardware

### Método de Conexión Recomendado
- **Windows Printer Driver:** Instalación del driver oficial/genérico de Windows
- Aparece como "80mm Series Printer" en el panel de impresoras del sistema

### Requerimientos de Alimentación
- Voltaje: DC 24V / 2.5A
- Potencia: 60W

## Comandos ESC/POS Específicos

### Comandos Básicos
| Función | Comando (HEX) | Descripción |
|---------|---------------|-------------|
| Inicializar Impresora | 1B 40 | Inicializa la impresora y limpia el buffer |
| Salto de línea | 0A | Avanza una línea |
| Alimentar papel N líneas | 1B 64 n | Avanza 'n' líneas de papel |
| Retroceder papel N líneas | 1B 65 n | Retrocede 'n' líneas de papel (si lo soporta) |

### Formato de Texto
| Función | Comando (HEX) | Descripción |
|---------|---------------|-------------|
| Alineación izquierda | 1B 61 00 | Alinea el texto a la izquierda |
| Alineación centrada | 1B 61 01 | Centra el texto |
| Alineación derecha | 1B 61 02 | Alinea el texto a la derecha |
| Negrita activada | 1B 45 01 | Activa texto en negrita |
| Negrita desactivada | 1B 45 00 | Desactiva texto en negrita |
| Subrayado activado | 1B 2D 01 | Activa texto subrayado sencillo |
| Subrayado desactivado | 1B 2D 00 | Desactiva texto subrayado |
| Doble altura activada | 1B 21 10 | Activa texto de doble altura |
| Doble altura desactivada | 1B 21 00 | Desactiva texto de doble altura |

### Corte de Papel
| Función | Comando (HEX) | Descripción |
|---------|---------------|-------------|
| Corte total | 1D 56 41 | Realiza un corte completo del papel |
| Corte parcial | 1D 56 42 | Realiza un corte parcial (dejando un punto sin cortar) |

### Control de Cajón de Dinero
| Función | Comando (HEX) | Descripción |
|---------|---------------|-------------|
| Abrir cajón, pin 2 | 1B 70 00 25 250 | Activa el pin 2 durante 250ms |
| Abrir cajón, pin 5 | 1B 70 01 25 250 | Activa el pin 5 durante 250ms |

### Códigos de Barras
| Función | Comando (HEX) | Descripción |
|---------|---------------|-------------|
| Código de barras EAN-13 | 1D 6B 02 ... | Imprime código de barras EAN-13 |
| Código de barras CODE39 | 1D 6B 04 ... | Imprime código de barras CODE39 |
| Código de barras CODE128 | 1D 6B 49 ... | Imprime código de barras CODE128 |
| QR Code | 1D 28 6B ... | Imprime código QR (secuencia compleja) |

### Impresión de Imágenes
| Función | Comando (HEX) | Descripción |
|---------|---------------|-------------|
| Impresión de Bitmap | 1D 76 30 ... | Imprime una imagen en formato raster |

## Configuración Recomendada

### Impresora
- Página de códigos: CP850 (Europeo occidental)
- Velocidad de impresión: Normal (180mm/seg.)
- Densidad de impresión: Media-Alta
- Corte automático: Habilitado
- Modo económico: Deshabilitado para mejor calidad

### Cajón de Dinero (si se utiliza)
- Conexión: RJ11 a impresora
- Método de apertura: Pin 2 (verificar si funciona, si no, probar pin 5)
- Duración de pulso: 250ms

## Solución de Problemas

### Papel se atasca
1. Verificar que se esté utilizando papel térmico de 80mm de buena calidad
2. Comprobar que el rodillo esté limpio y sin obstrucciones
3. Verificar que el cabezal de impresión esté limpio

### No imprime o imprime caracteres extraños
1. Reiniciar la impresora
2. Comprobar la conexión USB
3. Reinstalar driver si es necesario
4. Verificar que la página de códigos configurada sea la correcta

### Cajón de dinero no abre
1. Verificar conexión física entre cajón e impresora
2. Probar ambos pines (2 y 5) para ver cuál funciona con el cajón
3. Comprobar si el cajón requiere alimentación externa

## Recursos Adicionales
- **Manual de usuario completo:** Disponible en el directorio `docs/printer`
- **Driver oficial:** Descargar de la web del fabricante o usar Windows Update
- **Soporte técnico:** Contactar al proveedor local 