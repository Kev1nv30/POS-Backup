# Roadmap POS — Ferretería Las Naciones

> **Versión:** 20‑may‑2025 (actualizada)
> **Autor:** ChatGPT (o3) según solicitudes de Kevin
> **Propósito:** Registrar lo hecho y guiar los próximos sprints, priorizando **funcionalidad** > **migraciones de herramientas** > **UI/UX**.

---

## 1. Estado actual (resumen rápido)

| Área                                                               | Avance                                                                                                                  | Evidencia       |
| ------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------------------- | --------------- |
| **Base de datos**                                                  | SQLite con EF Core; migraciones automáticas y *seeding* desde Excel (productos + clientes).                             | `done.md`       |
| **Modelo de dominio**                                              | Entidades completas (Productos, Clientes, Ventas, Inventario, Configuración).                                           | `Core/Models/…` |
| **Servicios**                                                      | CRUD transaccional mediante `RepositoryService`; lógica de stock y ventas en tiempo real.                               | `Services/…`    |
| **UI principal (WPF)**                                             | Ventanas Productos \[F3], Clientes \[F2], Ventas \[F1] y Configuración \[F5].                                           |                 |
| Atajos de teclado activos; cálculo de IVA/total y venta en espera. | Compilación ejecutable                                                                                                  |                 |
| **Impresión de tickets**                                           | Implementado usando `ESC-POS-USB-NET` con clases `EscPosReceiptPrinter` y `ReceiptLayoutBuilder`. | `UI/Printing/`     |
| **Calidad de código**                                              | Limpieza de scripts obsoletos, `.gitignore` actualizado, repositorio GitHub público, documentación `done.md`.           | commit `f1d3…`  |

> **Conclusión:** La app es funcional para ventas **con** ticket físico. Falta integrar facturación electrónica y utilidades externas.

---

## 2. Hoja de ruta detallada

> Sprints ≈ 2 semanas de desarrollo + 1 semana de pruebas cada uno. Las duraciones son orientativas.

### 🟩 Sprint 6 – Impresión de tickets térmica (completado)

**Objetivo:** emitir tickets en impresora SPRT 80 mm con corte de papel y apertura de cajón opcional.

| Nº                                                                            | Tarea                                                                    | Estado | Detalles                                                                                                 |
| ----------------------------------------------------------------------------- | ------------------------------------------------------------------------ | ------ | ----------------------------------------------------------------------------------------------------------------- |
| 6.1                                                                           | **Recolección de requisitos**                                            | ✅ | – Documentados en `PrinterSpecs.md`: ancho 80mm (576px @ 203dpi), comandos ESC/POS para corte y cajón |
| 6.2                                                                           | **Driver & conexión**                                                    | ✅ | – Se utiliza la conexión Windows Printer para la impresora "80mm Series Printer" |
| 6.3                                                                           | **Adaptar `EscPosReceiptPrinter`**                                       | ✅ | – Implementado usando el paquete NuGet `ESC-POS-USB-NET` para comunicación directa |
| 6.4                                                                           | **Generar plantilla de ticket**                                          | ✅ | – `ReceiptLayoutBuilder` implementado con todos los elementos necesarios, incluidos datos de tienda |
| 6.5                                                                           | **Flujo de venta**                                                       | ✅ | – `VentasViewModel.EmitirTicketCommand` integrado con la funcionalidad de impresión |
| 6.6                                                                           | **UI Configuración → Impresora**                                         | ✅ | – Implementada ventana `ConfiguracionImpresoraWindow` con selector de impresora, apertura de cajón y prueba de impresión |
| 6.7                                                                           | **Documentación & fallback**                                             | ✅ | – Manejo gracioso de errores cuando no hay impresora disponible (muestra advertencia) |

### 🟧 Sprint 7 – Migración de utilidades web (OCR, Pricing, EAN‑13)

**Objetivo:** consumir herramientas Python desde la app sin depender de navegador externo.

1. **Arquitectura**
      - Mantener microservicios FastAPI (Docker opcional) en *localhost*.
      - Comunicar vía HTTP + JSON (System.Text.Json).
2. **Módulos a migrar**:
      - **OCR + Precios automáticos** → endpoint `/precio/pdf` (PDF → tabla + precios).
      - **Generador EAN‑13** → endpoint `/ean13?descripcion=…`.
3. **Cliente C#**
      - `ExternalServicesFacade.cs` con métodos `GetPricesAsync()` y `GenerateEanAsync()`.
4. **UI**
      - Ventana **Herramientas** → pestaña "Actualizar precios" (subir PDF, barra de progreso) y "Código de barras".
5. **Pruebas**
      - Mock FastAPI para CI.
      - Casos extremos: producto no emparejado, error de conexión.

### 🟨 Sprint 8 – Facturación electrónica (SimpleAPI)

* Wrapper C# para SDK SimpleAPI (envío y estado).
* Job en segundo plano con reintentos.
* UI Configuración → "Facturas pendientes".
* Ambiente de certificación SII, firma digital.

### 🟩 Sprint 9 – Copia de seguridad, logging y empaquetado

* Ventana Backup/Restore.
* Serilog → archivo diario y rollover.
* Instalador MSIX con BD en `%AppData%`.

### 🟦 Sprint 10 – UI/UX final

* Rediseño visual (íconos, layout flexible 16:9 & 4:3).
* Asistente de alta rápida de producto (escáner + formulario compacto).
* Dark + Light theme.

---

## 3. Tabla de dependencias rápidas

| Depende de                   | Bloquea                       |
| ---------------------------- | ----------------------------- |
| ✅ Driver impresora (6.2)    | ✅ Todas las tareas 6.3‑6.6   |
| Microservicios corriendo (7) | Actualizador de precios en UI |
| Certificado digital (8)      | Envío real al SII             |

---

## 4. Métricas de éxito

* ✅ **TPV operativa 💵 + ticket físico** en caja principal antes del 30‑jun‑2025.
* **Errores de impresión < 1 %** tras 100 ventas.
* **Tiempo promedio de venta** (captura de pago → ticket) **≤ 4 s**.

---

## 5. Próximo paso inmediato

> Arrancar Sprint 7: Desarrollar o migrar los microservicios Python para OCR y generación de códigos de barras, e integrarlos con la aplicación principal.

