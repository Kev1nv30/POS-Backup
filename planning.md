PLANNING

1. Visión General

Desarrollar un sistema POS de escritorio para Ferretería Las Naciones, inspirado en Eleventa (versión mexicana), que integre de forma modular:

Gestión de productos e inventario

Registro básico de clientes y historial de compras

Ventas rápidas con emisión de tickets y facturación electrónica opcional

Integración de aplicaciones existentes: OCR de facturas proveedores, motor de precios, bodega digital 3D, generador de códigos EAN‑13

El objetivo es centralizar todas las herramientas en una sola aplicación WPF, fácil de usar y operable prácticamente solo con teclado.

2. Arquitectura y Módulos

Core POS

Ventana de Productos (CRUD e inventario)

Ventana de Clientes (CRUD + historial)

Ventana de Ventas (ticketing, toggles de factura)

Facturación Electrónica

Servicio SimpleAPI integrado como módulo separado

BackgroundWorker para lote de facturas pendientes

Integraciones Externas

OCR de Proveedores: microservicio Python con API REST

Pricing Engine: librería interna C# o servicio Python

Bodega 3D: WebView2 con p5.js

Barcode Generator: servicio Python o biblioteca .NET

Base de Datos

SQLite embebido (o SQL Server Express para multi‑caja en red)

EF Core para ORM y migraciones

3. Restricciones y Requisitos

Plataforma objetivo: Windows 10/11

Usuario final: adultos mayores, con atención mínima al mouse

Rendimiento: arranque < 5s, UI responsiva sin demoras perceptibles

Mantenimiento: auto‑deploy con instalador Auto‑Update

Seguridad: almacenamiento local seguro, respaldo automático de BD

4. Tecnologías y Herramientas

Lenguaje: C# .NET 8

UI: WPF (.NET 8)

ORM: Entity Framework Core + SQLite

Facturación: SimpleAPI SDK (.NET) o REST

Reporting/PDF: QuestPDF o iText7

OCR: Python + Google Document AI expuesto vía FastAPI

Pricing: Migración a .NET o FastAPI Python

Bodega 3D: p5.js + WebView2

Código de barras: BarcodeLib (.NET) o Python EAN13 + FastAPI

Control de versiones: Git (GitHub)

Tareas y planning: PLAN​NING.md y TASK.md en repositorio

5. Fases del Proyecto

Sprint 1: Modelo de datos + interfaz inicial Productos (ver tabla) + seed visual

Sprint 2: CRUD completo Productos, alertas de stock

Sprint 3: Ventana Clientes y historial de compras

Sprint 4: Ventana Ventas y ticketing con toggle factura

Sprint 5: Módulo Facturación electrónica SimpleAPI

Sprints Extra: Integración OCR, Pricing, Bodega 3D, Barcode Generator

Sprint Final: QA, pruebas E2E, instalador y despliegue

6. Documentos de Referencia

Este archivo PLANNING.md referenciado en cada nueva conversación

TASK.md para seguimiento diario de tareas