-- =====================================================================
-- WinMovers - Script de Creación de Base de Datos
-- Motor: SQL Server
-- Versión: 2.0.0
-- Fecha: 2026-07-22
-- Descripción: Sistema interno de gestión de traslados y mudanzas
-- =====================================================================
-- ESTRUCTURA DE ESTE SCRIPT
--   1. Prólogo          : crea la base de datos WinMoversDB si no existe.
--   2. Esquema (EF Core): tablas, llaves foráneas, índices y el registro
--                         de migraciones (__EFMigrationsHistory).
--                         >> Esta sección se GENERA con:
--                            dotnet ef migrations script
--                         No editar a mano: regenerarla al agregar una
--                         nueva migración para mantenerla fiel al modelo.
--   3. Seeds            : datos iniciales (Roles de Identity y el
--                         Catálogo de Documentos). No están en las
--                         migraciones EF, se mantienen aquí.
--   4. Procedimientos   : stored procedures usados por la aplicación
--                         (p. ej. sp_Importacion_Insertar,
--                         sp_Exportacion_Insertar). No están en EF.
-- =====================================================================
-- Módulos:
--   ORD - Órdenes de Trabajo
--   VIS - Control de Visitas
--   EXP - Exportaciones (Checklist de embarque)
--   IMP - Importaciones (Checklist de embarque)
--   COT - Cotizaciones (HU-COT-001 a 004)
--   AUT - Usuarios, Roles y Auditoría (Identity)
-- =====================================================================

-- =====================================================================
-- 1. PRÓLOGO
-- =====================================================================
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'WinMoversDB')
BEGIN
    CREATE DATABASE WinMoversDB;
END
GO

USE WinMoversDB;
GO

-- Requerido para los índices filtrados, la vista y los procedimientos.
-- SSMS y EF Core dejan estas opciones en ON automáticamente, pero sqlcmd
-- las deja en OFF y la creación de índices filtrados fallaría.
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- =====================================================================
-- 2. ESQUEMA (generado por EF Core - no editar a mano)
-- =====================================================================
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [ControlVisitas] (
    [IdVisita] int NOT NULL IDENTITY,
    [FechaLlamada] datetime2 NULL,
    [FechaVisita] datetime2 NULL,
    [Hora] nvarchar(10) NULL,
    [NombreCliente] nvarchar(150) NOT NULL,
    [TelefonoHabitacion] nvarchar(30) NULL,
    [TelefonoCelular] nvarchar(30) NULL,
    [Empresa] nvarchar(150) NULL,
    [TelefonoCompania] nvarchar(30) NULL,
    [DireccionOrigen] nvarchar(500) NULL,
    [DireccionDestino] nvarchar(500) NULL,
    [Observaciones] nvarchar(max) NULL,
    [PuertaAPuerta] bit NOT NULL,
    [PuertaAPuerto] bit NOT NULL,
    [Empaque] bit NOT NULL,
    [MudanzaLocal] bit NOT NULL,
    [Origen] nvarchar(150) NULL,
    [TramitesAduana] nvarchar(100) NULL,
    [Flete] nvarchar(100) NULL,
    [Destino] nvarchar(150) NULL,
    [TarifaTotal] nvarchar(100) NULL,
    [CompaniaMaritima] nvarchar(100) NULL,
    [Corresponsal] nvarchar(100) NULL,
    [HechoPor] nvarchar(100) NULL,
    [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_ControlVisitas] PRIMARY KEY ([IdVisita])
);

CREATE TABLE [Exportaciones] (
    [IdExportacion] int NOT NULL IDENTITY,
    [NombreCliente] nvarchar(150) NOT NULL,
    [Referencia] nvarchar(100) NULL,
    [Fecha] datetime2 NULL,
    [Observaciones] nvarchar(max) NULL,
    [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Exportaciones] PRIMARY KEY ([IdExportacion])
);

CREATE TABLE [Importaciones] (
    [IdImportacion] int NOT NULL IDENTITY,
    [NombreCliente] nvarchar(150) NOT NULL,
    [Referencia] nvarchar(100) NULL,
    [Fecha] datetime2 NULL,
    [Observaciones] nvarchar(max) NULL,
    [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Importaciones] PRIMARY KEY ([IdImportacion])
);

CREATE TABLE [OrdenesTrabajo] (
    [IdOrden] int NOT NULL IDENTITY,
    [NumeroOT] nvarchar(20) NOT NULL,
    [FechaServicio] datetime2 NULL,
    [Fecha] datetime2 NULL,
    [Hora] nvarchar(10) NULL,
    [NombreCliente] nvarchar(150) NOT NULL,
    [TelefonoCelular] nvarchar(30) NULL,
    [TelefonoResidencia] nvarchar(30) NULL,
    [Compania] nvarchar(150) NULL,
    [TelefonoEmpresa] nvarchar(30) NULL,
    [Contacto] nvarchar(150) NULL,
    [DireccionOrigen] nvarchar(500) NULL,
    [DireccionDestino] nvarchar(500) NULL,
    [DetalleServicio] nvarchar(max) NULL,
    [Materiales] nvarchar(500) NULL,
    [FacturarA] nvarchar(150) NULL,
    [DireccionCobro] nvarchar(500) NULL,
    [HechoPor] nvarchar(100) NULL,
    [FechaCreacion] datetime2 NOT NULL DEFAULT (GETDATE()),
    [FechaActualizacion] datetime2 NULL,
    CONSTRAINT [PK_OrdenesTrabajo] PRIMARY KEY ([IdOrden])
);

CREATE TABLE Inventario
(
    id_material INT IDENTITY(1,1) PRIMARY KEY,

    nombre_material NVARCHAR(100) NOT NULL,

    descripcion NVARCHAR(250) NULL,

    categoria NVARCHAR(50) NOT NULL,

    unidad NVARCHAR(30) NOT NULL,

    existencias INT NOT NULL
        CHECK(existencias >= 0),

    stock_minimo INT NOT NULL
        DEFAULT 0,

    fecha_creacion DATETIME2 NOT NULL
        DEFAULT GETDATE(),

    fecha_actualizacion DATETIME2 NULL
);

CREATE TABLE OrdenTrabajoMaterial
(
    id_orden_material INT IDENTITY(1,1) PRIMARY KEY,

    id_orden INT NOT NULL,

    id_material INT NOT NULL,

    cantidad INT NOT NULL CHECK(cantidad > 0),

    fecha_asignacion DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_OrdenMaterial_Orden
        FOREIGN KEY(id_orden)
        REFERENCES Ordenes_Trabajo(id_orden),

    CONSTRAINT FK_OrdenMaterial_Inventario
        FOREIGN KEY(id_material)
        REFERENCES Inventario(id_material)
);

-- HU-INV-003: bienes del cliente transportados en cada orden.
CREATE TABLE BienesMudanza
(
    id_bien INT IDENTITY(1,1) PRIMARY KEY,

    id_orden INT NOT NULL,

    nombre_bien NVARCHAR(150) NOT NULL,

    descripcion NVARCHAR(250) NULL,

    cantidad INT NOT NULL CHECK(cantidad > 0),

    condicion NVARCHAR(50) NOT NULL,

    observaciones NVARCHAR(500) NULL,

    fecha_registro DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_BienesMudanza_Orden
        FOREIGN KEY(id_orden)
        REFERENCES Ordenes_Trabajo(id_orden)
        ON DELETE CASCADE
);

-- Escenario 2 de HU-INV-003: un bien no puede repetirse en la misma orden.
CREATE UNIQUE INDEX UX_BienesMudanza_Orden_Nombre
    ON BienesMudanza(id_orden, nombre_bien);

CREATE TABLE [ExportacionesDocumentos] (
    [IdDocumento] int NOT NULL IDENTITY,
    [IdExportacion] int NOT NULL,
    [NombreDocumento] nvarchar(200) NOT NULL,
    [TipoAgente] nvarchar(20) NOT NULL,
    [Completado] bit NOT NULL,
    CONSTRAINT [PK_ExportacionesDocumentos] PRIMARY KEY ([IdDocumento]),
    CONSTRAINT [FK_ExportacionesDocumentos_Exportaciones_IdExportacion] FOREIGN KEY ([IdExportacion]) REFERENCES [Exportaciones] ([IdExportacion]) ON DELETE CASCADE
);

CREATE TABLE [ImportacionesDocumentos] (
    [IdDocumento] int NOT NULL IDENTITY,
    [IdImportacion] int NOT NULL,
    [NombreDocumento] nvarchar(200) NOT NULL,
    [TipoAgente] nvarchar(20) NOT NULL,
    [Completado] bit NOT NULL,
    CONSTRAINT [PK_ImportacionesDocumentos] PRIMARY KEY ([IdDocumento]),
    CONSTRAINT [FK_ImportacionesDocumentos_Importaciones_IdImportacion] FOREIGN KEY ([IdImportacion]) REFERENCES [Importaciones] ([IdImportacion]) ON DELETE CASCADE
);

CREATE INDEX [IX_ExportacionesDocumentos_IdExportacion] ON [ExportacionesDocumentos] ([IdExportacion]);

CREATE INDEX [IX_ImportacionesDocumentos_IdImportacion] ON [ImportacionesDocumentos] ([IdImportacion]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260515185930_Initial', N'9.0.0');

ALTER TABLE [ExportacionesDocumentos] DROP CONSTRAINT [FK_ExportacionesDocumentos_Exportaciones_IdExportacion];

ALTER TABLE [ImportacionesDocumentos] DROP CONSTRAINT [FK_ImportacionesDocumentos_Importaciones_IdImportacion];

ALTER TABLE [OrdenesTrabajo] DROP CONSTRAINT [PK_OrdenesTrabajo];

ALTER TABLE [ImportacionesDocumentos] DROP CONSTRAINT [PK_ImportacionesDocumentos];

ALTER TABLE [ExportacionesDocumentos] DROP CONSTRAINT [PK_ExportacionesDocumentos];

ALTER TABLE [ControlVisitas] DROP CONSTRAINT [PK_ControlVisitas];

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportacionesDocumentos]') AND [c].[name] = N'NombreDocumento');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ImportacionesDocumentos] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [ImportacionesDocumentos] DROP COLUMN [NombreDocumento];

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ImportacionesDocumentos]') AND [c].[name] = N'TipoAgente');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ImportacionesDocumentos] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [ImportacionesDocumentos] DROP COLUMN [TipoAgente];

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportacionesDocumentos]') AND [c].[name] = N'NombreDocumento');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [ExportacionesDocumentos] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [ExportacionesDocumentos] DROP COLUMN [NombreDocumento];

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExportacionesDocumentos]') AND [c].[name] = N'TipoAgente');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [ExportacionesDocumentos] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [ExportacionesDocumentos] DROP COLUMN [TipoAgente];

EXEC sp_rename N'[OrdenesTrabajo]', N'Ordenes_Trabajo', 'OBJECT';

EXEC sp_rename N'[ImportacionesDocumentos]', N'Importaciones_Documentos', 'OBJECT';

EXEC sp_rename N'[ExportacionesDocumentos]', N'Exportaciones_Documentos', 'OBJECT';

EXEC sp_rename N'[ControlVisitas]', N'Control_Visitas', 'OBJECT';

EXEC sp_rename N'[Importaciones].[Referencia]', N'referencia', 'COLUMN';

EXEC sp_rename N'[Importaciones].[Observaciones]', N'observaciones', 'COLUMN';

EXEC sp_rename N'[Importaciones].[Fecha]', N'fecha', 'COLUMN';

EXEC sp_rename N'[Importaciones].[NombreCliente]', N'nombre_cliente', 'COLUMN';

EXEC sp_rename N'[Importaciones].[FechaCreacion]', N'fecha_creacion', 'COLUMN';

EXEC sp_rename N'[Importaciones].[IdImportacion]', N'id_importacion', 'COLUMN';

EXEC sp_rename N'[Exportaciones].[Referencia]', N'referencia', 'COLUMN';

EXEC sp_rename N'[Exportaciones].[Observaciones]', N'observaciones', 'COLUMN';

EXEC sp_rename N'[Exportaciones].[Fecha]', N'fecha', 'COLUMN';

EXEC sp_rename N'[Exportaciones].[NombreCliente]', N'nombre_cliente', 'COLUMN';

EXEC sp_rename N'[Exportaciones].[FechaCreacion]', N'fecha_creacion', 'COLUMN';

EXEC sp_rename N'[Exportaciones].[IdExportacion]', N'id_exportacion', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[Materiales]', N'materiales', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[Hora]', N'hora', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[Fecha]', N'fecha', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[Contacto]', N'contacto', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[Compania]', N'compania', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[TelefonoResidencia]', N'telefono_residencia', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[TelefonoEmpresa]', N'telefono_empresa', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[TelefonoCelular]', N'telefono_celular', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[NumeroOT]', N'numero_ot', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[NombreCliente]', N'nombre_cliente', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[HechoPor]', N'hecho_por', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[FechaServicio]', N'fecha_servicio', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[FechaCreacion]', N'fecha_creacion', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[FechaActualizacion]', N'fecha_actualizacion', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[FacturarA]', N'facturar_a', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[DireccionOrigen]', N'direccion_origen', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[DireccionDestino]', N'direccion_destino', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[DireccionCobro]', N'direccion_cobro', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[DetalleServicio]', N'detalle_servicio', 'COLUMN';

EXEC sp_rename N'[Ordenes_Trabajo].[IdOrden]', N'id_orden', 'COLUMN';

EXEC sp_rename N'[Importaciones_Documentos].[Completado]', N'completado', 'COLUMN';

EXEC sp_rename N'[Importaciones_Documentos].[IdImportacion]', N'id_importacion', 'COLUMN';

EXEC sp_rename N'[Importaciones_Documentos].[IdDocumento]', N'id_imp_doc', 'COLUMN';

EXEC sp_rename N'[Importaciones_Documentos].[IX_ImportacionesDocumentos_IdImportacion]', N'IX_Importaciones_Documentos_id_importacion', 'INDEX';

EXEC sp_rename N'[Exportaciones_Documentos].[Completado]', N'completado', 'COLUMN';

EXEC sp_rename N'[Exportaciones_Documentos].[IdExportacion]', N'id_exportacion', 'COLUMN';

EXEC sp_rename N'[Exportaciones_Documentos].[IdDocumento]', N'id_exp_doc', 'COLUMN';

EXEC sp_rename N'[Exportaciones_Documentos].[IX_ExportacionesDocumentos_IdExportacion]', N'IX_Exportaciones_Documentos_id_exportacion', 'INDEX';

EXEC sp_rename N'[Control_Visitas].[Origen]', N'origen', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[Observaciones]', N'observaciones', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[Hora]', N'hora', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[Flete]', N'flete', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[Empresa]', N'empresa', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[Empaque]', N'empaque', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[Destino]', N'destino', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[Corresponsal]', N'corresponsal', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[TramitesAduana]', N'tramites_aduana', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[TelefonoHabitacion]', N'telefono_habitacion', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[TelefonoCompania]', N'telefono_compania', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[TelefonoCelular]', N'telefono_celular', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[TarifaTotal]', N'tarifa_total', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[PuertaAPuerto]', N'puerta_a_puerto', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[PuertaAPuerta]', N'puerta_a_puerta', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[NombreCliente]', N'nombre_cliente', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[MudanzaLocal]', N'mudanza_local', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[HechoPor]', N'hecho_por', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[FechaVisita]', N'fecha_visita', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[FechaLlamada]', N'fecha_llamada', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[FechaCreacion]', N'fecha_creacion', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[DireccionOrigen]', N'direccion_origen', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[DireccionDestino]', N'direccion_destino', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[CompaniaMaritima]', N'compania_maritima', 'COLUMN';

EXEC sp_rename N'[Control_Visitas].[IdVisita]', N'id_visita', 'COLUMN';

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Importaciones]') AND [c].[name] = N'referencia');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Importaciones] DROP CONSTRAINT [' + @var4 + '];');
UPDATE [Importaciones] SET [referencia] = N'' WHERE [referencia] IS NULL;
ALTER TABLE [Importaciones] ALTER COLUMN [referencia] nvarchar(100) NOT NULL;
ALTER TABLE [Importaciones] ADD DEFAULT N'' FOR [referencia];

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Importaciones]') AND [c].[name] = N'fecha');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Importaciones] DROP CONSTRAINT [' + @var5 + '];');
UPDATE [Importaciones] SET [fecha] = '0001-01-01T00:00:00.0000000' WHERE [fecha] IS NULL;
ALTER TABLE [Importaciones] ALTER COLUMN [fecha] datetime2 NOT NULL;
ALTER TABLE [Importaciones] ADD DEFAULT '0001-01-01T00:00:00.0000000' FOR [fecha];

ALTER TABLE [Importaciones] ADD [cajas] int NOT NULL DEFAULT 0;

ALTER TABLE [Importaciones] ADD [fecha_actualizacion] datetime2 NULL;

ALTER TABLE [Importaciones] ADD [id_orden] int NULL;

ALTER TABLE [Importaciones] ADD [kilos] decimal(18,2) NOT NULL DEFAULT 0.0;

ALTER TABLE [Importaciones] ADD [pais] nvarchar(100) NOT NULL DEFAULT N'';

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Exportaciones]') AND [c].[name] = N'referencia');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [Exportaciones] DROP CONSTRAINT [' + @var6 + '];');
UPDATE [Exportaciones] SET [referencia] = N'' WHERE [referencia] IS NULL;
ALTER TABLE [Exportaciones] ALTER COLUMN [referencia] nvarchar(100) NOT NULL;
ALTER TABLE [Exportaciones] ADD DEFAULT N'' FOR [referencia];

ALTER TABLE [Exportaciones] ADD [cajas] int NOT NULL DEFAULT 0;

ALTER TABLE [Exportaciones] ADD [fecha_actualizacion] datetime2 NULL;

ALTER TABLE [Exportaciones] ADD [id_orden] int NULL;

ALTER TABLE [Exportaciones] ADD [kilos] decimal(18,2) NOT NULL DEFAULT 0.0;

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Ordenes_Trabajo]') AND [c].[name] = N'materiales');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Ordenes_Trabajo] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [Ordenes_Trabajo] ALTER COLUMN [materiales] nvarchar(max) NULL;

ALTER TABLE [Ordenes_Trabajo] ADD [estado] nvarchar(20) NOT NULL DEFAULT N'';

ALTER TABLE [Ordenes_Trabajo] ADD [id_cliente] int NULL;

ALTER TABLE [Importaciones_Documentos] ADD [fecha_completado] datetime2 NULL;

ALTER TABLE [Importaciones_Documentos] ADD [id_tipo_documento] int NOT NULL DEFAULT 0;

ALTER TABLE [Importaciones_Documentos] ADD [observaciones] nvarchar(max) NULL;

ALTER TABLE [Importaciones_Documentos] ADD [tipo_checklist] nvarchar(20) NULL;

ALTER TABLE [Exportaciones_Documentos] ADD [fecha_completado] datetime2 NULL;

ALTER TABLE [Exportaciones_Documentos] ADD [id_tipo_documento] int NOT NULL DEFAULT 0;

ALTER TABLE [Exportaciones_Documentos] ADD [observaciones] nvarchar(max) NULL;

ALTER TABLE [Exportaciones_Documentos] ADD [tipo_checklist] nvarchar(20) NULL;

ALTER TABLE [Control_Visitas] ADD [fecha_actualizacion] datetime2 NULL;

ALTER TABLE [Ordenes_Trabajo] ADD CONSTRAINT [PK_Ordenes_Trabajo] PRIMARY KEY ([id_orden]);

ALTER TABLE [Importaciones_Documentos] ADD CONSTRAINT [PK_Importaciones_Documentos] PRIMARY KEY ([id_imp_doc]);

ALTER TABLE [Exportaciones_Documentos] ADD CONSTRAINT [PK_Exportaciones_Documentos] PRIMARY KEY ([id_exp_doc]);

ALTER TABLE [Control_Visitas] ADD CONSTRAINT [PK_Control_Visitas] PRIMARY KEY ([id_visita]);

CREATE TABLE [Catalogo_Documentos] (
    [id_tipo_documento] int NOT NULL IDENTITY,
    [nombre] nvarchar(150) NOT NULL,
    [aplica_exportacion] bit NOT NULL,
    [aplica_importacion] bit NOT NULL,
    [aplica_winmovers] bit NOT NULL,
    [aplica_otro_agente] bit NOT NULL,
    [orden_presentacion] int NOT NULL,
    [activo] bit NOT NULL,
    CONSTRAINT [PK_Catalogo_Documentos] PRIMARY KEY ([id_tipo_documento])
);

CREATE TABLE [Clientes] (
    [id_cliente] int NOT NULL IDENTITY,
    [nombre_cliente] nvarchar(200) NOT NULL,
    [telefono_celular] nvarchar(20) NULL,
    [telefono_residencia] nvarchar(20) NULL,
    [telefono_empresa] nvarchar(20) NULL,
    [empresa] nvarchar(200) NULL,
    [contacto] nvarchar(200) NULL,
    [correo_electronico] nvarchar(200) NULL,
    [direccion] nvarchar(500) NOT NULL,
    [observaciones] nvarchar(max) NULL,
    [activo] bit NOT NULL,
    [fecha_registro] datetime2 NOT NULL,
    [fecha_creacion] datetime2 NOT NULL,
    [fecha_actualizacion] datetime2 NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([id_cliente])
);

CREATE TABLE [Exportaciones_Archivos] (
    [id_archivo] int NOT NULL IDENTITY,
    [id_exportacion] int NOT NULL,
    [nombre_original] nvarchar(max) NOT NULL,
    [nombre_guardado] nvarchar(max) NOT NULL,
    [tipo_mime] nvarchar(max) NOT NULL,
    [tamanio_bytes] bigint NOT NULL,
    [fecha_carga] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Exportaciones_Archivos] PRIMARY KEY ([id_archivo]),
    CONSTRAINT [FK_Exportaciones_Archivos_Exportaciones_id_exportacion] FOREIGN KEY ([id_exportacion]) REFERENCES [Exportaciones] ([id_exportacion]) ON DELETE CASCADE
);

CREATE TABLE [Importaciones_Archivos] (
    [id_archivo] int NOT NULL IDENTITY,
    [id_importacion] int NOT NULL,
    [nombre_original] nvarchar(max) NOT NULL,
    [nombre_guardado] nvarchar(max) NOT NULL,
    [tipo_mime] nvarchar(max) NOT NULL,
    [tamanio_bytes] bigint NOT NULL,
    [fecha_carga] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Importaciones_Archivos] PRIMARY KEY ([id_archivo]),
    CONSTRAINT [FK_Importaciones_Archivos_Importaciones_id_importacion] FOREIGN KEY ([id_importacion]) REFERENCES [Importaciones] ([id_importacion]) ON DELETE CASCADE
);

CREATE TABLE [OrdenesTrabajo_Archivos] (
    [id_archivo] int NOT NULL IDENTITY,
    [id_orden] int NOT NULL,
    [nombre_original] nvarchar(max) NOT NULL,
    [nombre_guardado] nvarchar(max) NOT NULL,
    [tipo_mime] nvarchar(max) NOT NULL,
    [tamanio_bytes] bigint NOT NULL,
    [fecha_carga] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_OrdenesTrabajo_Archivos] PRIMARY KEY ([id_archivo]),
    CONSTRAINT [FK_OrdenesTrabajo_Archivos_Ordenes_Trabajo_id_orden] FOREIGN KEY ([id_orden]) REFERENCES [Ordenes_Trabajo] ([id_orden]) ON DELETE CASCADE
);

CREATE TABLE [Roles] (
    [id_rol] int NOT NULL IDENTITY,
    [descripcion] nvarchar(250) NULL,
    [nombre] nvarchar(256) NULL,
    [nombre_normalizado] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([id_rol])
);

CREATE TABLE [Usuarios] (
    [id_usuario] int NOT NULL IDENTITY,
    [nombre_completo] nvarchar(200) NOT NULL,
    [activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [fecha_creacion] datetime2 NOT NULL DEFAULT (GETDATE()),
    [debe_cambiar_contrasena] bit NOT NULL DEFAULT CAST(0 AS bit),
    [nombre_usuario] nvarchar(256) NULL,
    [nombre_usuario_normalizado] nvarchar(256) NULL,
    [correo] nvarchar(256) NULL,
    [correo_normalizado] nvarchar(256) NULL,
    [correo_confirmado] bit NOT NULL,
    [contrasena_hash] nvarchar(max) NULL,
    [security_stamp] nvarchar(max) NULL,
    [concurrency_stamp] nvarchar(max) NULL,
    [telefono] nvarchar(max) NULL,
    [telefono_confirmado] bit NOT NULL,
    [doble_factor_habilitado] bit NOT NULL,
    [bloqueo_hasta] datetimeoffset NULL,
    [bloqueo_habilitado] bit NOT NULL,
    [intentos_fallidos] int NOT NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([id_usuario])
);

CREATE TABLE [Roles_Claims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_Roles_Claims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Roles_Claims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([id_rol]) ON DELETE CASCADE
);

CREATE TABLE [Accesos_Auditoria] (
    [id_auditoria] int NOT NULL IDENTITY,
    [id_usuario] int NULL,
    [correo_intentado] nvarchar(256) NOT NULL,
    [exitoso] bit NOT NULL,
    [motivo] nvarchar(200) NULL,
    [ip_address] nvarchar(45) NULL,
    [fecha] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Accesos_Auditoria] PRIMARY KEY ([id_auditoria]),
    CONSTRAINT [FK_Accesos_Auditoria_Usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE SET NULL
);

CREATE TABLE [Clientes_Historial] (
    [id_historial] int NOT NULL IDENTITY,
    [id_cliente] int NOT NULL,
    [campo_modificado] nvarchar(100) NOT NULL,
    [valor_anterior] nvarchar(max) NULL,
    [valor_nuevo] nvarchar(max) NULL,
    [id_usuario] int NULL,
    [fecha_cambio] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Clientes_Historial] PRIMARY KEY ([id_historial]),
    CONSTRAINT [FK_Clientes_Historial_Clientes_id_cliente] FOREIGN KEY ([id_cliente]) REFERENCES [Clientes] ([id_cliente]) ON DELETE CASCADE,
    CONSTRAINT [FK_Clientes_Historial_Usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE SET NULL
);

CREATE TABLE [Cotizaciones] (
    [id_cotizacion] int NOT NULL IDENTITY,
    [numero_cotizacion] nvarchar(20) NOT NULL,
    [fecha] datetime2 NOT NULL,
    [id_cliente] int NULL,
    [nombre_cliente] nvarchar(200) NOT NULL,
    [compania] nvarchar(200) NULL,
    [contacto] nvarchar(200) NULL,
    [correo_cliente] nvarchar(200) NULL,
    [telefono_celular] nvarchar(30) NULL,
    [tipo_servicio] nvarchar(50) NOT NULL,
    [origen] nvarchar(200) NULL,
    [destino] nvarchar(200) NULL,
    [volumen_m3] decimal(10,2) NULL,
    [tipo_contenedor] nvarchar(30) NULL,
    [compania_maritima] nvarchar(100) NULL,
    [corresponsal] nvarchar(100) NULL,
    [dias_empaque] int NULL,
    [dias_transito] int NULL,
    [dias_desalmacenaje] int NULL,
    [dias_frecuencia_salidas] int NULL,
    [costo_origen] decimal(18,2) NOT NULL,
    [costo_tramites_aduana] decimal(18,2) NOT NULL,
    [costo_flete] decimal(18,2) NOT NULL,
    [costo_destino] decimal(18,2) NOT NULL,
    [incluye_seguro] bit NOT NULL DEFAULT CAST(0 AS bit),
    [valor_declarado] decimal(18,2) NULL,
    [porcentaje_seguro] decimal(5,2) NOT NULL,
    [subtotal] decimal(18,2) NOT NULL,
    [monto_seguro] decimal(18,2) NOT NULL,
    [tarifa_total] decimal(18,2) NOT NULL,
    [moneda] nvarchar(3) NOT NULL,
    [vigencia_dias] int NOT NULL DEFAULT 60,
    [forma_pago] nvarchar(200) NULL,
    [exclusiones] nvarchar(max) NULL,
    [observaciones] nvarchar(max) NULL,
    [estado] nvarchar(20) NOT NULL,
    [fecha_envio] datetime2 NULL,
    [correo_envio] nvarchar(200) NULL,
    [id_orden_generada] int NULL,
    [hecho_por] nvarchar(100) NULL,
    [id_usuario] int NULL,
    [fecha_creacion] datetime2 NOT NULL DEFAULT (GETDATE()),
    [fecha_actualizacion] datetime2 NULL,
    CONSTRAINT [PK_Cotizaciones] PRIMARY KEY ([id_cotizacion]),
    CONSTRAINT [FK_Cotizaciones_Clientes_id_cliente] FOREIGN KEY ([id_cliente]) REFERENCES [Clientes] ([id_cliente]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Cotizaciones_Ordenes_Trabajo_id_orden_generada] FOREIGN KEY ([id_orden_generada]) REFERENCES [Ordenes_Trabajo] ([id_orden]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Cotizaciones_Usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE SET NULL
);

CREATE TABLE [Ordenes_Trabajo_Historial] (
    [id_historial] int NOT NULL IDENTITY,
    [id_orden] int NOT NULL,
    [campo_modificado] nvarchar(max) NOT NULL,
    [valor_anterior] nvarchar(max) NULL,
    [valor_nuevo] nvarchar(max) NULL,
    [id_usuario] int NULL,
    [fecha_cambio] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Ordenes_Trabajo_Historial] PRIMARY KEY ([id_historial]),
    CONSTRAINT [FK_Ordenes_Trabajo_Historial_Ordenes_Trabajo_id_orden] FOREIGN KEY ([id_orden]) REFERENCES [Ordenes_Trabajo] ([id_orden]) ON DELETE CASCADE,
    CONSTRAINT [FK_Ordenes_Trabajo_Historial_Usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE SET NULL
);

CREATE TABLE [Ordenes_Trabajo_Notas] (
    [id_nota] int NOT NULL IDENTITY,
    [id_orden] int NOT NULL,
    [contenido] nvarchar(max) NOT NULL,
    [id_usuario] int NULL,
    [fecha_creacion] datetime2 NOT NULL DEFAULT (GETDATE()),
    [fecha_actualizacion] datetime2 NULL,
    CONSTRAINT [PK_Ordenes_Trabajo_Notas] PRIMARY KEY ([id_nota]),
    CONSTRAINT [FK_Ordenes_Trabajo_Notas_Ordenes_Trabajo_id_orden] FOREIGN KEY ([id_orden]) REFERENCES [Ordenes_Trabajo] ([id_orden]) ON DELETE CASCADE,
    CONSTRAINT [FK_Ordenes_Trabajo_Notas_Usuarios_id_usuario] FOREIGN KEY ([id_usuario]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE SET NULL
);

CREATE TABLE [Roles_Auditoria] (
    [id_auditoria] int NOT NULL IDENTITY,
    [accion] nvarchar(50) NOT NULL,
    [nombre_rol] nvarchar(256) NOT NULL,
    [id_usuario_afectado] int NULL,
    [id_usuario_responsable] int NULL,
    [detalle] nvarchar(max) NULL,
    [fecha] datetime2 NOT NULL DEFAULT (GETDATE()),
    CONSTRAINT [PK_Roles_Auditoria] PRIMARY KEY ([id_auditoria]),
    CONSTRAINT [FK_Roles_Auditoria_Usuarios_id_usuario_afectado] FOREIGN KEY ([id_usuario_afectado]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE SET NULL,
    CONSTRAINT [FK_Roles_Auditoria_Usuarios_id_usuario_responsable] FOREIGN KEY ([id_usuario_responsable]) REFERENCES [Usuarios] ([id_usuario])
);

CREATE TABLE [Usuarios_Claims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_Usuarios_Claims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Usuarios_Claims_Usuarios_UserId] FOREIGN KEY ([UserId]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE CASCADE
);

CREATE TABLE Clientes_Historial (
    id_historial INT IDENTITY(1,1) PRIMARY KEY,

    id_cliente INT NOT NULL,

    campo_modificado NVARCHAR(100) NOT NULL,

    valor_anterior NVARCHAR(500) NULL,

    valor_nuevo NVARCHAR(500) NULL,

    usuario NVARCHAR(100) NULL,

    fecha_cambio DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_ClientesHistorial_Cliente
        FOREIGN KEY (id_cliente)
        REFERENCES Clientes(id_cliente)
        ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX IX_ClientesHistorial_Cliente
ON Clientes_Historial(id_cliente);
GO

-- HU-ORD-004: Notas y observaciones
CREATE TABLE Ordenes_Trabajo_Notas (
    id_nota              INT IDENTITY(1,1) PRIMARY KEY,
    id_orden             INT             NOT NULL,
    contenido            NVARCHAR(MAX)   NOT NULL,
    usuario              NVARCHAR(100)   NULL,       -- texto libre por ahora (sin auth)
    fecha_creacion       DATETIME2       NOT NULL DEFAULT GETDATE(),
    fecha_actualizacion  DATETIME2       NULL,
    CONSTRAINT FK_OTNota_Orden
        FOREIGN KEY (id_orden)
        REFERENCES Ordenes_Trabajo(id_orden) ON DELETE CASCADE
CREATE TABLE [Usuarios_Logins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Usuarios_Logins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_Usuarios_Logins_Usuarios_UserId] FOREIGN KEY ([UserId]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE CASCADE
);

CREATE TABLE [Usuarios_Roles] (
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_Usuarios_Roles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_Usuarios_Roles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([id_rol]) ON DELETE CASCADE,
    CONSTRAINT [FK_Usuarios_Roles_Usuarios_UserId] FOREIGN KEY ([UserId]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE CASCADE
);

CREATE TABLE [Usuarios_Tokens] (
    [UserId] int NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_Usuarios_Tokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_Usuarios_Tokens_Usuarios_UserId] FOREIGN KEY ([UserId]) REFERENCES [Usuarios] ([id_usuario]) ON DELETE CASCADE
);

CREATE INDEX [IX_Importaciones_id_orden] ON [Importaciones] ([id_orden]);

CREATE INDEX [IX_Exportaciones_id_orden] ON [Exportaciones] ([id_orden]);

CREATE INDEX [IX_Ordenes_Trabajo_id_cliente] ON [Ordenes_Trabajo] ([id_cliente]);

CREATE INDEX [IX_Importaciones_Documentos_id_tipo_documento] ON [Importaciones_Documentos] ([id_tipo_documento]);

CREATE INDEX [IX_Exportaciones_Documentos_id_tipo_documento] ON [Exportaciones_Documentos] ([id_tipo_documento]);

CREATE INDEX [IX_Accesos_Auditoria_id_usuario] ON [Accesos_Auditoria] ([id_usuario]);

CREATE INDEX [IX_Clientes_Historial_id_cliente] ON [Clientes_Historial] ([id_cliente]);

CREATE INDEX [IX_Clientes_Historial_id_usuario] ON [Clientes_Historial] ([id_usuario]);

CREATE INDEX [IX_Cotizaciones_id_cliente] ON [Cotizaciones] ([id_cliente]);

CREATE INDEX [IX_Cotizaciones_id_orden_generada] ON [Cotizaciones] ([id_orden_generada]);

CREATE INDEX [IX_Cotizaciones_id_usuario] ON [Cotizaciones] ([id_usuario]);

CREATE UNIQUE INDEX [IX_Cotizaciones_numero_cotizacion] ON [Cotizaciones] ([numero_cotizacion]);

CREATE INDEX [IX_Exportaciones_Archivos_id_exportacion] ON [Exportaciones_Archivos] ([id_exportacion]);

CREATE INDEX [IX_Importaciones_Archivos_id_importacion] ON [Importaciones_Archivos] ([id_importacion]);

CREATE INDEX [IX_Ordenes_Trabajo_Historial_id_orden] ON [Ordenes_Trabajo_Historial] ([id_orden]);

CREATE INDEX [IX_Ordenes_Trabajo_Historial_id_usuario] ON [Ordenes_Trabajo_Historial] ([id_usuario]);

CREATE INDEX [IX_Ordenes_Trabajo_Notas_id_orden] ON [Ordenes_Trabajo_Notas] ([id_orden]);

CREATE INDEX [IX_Ordenes_Trabajo_Notas_id_usuario] ON [Ordenes_Trabajo_Notas] ([id_usuario]);

CREATE INDEX [IX_OrdenesTrabajo_Archivos_id_orden] ON [OrdenesTrabajo_Archivos] ([id_orden]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([nombre_normalizado]) WHERE [nombre_normalizado] IS NOT NULL;

CREATE INDEX [IX_Roles_Auditoria_id_usuario_afectado] ON [Roles_Auditoria] ([id_usuario_afectado]);

CREATE INDEX [IX_Roles_Auditoria_id_usuario_responsable] ON [Roles_Auditoria] ([id_usuario_responsable]);

CREATE INDEX [IX_Roles_Claims_RoleId] ON [Roles_Claims] ([RoleId]);

CREATE INDEX [EmailIndex] ON [Usuarios] ([correo_normalizado]);

CREATE UNIQUE INDEX [UserNameIndex] ON [Usuarios] ([nombre_usuario_normalizado]) WHERE [nombre_usuario_normalizado] IS NOT NULL;

CREATE INDEX [IX_Usuarios_Claims_UserId] ON [Usuarios_Claims] ([UserId]);

CREATE INDEX [IX_Usuarios_Logins_UserId] ON [Usuarios_Logins] ([UserId]);

CREATE INDEX [IX_Usuarios_Roles_RoleId] ON [Usuarios_Roles] ([RoleId]);

ALTER TABLE [Exportaciones] ADD CONSTRAINT [FK_Exportaciones_Ordenes_Trabajo_id_orden] FOREIGN KEY ([id_orden]) REFERENCES [Ordenes_Trabajo] ([id_orden]);

ALTER TABLE [Exportaciones_Documentos] ADD CONSTRAINT [FK_Exportaciones_Documentos_Catalogo_Documentos_id_tipo_documento] FOREIGN KEY ([id_tipo_documento]) REFERENCES [Catalogo_Documentos] ([id_tipo_documento]) ON DELETE NO ACTION;

ALTER TABLE [Exportaciones_Documentos] ADD CONSTRAINT [FK_Exportaciones_Documentos_Exportaciones_id_exportacion] FOREIGN KEY ([id_exportacion]) REFERENCES [Exportaciones] ([id_exportacion]) ON DELETE CASCADE;

ALTER TABLE [Importaciones] ADD CONSTRAINT [FK_Importaciones_Ordenes_Trabajo_id_orden] FOREIGN KEY ([id_orden]) REFERENCES [Ordenes_Trabajo] ([id_orden]);

ALTER TABLE [Importaciones_Documentos] ADD CONSTRAINT [FK_Importaciones_Documentos_Catalogo_Documentos_id_tipo_documento] FOREIGN KEY ([id_tipo_documento]) REFERENCES [Catalogo_Documentos] ([id_tipo_documento]) ON DELETE NO ACTION;

ALTER TABLE [Importaciones_Documentos] ADD CONSTRAINT [FK_Importaciones_Documentos_Importaciones_id_importacion] FOREIGN KEY ([id_importacion]) REFERENCES [Importaciones] ([id_importacion]) ON DELETE CASCADE;

ALTER TABLE [Ordenes_Trabajo] ADD CONSTRAINT [FK_Ordenes_Trabajo_Clientes_id_cliente] FOREIGN KEY ([id_cliente]) REFERENCES [Clientes] ([id_cliente]) ON DELETE NO ACTION;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260715175341_AgregarModuloCotizaciones', N'9.0.0');

COMMIT;
GO


-- =====================================================================
-- 3. SEEDS  y  4. PROCEDIMIENTOS ALMACENADOS
-- (no forman parte de las migraciones EF; se mantienen a mano)
-- =====================================================================

-- Inserts iniciales para la tabla de roles de Identity
INSERT INTO Roles (nombre, nombre_normalizado, descripcion, ConcurrencyStamp)
VALUES
    ('Administrador', 'ADMINISTRADOR', 'Acceso total al sistema', NEWID()),
    ('Empleado', 'EMPLEADO', 'Acceso operativo a órdenes, clientes e importaciones/exportaciones', NEWID()),
    ('SinRol', 'SINROL', 'Rol temporal para usuarios sin un rol asignado (por eliminación de su rol anterior)', NEWID());
GO

---cambios--
ALTER TABLE Ordenes_Trabajo
ADD estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente';

ALTER TABLE Importaciones
ADD id_orden INT;

ALTER TABLE Exportaciones
ADD id_orden INT;

ALTER TABLE [dbo].[Importaciones]
ALTER COLUMN [pais] NVARCHAR(100) NOT NULL;

ALTER TABLE Usuarios
ADD clave_autenticador nvarchar(max) NULL;

--cambio tabla ordenes de trabajo HU-CLI-004--
ALTER TABLE Ordenes_Trabajo
ADD id_cliente INT NULL;

ALTER TABLE Ordenes_Trabajo
ADD CONSTRAINT FK_OrdenTrabajo_Cliente
FOREIGN KEY (id_cliente)
REFERENCES Clientes(id_cliente);

-- =========================================================
-- TABLAS DE IDENTITY (nombres adaptados a la convención del proyecto)
-- =========================================================
CREATE TABLE Usuarios (
    id_usuario                      INT IDENTITY(1,1) PRIMARY KEY,
    nombre_completo                 NVARCHAR(200)   NOT NULL,
    activo                          BIT             NOT NULL DEFAULT 1,
    fecha_creacion                  DATETIME2       NOT NULL DEFAULT GETDATE(),
    debe_cambiar_contrasena         BIT             NOT NULL DEFAULT 0,
    nombre_usuario                  NVARCHAR(256)   NULL,
    nombre_usuario_normalizado      NVARCHAR(256)   NULL,
    correo                          NVARCHAR(256)   NULL,
    correo_normalizado              NVARCHAR(256)   NULL,
    correo_confirmado               BIT             NOT NULL DEFAULT 0,
    contrasena_hash                 NVARCHAR(MAX)   NULL,
    telefono                        NVARCHAR(30)    NULL,
    telefono_confirmado             BIT             NOT NULL DEFAULT 0,
    doble_factor_habilitado         BIT             NOT NULL DEFAULT 0,
    bloqueo_hasta                   DATETIMEOFFSET  NULL,
    bloqueo_habilitado              BIT             NOT NULL DEFAULT 1,
    intentos_fallidos               INT             NOT NULL DEFAULT 0,
    security_stamp                  NVARCHAR(MAX)   NULL,
    concurrency_stamp                NVARCHAR(MAX)   NULL
);
CREATE UNIQUE INDEX IX_Usuarios_NombreUsuarioNormalizado ON Usuarios(nombre_usuario_normalizado) WHERE nombre_usuario_normalizado IS NOT NULL;
CREATE INDEX IX_Usuarios_CorreoNormalizado ON Usuarios(correo_normalizado);
GO

CREATE TABLE Roles (
    id_rol                   INT IDENTITY(1,1) PRIMARY KEY,
    nombre                   NVARCHAR(256)   NULL,
    nombre_normalizado       NVARCHAR(256)   NULL,
    descripcion              NVARCHAR(250)   NULL,
    ConcurrencyStamp         NVARCHAR(MAX)   NULL
);
CREATE UNIQUE INDEX IX_Roles_NombreNormalizado ON Roles(nombre_normalizado) WHERE nombre_normalizado IS NOT NULL;
GO

CREATE TABLE Usuarios_Roles (
    UserId  INT NOT NULL,
    RoleId  INT NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES Roles(id_rol) ON DELETE CASCADE
);
GO

CREATE TABLE Usuarios_Claims (
    Id           INT IDENTITY(1,1) PRIMARY KEY,
    UserId       INT NOT NULL,
    ClaimType    NVARCHAR(MAX) NULL,
    ClaimValue   NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserId) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE
);
GO

CREATE TABLE Roles_Claims (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    RoleId      INT NOT NULL,
    ClaimType   NVARCHAR(MAX) NULL,
    ClaimValue  NVARCHAR(MAX) NULL,
    FOREIGN KEY (RoleId) REFERENCES Roles(id_rol) ON DELETE CASCADE
);
GO

CREATE TABLE Usuarios_Logins (
    LoginProvider        NVARCHAR(450) NOT NULL,
    ProviderKey           NVARCHAR(450) NOT NULL,
    ProviderDisplayName   NVARCHAR(MAX) NULL,
    UserId                INT NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE
);
GO

CREATE TABLE Usuarios_Tokens (
    UserId          INT NOT NULL,
    LoginProvider   NVARCHAR(450) NOT NULL,
    Name            NVARCHAR(450) NOT NULL,
    Value           NVARCHAR(MAX) NULL,
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES Usuarios(id_usuario) ON DELETE CASCADE
);
GO

-- Tabla de auditoría para los accesos al sistema
CREATE TABLE Accesos_Auditoria (
    id_auditoria         INT IDENTITY(1,1) PRIMARY KEY,
    id_usuario           INT             NULL,
    correo_intentado     NVARCHAR(256)   NOT NULL,
    exitoso              BIT             NOT NULL,
    motivo               NVARCHAR(200)   NULL,
    ip_address           NVARCHAR(45)    NULL,
    fecha                DATETIME2       NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario) ON DELETE SET NULL
);
GO

-- Cambios en las otras tablas que usaban usuario fijo
-- Tabla de Ordenes_Trabajo_Historial
ALTER TABLE Ordenes_Trabajo_Historial DROP COLUMN usuario;
GO

ALTER TABLE Ordenes_Trabajo_Historial ADD id_usuario INT NULL;
ALTER TABLE Ordenes_Trabajo_Historial
    ADD CONSTRAINT FK_OrdenesTrabajoHistorial_Usuarios
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario) ON DELETE SET NULL;
GO

-- Tabla de Ordenes_Trabajo_Notas
ALTER TABLE Ordenes_Trabajo_Notas DROP COLUMN usuario;
GO

ALTER TABLE Ordenes_Trabajo_Notas ADD id_usuario INT NULL;
ALTER TABLE Ordenes_Trabajo_Notas
    ADD CONSTRAINT FK_OrdenesTrabajoNotas_Usuarios
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario) ON DELETE SET NULL;
GO

-- Tabla de Clientes_Historial
ALTER TABLE Clientes_Historial DROP COLUMN usuario;
GO

ALTER TABLE Clientes_Historial ADD id_usuario INT NULL;
ALTER TABLE Clientes_Historial
    ADD CONSTRAINT FK_ClientesHistorial_Usuarios
    FOREIGN KEY (id_usuario) REFERENCES Usuarios(id_usuario) ON DELETE SET NULL;
GO

CREATE TABLE dbo.Cotizaciones
(
    id_cotizacion INT IDENTITY(1,1) NOT NULL,

    numero_cotizacion NVARCHAR(20) NOT NULL,
    fecha DATETIME2 NOT NULL DEFAULT(GETDATE()),

    -- Cliente
    id_cliente INT NULL,
    nombre_cliente NVARCHAR(200) NOT NULL,
    compania NVARCHAR(200) NULL,
    contacto NVARCHAR(200) NULL,
    correo_cliente NVARCHAR(200) NULL,
    telefono_celular NVARCHAR(30) NULL,

    -- Servicio
    tipo_servicio NVARCHAR(50) NOT NULL DEFAULT('Puerta a Puerta'),
    origen NVARCHAR(200) NULL,
    destino NVARCHAR(200) NULL,
    volumen_m3 DECIMAL(10,2) NULL,
    tipo_contenedor NVARCHAR(30) NULL,
    compania_maritima NVARCHAR(100) NULL,
    corresponsal NVARCHAR(100) NULL,

    -- Cronograma
    dias_empaque INT NULL,
    dias_transito INT NULL,
    dias_desalmacenaje INT NULL,
    dias_frecuencia_salidas INT NULL,

    -- Costos
    costo_origen DECIMAL(18,2) NOT NULL DEFAULT(0),
    costo_tramites_aduana DECIMAL(18,2) NOT NULL DEFAULT(0),
    costo_flete DECIMAL(18,2) NOT NULL DEFAULT(0),
    costo_destino DECIMAL(18,2) NOT NULL DEFAULT(0),

    -- Seguro
    incluye_seguro BIT NOT NULL DEFAULT(0),
    valor_declarado DECIMAL(18,2) NULL,
    porcentaje_seguro DECIMAL(5,2) NOT NULL DEFAULT(3.5),

    -- Totales
    subtotal DECIMAL(18,2) NOT NULL DEFAULT(0),
    monto_seguro DECIMAL(18,2) NOT NULL DEFAULT(0),
    tarifa_total DECIMAL(18,2) NOT NULL DEFAULT(0),
    moneda NVARCHAR(3) NOT NULL DEFAULT('USD'),

    -- Condiciones
    vigencia_dias INT NOT NULL DEFAULT(60),
    forma_pago NVARCHAR(200) NULL,
    exclusiones NVARCHAR(MAX) NULL,
    observaciones NVARCHAR(MAX) NULL,

    -- Estado
    estado NVARCHAR(20) NOT NULL DEFAULT('Borrador'),
    fecha_envio DATETIME2 NULL,
    correo_envio NVARCHAR(200) NULL,

    -- Conversión a OT
    id_orden_generada INT NULL,

    -- Auditoría
    hecho_por NVARCHAR(100) NULL,
    id_usuario INT NULL,
    fecha_creacion DATETIME2 NOT NULL DEFAULT(GETDATE()),
    fecha_actualizacion DATETIME2 NULL,

    CONSTRAINT PK_Cotizaciones
        PRIMARY KEY (id_cotizacion)
);
GO

CREATE UNIQUE INDEX IX_Cotizaciones_NumeroCotizacion
ON dbo.Cotizaciones(numero_cotizacion);
GO

ALTER TABLE dbo.Cotizaciones
ADD CONSTRAINT FK_Cotizaciones_Clientes
FOREIGN KEY(id_cliente)
REFERENCES dbo.Clientes(id_cliente);
GO

ALTER TABLE dbo.Cotizaciones
ADD CONSTRAINT FK_Cotizaciones_OrdenTrabajo
FOREIGN KEY(id_orden_generada)
REFERENCES dbo.Ordenes_Trabajo(id_orden);
GO

ALTER TABLE dbo.Cotizaciones
ADD CONSTRAINT FK_Cotizaciones_Usuarios
FOREIGN KEY(id_usuario)
REFERENCES dbo.Usuarios(id_usuario);
GO

-- =========================================================
-- ROLES AUDITORIA (HU-AUT-003)
-- =========================================================
CREATE TABLE Roles_Auditoria (
    id_auditoria              INT IDENTITY(1,1) PRIMARY KEY,
    accion                    NVARCHAR(50)    NOT NULL,
    nombre_rol                NVARCHAR(256)   NOT NULL,
    id_usuario_afectado       INT             NULL,
    id_usuario_responsable    INT             NULL,
    detalle                   NVARCHAR(MAX)   NULL,
    fecha                     DATETIME2       NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY (id_usuario_afectado) REFERENCES Usuarios(id_usuario) ON DELETE SET NULL,
    FOREIGN KEY (id_usuario_responsable) REFERENCES Usuarios(id_usuario) ON DELETE NO ACTION
);
GO

-- Inserts iniciales para la tabla de roles de Identity
INSERT INTO Roles (nombre, nombre_normalizado, descripcion, ConcurrencyStamp)
VALUES
    ('Administrador', 'ADMINISTRADOR', 'Acceso total al sistema', NEWID()),
    ('Empleado', 'EMPLEADO', 'Acceso operativo a órdenes, clientes e importaciones/exportaciones', NEWID()),
    ('SinRol', 'SINROL', 'Rol temporal para usuarios sin un rol asignado (por eliminación de su rol anterior)', NEWID());
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
-- Se incluye 'activo' explícitamente: el esquema generado por EF no define
-- un DEFAULT para esta columna, por lo que el seed debe proveer el valor.
INSERT INTO Catalogo_Documentos (nombre, aplica_exportacion, aplica_importacion, aplica_winmovers, aplica_otro_agente, orden_presentacion, activo) VALUES
    ('Reporte de Visita Previa',            1, 0, 1, 1, 1,  1),
    ('Cotización',                          1, 1, 1, 0, 2,  1),
    ('Lista de inventario para el seguro',  1, 1, 1, 1, 3,  1),
    ('Cotización con firma de aceptación',  1, 1, 1, 0, 4,  1),
    ('Hoja de Trabajo',                     1, 1, 1, 1, 5,  1),
    ('Pre-Aviso al agente de destino',      1, 0, 1, 1, 6,  1),
    ('Instrucciones del Embarque',          1, 1, 1, 1, 7,  1),
    ('Carte de porte, AWA o B-L',           1, 1, 1, 1, 8,  1),
    ('Certificado del seguro',              1, 1, 1, 1, 9,  1),
    ('Lista de empaque firmada',            1, 1, 1, 1, 10, 1),
    ('Factura',                             1, 1, 1, 1, 11, 1),
    ('Confirmación de Entrega',             1, 1, 1, 1, 12, 1);
GO

-- Procedimientos almacenados para subir archivos a importaciones
-- Insertar archivo
CREATE PROCEDURE sp_ImportacionArchivo_Insertar
    @id_importacion     INT,
    @nombre_original    NVARCHAR(255),
    @nombre_guardado    NVARCHAR(255),
    @tipo_mime          NVARCHAR(100),
    @tamanio_bytes      BIGINT
AS
BEGIN
    INSERT INTO Importaciones_Archivos 
        (id_importacion, nombre_original, nombre_guardado, tipo_mime, tamanio_bytes)
    VALUES 
        (@id_importacion, @nombre_original, @nombre_guardado, @tipo_mime, @tamanio_bytes);

    SELECT SCOPE_IDENTITY() AS id_archivo;
END
GO

-- Obtener archivos por importación
CREATE PROCEDURE sp_ImportacionArchivo_ObtenerPorImportacion
    @id_importacion INT
AS
BEGIN
    SELECT id_archivo, id_importacion, nombre_original, nombre_guardado,
           tipo_mime, tamanio_bytes, fecha_carga
    FROM Importaciones_Archivos
    WHERE id_importacion = @id_importacion
    ORDER BY fecha_carga DESC;
END
GO

-- Obtener un archivo por ID (para descarga)
CREATE PROCEDURE sp_ImportacionArchivo_ObtenerPorId
    @id_archivo INT
AS
BEGIN
    SELECT id_archivo, id_importacion, nombre_original, nombre_guardado,
           tipo_mime, tamanio_bytes, fecha_carga
    FROM Importaciones_Archivos
    WHERE id_archivo = @id_archivo;
END
GO

-- Eliminar archivo
CREATE PROCEDURE sp_ImportacionArchivo_Eliminar
    @id_archivo INT
AS
BEGIN
    SELECT nombre_guardado FROM Importaciones_Archivos WHERE id_archivo = @id_archivo;
    DELETE FROM Importaciones_Archivos WHERE id_archivo = @id_archivo;
END
GO

-- Procedimientos almacenados para subir archivos a exportaciones
CREATE PROCEDURE sp_ExportacionArchivo_Insertar
    @id_exportacion INT, @nombre_original NVARCHAR(255),
    @nombre_guardado NVARCHAR(255), @tipo_mime NVARCHAR(100), @tamanio_bytes BIGINT
AS
BEGIN
    INSERT INTO Exportaciones_Archivos
        (id_exportacion, nombre_original, nombre_guardado, tipo_mime, tamanio_bytes)
    VALUES (@id_exportacion, @nombre_original, @nombre_guardado, @tipo_mime, @tamanio_bytes);
    SELECT SCOPE_IDENTITY() AS id_archivo;
END
GO

CREATE PROCEDURE sp_ExportacionArchivo_Eliminar
    @id_archivo INT
AS
BEGIN
    SELECT nombre_guardado FROM Exportaciones_Archivos WHERE id_archivo = @id_archivo;
    DELETE FROM Exportaciones_Archivos WHERE id_archivo = @id_archivo;
END
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

-- Procedimientos almacenados para el historial
CREATE PROCEDURE sp_OrdenHistorial_Insertar
    @id_orden          INT,
    @campo_modificado  NVARCHAR(50),
    @valor_anterior    NVARCHAR(100) = NULL,
    @valor_nuevo       NVARCHAR(100) = NULL,
    @id_usuario        INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Ordenes_Trabajo_Historial
        (id_orden, campo_modificado, valor_anterior, valor_nuevo, id_usuario)
    VALUES
        (@id_orden, @campo_modificado, @valor_anterior, @valor_nuevo, @id_usuario);
END
GO

CREATE PROCEDURE sp_OrdenHistorial_ObtenerPorOrden
    @id_orden INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Ordenes_Trabajo_Historial
    WHERE id_orden = @id_orden
    ORDER BY fecha_cambio DESC;
END
GO

-- Procedimientos almacenados para validar conflicto de fecha/hora
CREATE PROCEDURE sp_OrdenTrabajo_ValidarConflicto
    @id_orden        INT,           -- la orden actual (para excluirla)
    @fecha_servicio  DATE,
    @hora            NVARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT id_orden, numero_ot, nombre_cliente
    FROM Ordenes_Trabajo
    WHERE fecha_servicio = @fecha_servicio
      AND hora = @hora
      AND id_orden <> @id_orden;
END
GO

-- Procedimientos almacenados para las notas
CREATE PROCEDURE sp_OrdenNota_Insertar
    @id_orden    INT,
    @contenido   NVARCHAR(MAX),
    @id_usuario  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO Ordenes_Trabajo_Notas (id_orden, contenido, id_usuario)
    VALUES (@id_orden, @contenido, @id_usuario);

    SELECT SCOPE_IDENTITY() AS id_nota;
END
GO

CREATE PROCEDURE sp_OrdenNota_Actualizar
    @id_nota     INT,
    @contenido   NVARCHAR(MAX),
    @id_usuario  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Ordenes_Trabajo_Notas SET
        contenido = @contenido,
        id_usuario = @id_usuario,
        fecha_actualizacion = GETDATE()
    WHERE id_nota = @id_nota;
END
GO

CREATE PROCEDURE sp_OrdenNota_ObtenerPorOrden
    @id_orden INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Ordenes_Trabajo_Notas
    WHERE id_orden = @id_orden
    ORDER BY fecha_creacion DESC;
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
    @cajas              INT,
    @kilos              DECIMAL(18,2),
    @referencia         NVARCHAR(50) = NULL,
    @fecha              DATE = NULL,
    @observaciones      NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Exportaciones
    (
        nombre_cliente,
        cajas,
        kilos,
        referencia,
        fecha,
        observaciones,
        fecha_creacion
    )
    VALUES
    (
        @nombre_cliente,
        @cajas,
        @kilos,
        @referencia,
        @fecha,
        @observaciones,
        GETDATE()
    );

    DECLARE @id_exportacion INT = SCOPE_IDENTITY();

    INSERT INTO Exportaciones_Documentos
        (id_exportacion, id_tipo_documento, tipo_checklist)
    SELECT @id_exportacion, id_tipo_documento, 'WinMovers'
    FROM Catalogo_Documentos
    WHERE aplica_exportacion = 1
      AND aplica_winmovers = 1
      AND activo = 1;

    INSERT INTO Exportaciones_Documentos
        (id_exportacion, id_tipo_documento, tipo_checklist)
    SELECT @id_exportacion, id_tipo_documento, 'OtroAgente'
    FROM Catalogo_Documentos
    WHERE aplica_exportacion = 1
      AND aplica_otro_agente = 1
      AND activo = 1;
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
    @pais               NVARCHAR(100),
    @cajas              INT,
    @kilos              DECIMAL(18,2),
    @referencia         NVARCHAR(50) = NULL,
    @fecha              DATE = NULL,
    @observaciones      NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO Importaciones
    (
        nombre_cliente,
        pais,
        cajas,
        kilos,
        referencia,
        fecha,
        observaciones,
        fecha_creacion
    )
    VALUES
    (
        @nombre_cliente,
        @pais,
        @cajas,
        @kilos,
        @referencia,
        @fecha,
        @observaciones,
        GETDATE()
    );

    DECLARE @id_importacion INT = SCOPE_IDENTITY();

    -- Checklist WinMovers
    INSERT INTO Importaciones_Documentos
        (id_importacion, id_tipo_documento, tipo_checklist)
    SELECT
        @id_importacion,
        id_tipo_documento,
        'WinMovers'
    FROM Catalogo_Documentos
    WHERE aplica_importacion = 1
      AND aplica_winmovers = 1
      AND activo = 1;

    -- Checklist Otro Agente
    INSERT INTO Importaciones_Documentos
        (id_importacion, id_tipo_documento, tipo_checklist)
    SELECT
        @id_importacion,
        id_tipo_documento,
        'OtroAgente'
    FROM Catalogo_Documentos
    WHERE aplica_importacion = 1
      AND aplica_otro_agente = 1
      AND activo = 1;

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
