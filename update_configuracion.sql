-- Permitir valores NULL en los campos Clave y Valor
PRAGMA foreign_keys=off;

BEGIN TRANSACTION;

-- 1. Crear una tabla temporal con la nueva estructura
CREATE TABLE Configuraciones_temp (
    Id INTEGER NOT NULL CONSTRAINT "PK_Configuraciones" PRIMARY KEY AUTOINCREMENT,
    Clave TEXT,
    Valor TEXT,
    Descripcion TEXT,
    FechaCreacion TEXT NOT NULL,
    UltimaModificacion TEXT,
    NombreTienda TEXT NOT NULL DEFAULT 'Ferretería Las Naciones',
    DireccionTienda TEXT,
    TelefonoTienda TEXT,
    UsarImpresoraTermica INTEGER NOT NULL DEFAULT 0,
    ImpresoraTermicaNombre TEXT,
    UsarPuertoSerie INTEGER NOT NULL DEFAULT 0,
    PuertoSerieNombre TEXT,
    PuertoSerieBaudRate INTEGER NOT NULL DEFAULT 9600,
    PuertoSerieDataBits INTEGER NOT NULL DEFAULT 8,
    PuertoSerieParity TEXT NOT NULL DEFAULT 'None',
    PuertoSerieStopBits TEXT NOT NULL DEFAULT 'One',
    ImprimirTicketAutomatico INTEGER NOT NULL DEFAULT 1,
    AnchoPapel INTEGER NOT NULL DEFAULT 48,
    MensajeFinalTicket TEXT NOT NULL,
    ImprimirCopia INTEGER NOT NULL,
    UsaPuertoSerial INTEGER NOT NULL,
    PuertoSerial TEXT,
    BaudRate INTEGER NOT NULL
);

-- 2. Copiar datos existentes (si los hay)
INSERT OR IGNORE INTO Configuraciones_temp 
SELECT * FROM Configuraciones;

-- 3. Eliminar la tabla original
DROP TABLE IF EXISTS Configuraciones;

-- 4. Renombrar la tabla temporal
ALTER TABLE Configuraciones_temp RENAME TO Configuraciones;

-- 5. Insertar una configuración predeterminada si no existe ninguna
INSERT OR IGNORE INTO Configuraciones (
    Clave, Valor, Descripcion, FechaCreacion, UltimaModificacion,
    NombreTienda, DireccionTienda, TelefonoTienda, 
    UsarImpresoraTermica, ImprimirTicketAutomatico, AnchoPapel, MensajeFinalTicket, ImprimirCopia, UsaPuertoSerial, PuertoSerial, BaudRate
)
SELECT 
    'configuracion_general', '{}', 'Configuración general del sistema', 
    datetime('now'), datetime('now'),
    'Ferretería Las Naciones', 'Dirección de la tienda', 'Teléfono de la tienda',
    0, 1, 48, 'Mensaje final del ticket', 1, 0, '', 9600
WHERE NOT EXISTS (SELECT 1 FROM Configuraciones);

-- Verificar si existe la columna AnchoPapel
SELECT * FROM pragma_table_info('Configuraciones') WHERE name = 'AnchoPapel';

-- Añadir la columna AnchoPapel si no existe
ALTER TABLE Configuraciones ADD COLUMN AnchoPapel INTEGER NOT NULL DEFAULT 48;

COMMIT;

PRAGMA foreign_keys=on; 