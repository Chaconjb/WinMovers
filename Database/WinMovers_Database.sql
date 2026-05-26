-- =====================================================================
-- WinMovers - Script de Creación de Base de Datos
-- Motor: SQL Server
-- Versión: 1.0.0
-- Fecha: 2026-02-25
-- Descripción: Sistema interno de gestión de traslados y mudanzas
-- =====================================================================
-- Módulos implementados:
--   ORD - Órdenes de Trabajo
--   VIS - Control de Visitas
--   EXP - Exportaciones (Checklist de embarque)
--   IMP - Importaciones (Checklist de embarque)
-- =====================================================================
-- Mapeo Modelo JS → Tabla SQL:
--   OrdenTrabajo.model.js   → Ordenes_Trabajo
--   ControlVisitas.model.js → Control_Visitas
--   Exportacion.model.js    → Exportaciones + Exportaciones_Documentos
--   Importacion.model.js    → Importaciones + Importaciones_Documentos
-- =====================================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'WinMoversDB')
BEGIN
    CREATE DATABASE WinMoversDB;
END
GO

USE WinMoversDB;
GO


CREATE TABLE Ordenes_Trabajo (
    id_orden                INT IDENTITY(1,1) PRIMARY KEY,
    numero_ot               NVARCHAR(20)    NOT NULL UNIQUE, 
    fecha_servicio          DATE            NULL,              
    fecha                   DATE            NULL,
    hora                    NVARCHAR(10)    NULL, 
    nombre_cliente          NVARCHAR(150)   NOT NULL,           
    telefono_celular        NVARCHAR(30)    NULL,              
    telefono_residencia     NVARCHAR(30)    NULL,                
    compania                NVARCHAR(150)   NULL,          
    telefono_empresa        NVARCHAR(30)    NULL,              
    contacto                NVARCHAR(150)   NULL,           
    direccion_origen        NVARCHAR(500)   NULL,           
    direccion_destino       NVARCHAR(500)   NULL,                  
    detalle_servicio        NVARCHAR(MAX)   NULL,                 
    materiales              NVARCHAR(MAX)   NULL,              
    facturar_a              NVARCHAR(150)   NULL,                  
    direccion_cobro         NVARCHAR(500)   NULL,                 
    hecho_por               NVARCHAR(100)   NULL,                 
    fecha_creacion          DATETIME2       NOT NULL DEFAULT GETDATE(),  
    fecha_actualizacion     DATETIME2       NULL
);


CREATE TABLE Control_Visitas (
    id_visita               INT IDENTITY(1,1) PRIMARY KEY,
    fecha_llamada           DATE            NULL,                  
    fecha_visita            DATE            NULL,                  
    hora                    NVARCHAR(10)    NULL,                  
    nombre_cliente          NVARCHAR(150)   NOT NULL,              
    telefono_habitacion     NVARCHAR(30)    NULL,                  
    telefono_celular        NVARCHAR(30)    NULL,                  
    empresa                 NVARCHAR(150)   NULL,                  
    telefono_compania       NVARCHAR(30)    NULL,                  
    direccion_origen        NVARCHAR(500)   NULL,                  
    direccion_destino       NVARCHAR(500)   NULL,                  
    observaciones           NVARCHAR(MAX)   NULL,                  
    -- Tipo de servicio (en JS es un array, aquí como BIT individuales)
    puerta_a_puerta         BIT             NOT NULL DEFAULT 0,    
    puerta_a_puerto         BIT             NOT NULL DEFAULT 0,   
    empaque                 BIT             NOT NULL DEFAULT 0,    
    mudanza_local           BIT             NOT NULL DEFAULT 0,    
    -- Campos de cotización
    origen                  NVARCHAR(150)   NULL,                  
    tramites_aduana         NVARCHAR(100)   NULL,                
    flete                   NVARCHAR(100)   NULL,                 
    destino                 NVARCHAR(150)   NULL,                  
    tarifa_total            NVARCHAR(100)   NULL,                 
    compania_maritima       NVARCHAR(100)   NULL,                 
    corresponsal            NVARCHAR(100)   NULL,                  
    hecho_por               NVARCHAR(100)   NULL,                  
    fecha_creacion          DATETIME2       NOT NULL DEFAULT GETDATE(),  
    fecha_actualizacion     DATETIME2       NULL
);
GO

CREATE TABLE Catalogo_Documentos (
    id_tipo_documento       INT IDENTITY(1,1) PRIMARY KEY,
    nombre                  NVARCHAR(150)   NOT NULL,
    aplica_exportacion      BIT             NOT NULL DEFAULT 0,
    aplica_importacion      BIT             NOT NULL DEFAULT 0,
    aplica_winmovers        BIT             NOT NULL DEFAULT 0,
    aplica_otro_agente      BIT             NOT NULL DEFAULT 0,
    orden_presentacion      INT             NOT NULL DEFAULT 0,
    activo                  BIT             NOT NULL DEFAULT 1
);
GO

CREATE TABLE Exportaciones (
    id_exportacion          INT IDENTITY(1,1) PRIMARY KEY,
    nombre_cliente          NVARCHAR(150)   NOT NULL,         
    referencia              NVARCHAR(50)    NULL,           
    fecha                   DATE            NULL,           
    observaciones           NVARCHAR(MAX)   NULL,           
    fecha_creacion          DATETIME2       NOT NULL DEFAULT GETDATE(), 
    fecha_actualizacion     DATETIME2       NULL
);
GO

-- Checklist de documentos de exportación (JS: docsWinMovers + docsOtroAgente)
-- Cada fila = un documento del checklist con su estado completado/pendiente
CREATE TABLE Exportaciones_Documentos (
    id_exp_doc              INT IDENTITY(1,1) PRIMARY KEY,
    id_exportacion          INT             NOT NULL,
    id_tipo_documento       INT             NOT NULL,
    tipo_checklist          NVARCHAR(20)    NOT NULL CHECK (tipo_checklist IN ('WinMovers', 'OtroAgente')),
    completado              BIT             NOT NULL DEFAULT 0,    
    fecha_completado        DATETIME2       NULL,
    observaciones           NVARCHAR(500)   NULL,
    CONSTRAINT FK_ExpDoc_Exportacion FOREIGN KEY (id_exportacion) REFERENCES Exportaciones(id_exportacion)
        ON DELETE CASCADE,
    CONSTRAINT FK_ExpDoc_TipoDoc FOREIGN KEY (id_tipo_documento) REFERENCES Catalogo_Documentos(id_tipo_documento)
);
GO


-- Modelo JS: Importacion.model.js
-- Campos alineados con el constructor del modelo


CREATE TABLE Importaciones (
    id_importacion          INT IDENTITY(1,1) PRIMARY KEY,
    nombre_cliente          NVARCHAR(150)   NOT NULL,         
    referencia              NVARCHAR(50)    NULL,                
    fecha                   DATE            NULL,             
    observaciones           NVARCHAR(MAX)   NULL,             
    fecha_creacion          DATETIME2       NOT NULL DEFAULT GETDATE(),
    fecha_actualizacion     DATETIME2       NULL
);
GO

-- Checklist de documentos de importación (JS: docsWinMovers + docsOtroAgente)
CREATE TABLE Importaciones_Documentos (
    id_imp_doc              INT IDENTITY(1,1) PRIMARY KEY,
    id_importacion          INT             NOT NULL,
    id_tipo_documento       INT             NOT NULL,
    tipo_checklist          NVARCHAR(20)    NOT NULL CHECK (tipo_checklist IN ('WinMovers', 'OtroAgente')),
    completado              BIT             NOT NULL DEFAULT 0,
    fecha_completado        DATETIME2       NULL,
    observaciones           NVARCHAR(500)   NULL,
    CONSTRAINT FK_ImpDoc_Importacion FOREIGN KEY (id_importacion) REFERENCES Importaciones(id_importacion)
        ON DELETE CASCADE,
    CONSTRAINT FK_ImpDoc_TipoDoc FOREIGN KEY (id_tipo_documento) REFERENCES Catalogo_Documentos(id_tipo_documento)
);
GO



INSERT INTO Catalogo_Documentos (nombre, aplica_exportacion, aplica_importacion, aplica_winmovers, aplica_otro_agente, orden_presentacion) VALUES
    ('Reporte de Visita Previa',            1, 0, 1, 1, 1),
    ('Cotización',                          1, 1, 1, 0, 2),
    ('Lista de inventario para el seguro',  1, 1, 1, 1, 3),
    ('Cotización con firma de aceptación',  1, 1, 1, 0, 4),
    ('Hoja de Trabajo',                     1, 1, 1, 1, 5),
    ('Pre-Aviso al agente de destino',      1, 0, 1, 1, 6),
    ('Instrucciones del Embarque',          1, 1, 1, 1, 7),
    ('Carte de porte, AWA o B-L',           1, 1, 1, 1, 8),
    ('Certificado del seguro',              1, 1, 1, 1, 9),
    ('Lista de empaque firmada',            1, 1, 1, 1, 10),
    ('Factura',                             1, 1, 1, 1, 11),
    ('Confirmación de Entrega',             1, 1, 1, 1, 12);
GO



CREATE PROCEDURE sp_OrdenTrabajo_Insertar
    @numero_ot              NVARCHAR(20),
    @fecha_servicio         DATE = NULL,
    @fecha                  DATE = NULL,
    @hora                   NVARCHAR(10) = NULL,
    @nombre_cliente         NVARCHAR(150),
    @telefono_celular       NVARCHAR(30) = NULL,
    @telefono_residencia    NVARCHAR(30) = NULL,
    @compania               NVARCHAR(150) = NULL,
    @telefono_empresa       NVARCHAR(30) = NULL,
    @contacto               NVARCHAR(150) = NULL,
    @direccion_origen       NVARCHAR(500) = NULL,
    @direccion_destino      NVARCHAR(500) = NULL,
    @detalle_servicio       NVARCHAR(MAX) = NULL,
    @materiales             NVARCHAR(MAX) = NULL,
    @facturar_a             NVARCHAR(150) = NULL,
    @direccion_cobro        NVARCHAR(500) = NULL,
    @hecho_por              NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Ordenes_Trabajo (numero_ot, fecha_servicio, fecha, hora, nombre_cliente,
        telefono_celular, telefono_residencia, compania, telefono_empresa, contacto,
        direccion_origen, direccion_destino, detalle_servicio, materiales,
        facturar_a, direccion_cobro, hecho_por)
    VALUES (@numero_ot, @fecha_servicio, @fecha, @hora, @nombre_cliente,
        @telefono_celular, @telefono_residencia, @compania, @telefono_empresa, @contacto,
        @direccion_origen, @direccion_destino, @detalle_servicio, @materiales,
        @facturar_a, @direccion_cobro, @hecho_por);

    SELECT SCOPE_IDENTITY() AS id_orden;
END
GO

CREATE PROCEDURE sp_OrdenTrabajo_Actualizar
    @id_orden               INT,
    @numero_ot              NVARCHAR(20),
    @fecha_servicio         DATE = NULL,
    @fecha                  DATE = NULL,
    @hora                   NVARCHAR(10) = NULL,
    @nombre_cliente         NVARCHAR(150),
    @telefono_celular       NVARCHAR(30) = NULL,
    @telefono_residencia    NVARCHAR(30) = NULL,
    @compania               NVARCHAR(150) = NULL,
    @telefono_empresa       NVARCHAR(30) = NULL,
    @contacto               NVARCHAR(150) = NULL,
    @direccion_origen       NVARCHAR(500) = NULL,
    @direccion_destino      NVARCHAR(500) = NULL,
    @detalle_servicio       NVARCHAR(MAX) = NULL,
    @materiales             NVARCHAR(MAX) = NULL,
    @facturar_a             NVARCHAR(150) = NULL,
    @direccion_cobro        NVARCHAR(500) = NULL,
    @hecho_por              NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Ordenes_Trabajo SET
        numero_ot = @numero_ot,
        fecha_servicio = @fecha_servicio,
        fecha = @fecha,
        hora = @hora,
        nombre_cliente = @nombre_cliente,
        telefono_celular = @telefono_celular,
        telefono_residencia = @telefono_residencia,
        compania = @compania,
        telefono_empresa = @telefono_empresa,
        contacto = @contacto,
        direccion_origen = @direccion_origen,
        direccion_destino = @direccion_destino,
        detalle_servicio = @detalle_servicio,
        materiales = @materiales,
        facturar_a = @facturar_a,
        direccion_cobro = @direccion_cobro,
        hecho_por = @hecho_por,
        fecha_actualizacion = GETDATE()
    WHERE id_orden = @id_orden;
END
GO

CREATE PROCEDURE sp_OrdenTrabajo_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Ordenes_Trabajo ORDER BY fecha_creacion DESC;
END
GO

CREATE PROCEDURE sp_OrdenTrabajo_ObtenerPorId
    @id_orden INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Ordenes_Trabajo WHERE id_orden = @id_orden;
END
GO

CREATE PROCEDURE sp_OrdenTrabajo_Eliminar
    @id_orden INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Ordenes_Trabajo WHERE id_orden = @id_orden;
END
GO

CREATE PROCEDURE sp_OrdenTrabajo_Conteo
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS total FROM Ordenes_Trabajo;
END
GO


-- ═══════════════════════════════════════════════════════════════
-- CRUD: Control_Visitas (ControlVisitas.model.js)
-- ═══════════════════════════════════════════════════════════════

CREATE PROCEDURE sp_ControlVisita_Insertar
    @fecha_llamada          DATE = NULL,
    @fecha_visita           DATE = NULL,
    @hora                   NVARCHAR(10) = NULL,
    @nombre_cliente         NVARCHAR(150),
    @telefono_habitacion    NVARCHAR(30) = NULL,
    @telefono_celular       NVARCHAR(30) = NULL,
    @empresa                NVARCHAR(150) = NULL,
    @telefono_compania      NVARCHAR(30) = NULL,
    @direccion_origen       NVARCHAR(500) = NULL,
    @direccion_destino      NVARCHAR(500) = NULL,
    @observaciones          NVARCHAR(MAX) = NULL,
    @puerta_a_puerta        BIT = 0,
    @puerta_a_puerto        BIT = 0,
    @empaque                BIT = 0,
    @mudanza_local          BIT = 0,
    @origen                 NVARCHAR(150) = NULL,
    @tramites_aduana        NVARCHAR(100) = NULL,
    @flete                  NVARCHAR(100) = NULL,
    @destino                NVARCHAR(150) = NULL,
    @tarifa_total           NVARCHAR(100) = NULL,
    @compania_maritima      NVARCHAR(100) = NULL,
    @corresponsal           NVARCHAR(100) = NULL,
    @hecho_por              NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Control_Visitas (fecha_llamada, fecha_visita, hora, nombre_cliente,
        telefono_habitacion, telefono_celular, empresa, telefono_compania,
        direccion_origen, direccion_destino, observaciones,
        puerta_a_puerta, puerta_a_puerto, empaque, mudanza_local,
        origen, tramites_aduana, flete, destino, tarifa_total,
        compania_maritima, corresponsal, hecho_por)
    VALUES (@fecha_llamada, @fecha_visita, @hora, @nombre_cliente,
        @telefono_habitacion, @telefono_celular, @empresa, @telefono_compania,
        @direccion_origen, @direccion_destino, @observaciones,
        @puerta_a_puerta, @puerta_a_puerto, @empaque, @mudanza_local,
        @origen, @tramites_aduana, @flete, @destino, @tarifa_total,
        @compania_maritima, @corresponsal, @hecho_por);

    SELECT SCOPE_IDENTITY() AS id_visita;
END
GO

CREATE PROCEDURE sp_ControlVisita_Actualizar
    @id_visita              INT,
    @fecha_llamada          DATE = NULL,
    @fecha_visita           DATE = NULL,
    @hora                   NVARCHAR(10) = NULL,
    @nombre_cliente         NVARCHAR(150),
    @telefono_habitacion    NVARCHAR(30) = NULL,
    @telefono_celular       NVARCHAR(30) = NULL,
    @empresa                NVARCHAR(150) = NULL,
    @telefono_compania      NVARCHAR(30) = NULL,
    @direccion_origen       NVARCHAR(500) = NULL,
    @direccion_destino      NVARCHAR(500) = NULL,
    @observaciones          NVARCHAR(MAX) = NULL,
    @puerta_a_puerta        BIT = 0,
    @puerta_a_puerto        BIT = 0,
    @empaque                BIT = 0,
    @mudanza_local          BIT = 0,
    @origen                 NVARCHAR(150) = NULL,
    @tramites_aduana        NVARCHAR(100) = NULL,
    @flete                  NVARCHAR(100) = NULL,
    @destino                NVARCHAR(150) = NULL,
    @tarifa_total           NVARCHAR(100) = NULL,
    @compania_maritima      NVARCHAR(100) = NULL,
    @corresponsal           NVARCHAR(100) = NULL,
    @hecho_por              NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Control_Visitas SET
        fecha_llamada = @fecha_llamada,
        fecha_visita = @fecha_visita,
        hora = @hora,
        nombre_cliente = @nombre_cliente,
        telefono_habitacion = @telefono_habitacion,
        telefono_celular = @telefono_celular,
        empresa = @empresa,
        telefono_compania = @telefono_compania,
        direccion_origen = @direccion_origen,
        direccion_destino = @direccion_destino,
        observaciones = @observaciones,
        puerta_a_puerta = @puerta_a_puerta,
        puerta_a_puerto = @puerta_a_puerto,
        empaque = @empaque,
        mudanza_local = @mudanza_local,
        origen = @origen,
        tramites_aduana = @tramites_aduana,
        flete = @flete,
        destino = @destino,
        tarifa_total = @tarifa_total,
        compania_maritima = @compania_maritima,
        corresponsal = @corresponsal,
        hecho_por = @hecho_por,
        fecha_actualizacion = GETDATE()
    WHERE id_visita = @id_visita;
END
GO

CREATE PROCEDURE sp_ControlVisita_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Control_Visitas ORDER BY fecha_creacion DESC;
END
GO

CREATE PROCEDURE sp_ControlVisita_ObtenerPorId
    @id_visita INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Control_Visitas WHERE id_visita = @id_visita;
END
GO

CREATE PROCEDURE sp_ControlVisita_Eliminar
    @id_visita INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Control_Visitas WHERE id_visita = @id_visita;
END
GO

CREATE PROCEDURE sp_ControlVisita_Conteo
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS total FROM Control_Visitas;
END
GO


-- ═══════════════════════════════════════════════════════════════
-- CRUD: Exportaciones (Exportacion.model.js)
-- ═══════════════════════════════════════════════════════════════

CREATE PROCEDURE sp_Exportacion_Insertar
    @nombre_cliente     NVARCHAR(150),
    @referencia         NVARCHAR(50) = NULL,
    @fecha              DATE = NULL,
    @observaciones      NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Exportaciones (nombre_cliente, referencia, fecha, observaciones)
    VALUES (@nombre_cliente, @referencia, @fecha, @observaciones);

    DECLARE @id_exportacion INT = SCOPE_IDENTITY();

    -- Generar checklist WinMovers automáticamente (JS: docsWinMovers)
    INSERT INTO Exportaciones_Documentos (id_exportacion, id_tipo_documento, tipo_checklist)
    SELECT @id_exportacion, id_tipo_documento, 'WinMovers'
    FROM Catalogo_Documentos
    WHERE aplica_exportacion = 1 AND aplica_winmovers = 1 AND activo = 1
    ORDER BY orden_presentacion;

    -- Generar checklist Otro Agente automáticamente (JS: docsOtroAgente)
    INSERT INTO Exportaciones_Documentos (id_exportacion, id_tipo_documento, tipo_checklist)
    SELECT @id_exportacion, id_tipo_documento, 'OtroAgente'
    FROM Catalogo_Documentos
    WHERE aplica_exportacion = 1 AND aplica_otro_agente = 1 AND activo = 1
    ORDER BY orden_presentacion;

    SELECT @id_exportacion AS id_exportacion;
END
GO

CREATE PROCEDURE sp_Exportacion_Actualizar
    @id_exportacion     INT,
    @nombre_cliente     NVARCHAR(150),
    @referencia         NVARCHAR(50) = NULL,
    @fecha              DATE = NULL,
    @observaciones      NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Exportaciones SET
        nombre_cliente = @nombre_cliente,
        referencia = @referencia,
        fecha = @fecha,
        observaciones = @observaciones,
        fecha_actualizacion = GETDATE()
    WHERE id_exportacion = @id_exportacion;
END
GO

CREATE PROCEDURE sp_Exportacion_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Exportaciones ORDER BY fecha_creacion DESC;
END
GO

CREATE PROCEDURE sp_Exportacion_ObtenerPorId
    @id_exportacion INT
AS
BEGIN
    SET NOCOUNT ON;
    -- Datos generales
    SELECT * FROM Exportaciones WHERE id_exportacion = @id_exportacion;
    -- Checklist de documentos con nombre del documento
    SELECT ed.id_exp_doc, ed.tipo_checklist, cd.nombre AS nombre_documento,
           ed.completado, ed.fecha_completado, ed.observaciones
    FROM Exportaciones_Documentos ed
    INNER JOIN Catalogo_Documentos cd ON ed.id_tipo_documento = cd.id_tipo_documento
    WHERE ed.id_exportacion = @id_exportacion
    ORDER BY ed.tipo_checklist, cd.orden_presentacion;
END
GO

CREATE PROCEDURE sp_Exportacion_Eliminar
    @id_exportacion INT
AS
BEGIN
    SET NOCOUNT ON;
    -- CASCADE elimina los documentos automáticamente
    DELETE FROM Exportaciones WHERE id_exportacion = @id_exportacion;
END
GO

CREATE PROCEDURE sp_Exportacion_Conteo
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS total FROM Exportaciones;
END
GO

-- Marcar/desmarcar un documento del checklist
CREATE PROCEDURE sp_Exportacion_MarcarDocumento
    @id_exp_doc     INT,
    @completado     BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Exportaciones_Documentos SET
        completado = @completado,
        fecha_completado = CASE WHEN @completado = 1 THEN GETDATE() ELSE NULL END
    WHERE id_exp_doc = @id_exp_doc;
END
GO


-- ═══════════════════════════════════════════════════════════════
-- CRUD: Importaciones (Importacion.model.js)
-- ═══════════════════════════════════════════════════════════════

CREATE PROCEDURE sp_Importacion_Insertar
    @nombre_cliente     NVARCHAR(150),
    @referencia         NVARCHAR(50) = NULL,
    @fecha              DATE = NULL,
    @observaciones      NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Importaciones (nombre_cliente, referencia, fecha, observaciones)
    VALUES (@nombre_cliente, @referencia, @fecha, @observaciones);

    DECLARE @id_importacion INT = SCOPE_IDENTITY();

    -- Generar checklist WinMovers automáticamente (JS: docsWinMovers)
    INSERT INTO Importaciones_Documentos (id_importacion, id_tipo_documento, tipo_checklist)
    SELECT @id_importacion, id_tipo_documento, 'WinMovers'
    FROM Catalogo_Documentos
    WHERE aplica_importacion = 1 AND aplica_winmovers = 1 AND activo = 1
    ORDER BY orden_presentacion;

    -- Generar checklist Otro Agente automáticamente (JS: docsOtroAgente)
    INSERT INTO Importaciones_Documentos (id_importacion, id_tipo_documento, tipo_checklist)
    SELECT @id_importacion, id_tipo_documento, 'OtroAgente'
    FROM Catalogo_Documentos
    WHERE aplica_importacion = 1 AND aplica_otro_agente = 1 AND activo = 1
    ORDER BY orden_presentacion;

    SELECT @id_importacion AS id_importacion;
END
GO

CREATE PROCEDURE sp_Importacion_Actualizar
    @id_importacion     INT,
    @nombre_cliente     NVARCHAR(150),
    @referencia         NVARCHAR(50) = NULL,
    @fecha              DATE = NULL,
    @observaciones      NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Importaciones SET
        nombre_cliente = @nombre_cliente,
        referencia = @referencia,
        fecha = @fecha,
        observaciones = @observaciones,
        fecha_actualizacion = GETDATE()
    WHERE id_importacion = @id_importacion;
END
GO

CREATE PROCEDURE sp_Importacion_ObtenerTodos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Importaciones ORDER BY fecha_creacion DESC;
END
GO

CREATE PROCEDURE sp_Importacion_ObtenerPorId
    @id_importacion INT
AS
BEGIN
    SET NOCOUNT ON;
    -- Datos generales
    SELECT * FROM Importaciones WHERE id_importacion = @id_importacion;
    -- Checklist de documentos con nombre del documento
    SELECT id2.id_imp_doc, id2.tipo_checklist, cd.nombre AS nombre_documento,
           id2.completado, id2.fecha_completado, id2.observaciones
    FROM Importaciones_Documentos id2
    INNER JOIN Catalogo_Documentos cd ON id2.id_tipo_documento = cd.id_tipo_documento
    WHERE id2.id_importacion = @id_importacion
    ORDER BY id2.tipo_checklist, cd.orden_presentacion;
END
GO

CREATE PROCEDURE sp_Importacion_Eliminar
    @id_importacion INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM Importaciones WHERE id_importacion = @id_importacion;
END
GO

CREATE PROCEDURE sp_Importacion_Conteo
AS
BEGIN
    SET NOCOUNT ON;
    SELECT COUNT(*) AS total FROM Importaciones;
END
GO

-- Marcar/desmarcar un documento del checklist
CREATE PROCEDURE sp_Importacion_MarcarDocumento
    @id_imp_doc     INT,
    @completado     BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Importaciones_Documentos SET
        completado = @completado,
        fecha_completado = CASE WHEN @completado = 1 THEN GETDATE() ELSE NULL END
    WHERE id_imp_doc = @id_imp_doc;
END
GO


-- =====================================================================
-- ██████  VISTA: DASHBOARD (estadísticas para app.js)
-- =====================================================================

CREATE VIEW vw_Dashboard_Estadisticas AS
SELECT
    (SELECT COUNT(*) FROM Ordenes_Trabajo)  AS total_ordenes,
    (SELECT COUNT(*) FROM Control_Visitas)  AS total_visitas,
    (SELECT COUNT(*) FROM Exportaciones)    AS total_exportaciones,
    (SELECT COUNT(*) FROM Importaciones)    AS total_importaciones;
GO


-- =====================================================================
-- ██████  ÍNDICES PARA RENDIMIENTO
-- =====================================================================

CREATE NONCLUSTERED INDEX IX_Ordenes_NumeroOT ON Ordenes_Trabajo(numero_ot);
CREATE NONCLUSTERED INDEX IX_Ordenes_NombreCliente ON Ordenes_Trabajo(nombre_cliente);
CREATE NONCLUSTERED INDEX IX_Ordenes_FechaServicio ON Ordenes_Trabajo(fecha_servicio);
CREATE NONCLUSTERED INDEX IX_Visitas_NombreCliente ON Control_Visitas(nombre_cliente);
CREATE NONCLUSTERED INDEX IX_Visitas_FechaVisita ON Control_Visitas(fecha_visita);
CREATE NONCLUSTERED INDEX IX_Exportaciones_NombreCliente ON Exportaciones(nombre_cliente);
CREATE NONCLUSTERED INDEX IX_Importaciones_NombreCliente ON Importaciones(nombre_cliente);
CREATE NONCLUSTERED INDEX IX_ExpDocs_Exportacion ON Exportaciones_Documentos(id_exportacion);
CREATE NONCLUSTERED INDEX IX_ImpDocs_Importacion ON Importaciones_Documentos(id_importacion);
GO


-- =====================================================================
-- FIN DEL SCRIPT
-- =====================================================================
-- Para ejecutar:
--   1. Abrir SQL Server Management Studio (SSMS)
--   2. Conectarse a la instancia de SQL Server
--   3. Abrir este archivo y ejecutar (F5)
--
-- Mapeo Modelo JS → Stored Procedures:
--   ┌──────────────────────────┬──────────────────────────────────────┐
--   │ Método JS                │ Stored Procedure                     │
--   ├──────────────────────────┼──────────────────────────────────────┤
--   │ guardar() [nuevo]        │ sp_*_Insertar                        │
--   │ guardar() [editar]       │ sp_*_Actualizar                      │
--   │ obtenerTodas/Todos()     │ sp_*_ObtenerTodos                    │
--   │ obtenerPorId()           │ sp_*_ObtenerPorId                    │
--   │ eliminar()               │ sp_*_Eliminar                        │
--   │ conteo()                 │ sp_*_Conteo                          │
--   └──────────────────────────┴──────────────────────────────────────┘
--
-- Para agregar o quitar documentos del checklist:
--   INSERT/UPDATE/DELETE en Catalogo_Documentos
--   Los nuevos registros se asignarán automáticamente
--   a futuras exportaciones/importaciones.
-- =====================================================================
