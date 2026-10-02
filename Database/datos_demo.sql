-- =====================================================================
-- WinMovers - Datos de demostración
-- =====================================================================
-- Puebla todos los módulos con datos realistas para poder ver las
-- pantallas y las estadísticas funcionando.
--
--   * Es REPETIBLE: borra lo que sembró antes y vuelve a insertar.
--   * NO toca Usuarios, Roles ni las auditorías de seguridad.
--   * El historial cubre 12 meses hacia atrás porque el pronóstico
--     (DashboardsController.Pronostico) exige al menos 6 meses con
--     movimientos de importación/exportación.
--
-- Uso:
--   sqlcmd -S <servidor> -d WinMoversDB -E -C -I -f 65001 -i datos_demo.sql
--
--   El -f 65001 NO es opcional: este archivo está en UTF-8 y sqlcmd por
--   defecto lo lee con la página de códigos ANSI del sistema, lo que
--   guarda "España" como "EspaÃ±a". Lo mismo aplica a
--   WinMovers_Database.sql, que también tiene texto acentuado.
-- =====================================================================
SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @hoy DATE = CAST(GETDATE() AS DATE);
DECLARE @mes0 DATE = DATEFROMPARTS(YEAR(@hoy), MONTH(@hoy), 1);  -- mes actual

BEGIN TRANSACTION;

-- ---------------------------------------------------------------------
-- 1. Limpieza (orden inverso a las dependencias)
-- ---------------------------------------------------------------------
DELETE FROM ListasEmbalajeDetalle;
DELETE FROM ListasEmbalaje;
DELETE FROM BienesMudanza;
DELETE FROM OrdenTrabajoMaterial;
DELETE FROM Importaciones_Documentos;
DELETE FROM Exportaciones_Documentos;
DELETE FROM Ordenes_Trabajo_Notas;
DELETE FROM Ordenes_Trabajo_Historial;
DELETE FROM Clientes_Historial;
DELETE FROM Importaciones;
DELETE FROM Exportaciones;
DELETE FROM Cotizaciones;
DELETE FROM Control_Visitas;
DELETE FROM Ordenes_Trabajo;
DELETE FROM Inventario;
DELETE FROM Clientes;
DELETE FROM Catalogo_Documentos;

-- ---------------------------------------------------------------------
-- 2. Catálogo de documentos (checklists de importación / exportación)
-- ---------------------------------------------------------------------
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

-- ---------------------------------------------------------------------
-- 3. Clientes
-- ---------------------------------------------------------------------
INSERT INTO Clientes (nombre_cliente, telefono_celular, telefono_residencia, telefono_empresa,
                      empresa, contacto, correo_electronico, direccion, observaciones,
                      activo, fecha_registro, fecha_creacion)
VALUES
 ('Familia Jiménez Rojas',   '8812-4455','2225-1180',NULL,        NULL,                        NULL,             'jimenezrojas@gmail.com',   'Curridabat, San José, 300 m sur del parque',        'Mudanza familiar completa.',            1, DATEADD(MONTH,-11,@hoy), DATEADD(MONTH,-11,@hoy)),
 ('Marisol Vargas Céspedes', '8730-9014','2260-4471',NULL,        NULL,                        NULL,             'mvargas@outlook.com',      'Heredia centro, Ave 6, casa 22',                    NULL,                                    1, DATEADD(MONTH,-10,@hoy), DATEADD(MONTH,-10,@hoy)),
 ('Intel Costa Rica',        '8890-1200',NULL,       '2298-6000', 'Intel Costa Rica',          'Laura Quesada',  'lquesada@intel-cr.com',    'La Ribera de Belén, Heredia, Zona Franca',          'Cuenta corporativa, factura a 30 días.',1, DATEADD(MONTH,-10,@hoy), DATEADD(MONTH,-10,@hoy)),
 ('Roberto Solano Mora',     '8455-3321',NULL,       NULL,        NULL,                        NULL,             'rsolano@gmail.com',        'Escazú, San Rafael, Condominio Vistas del Valle',   NULL,                                    1, DATEADD(MONTH,-9, @hoy), DATEADD(MONTH,-9, @hoy)),
 ('Embajada de Canadá',      '8801-7788',NULL,       '2242-4400', 'Embajada de Canadá',        'Pierre Gagnon',  'consular@canada-cr.org',   'Sabana Sur, Edificio Oficentro Ejecutivo, piso 5',  'Traslados diplomáticos.',               1, DATEADD(MONTH,-8, @hoy), DATEADD(MONTH,-8, @hoy)),
 ('Ana Lucía Fernández',     '8377-2094','2494-3312',NULL,        NULL,                        NULL,             'alfernandez@hotmail.com',  'Alajuela, Barrio San José, 200 m este de la iglesia',NULL,                                   1, DATEADD(MONTH,-7, @hoy), DATEADD(MONTH,-7, @hoy)),
 ('Grupo Nación',            '8912-0043',NULL,       '2247-4747', 'Grupo Nación GN S.A.',      'Diego Ramírez',  'dramirez@nacion.com',      'Llorente de Tibás, San José',                       'Traslado de oficinas por etapas.',      1, DATEADD(MONTH,-6, @hoy), DATEADD(MONTH,-6, @hoy)),
 ('Familia Castro Umaña',    '8654-1122',NULL,       NULL,        NULL,                        NULL,             'castroumana@gmail.com',    'Cartago, Tres Ríos, Residencial El Roble',          NULL,                                    1, DATEADD(MONTH,-4, @hoy), DATEADD(MONTH,-4, @hoy)),
 ('Hospital CIMA',           '8700-5566',NULL,       '2208-1000', 'Hospital CIMA San José',    'Karla Induni',   'kinduni@cimacr.com',       'San Rafael de Escazú, Autopista Próspero Fernández','Equipo médico, requiere manejo especial.',1,DATEADD(MONTH,-3, @hoy), DATEADD(MONTH,-3, @hoy)),
 ('Javier Montero Alfaro',   '8221-6699',NULL,       NULL,        NULL,                        NULL,             'jmontero@gmail.com',       'Santa Ana, Pozos, Condominio Río Oro',              'Cliente referido por R. Solano.',       1, DATEADD(MONTH,-1, @hoy), DATEADD(MONTH,-1, @hoy));

-- ---------------------------------------------------------------------
-- 4. Inventario de materiales de embalaje
-- ---------------------------------------------------------------------
INSERT INTO Inventario (nombre_material, descripcion, categoria, unidad, existencias, stock_minimo, fecha_creacion)
VALUES
 ('Caja pequeña 40x30x30',   'Cartón corrugado doble, para libros y objetos pesados', 'Embalaje',  'Unidad', 240, 50, DATEADD(MONTH,-11,@hoy)),
 ('Caja mediana 50x40x40',   'Cartón corrugado, uso general',                         'Embalaje',  'Unidad', 185, 40, DATEADD(MONTH,-11,@hoy)),
 ('Caja grande 60x50x50',    'Cartón corrugado, ropa y almohadas',                    'Embalaje',  'Unidad', 120, 30, DATEADD(MONTH,-11,@hoy)),
 ('Caja portatrajes',        'Con tubo colgador incorporado',                         'Embalaje',  'Unidad',  35, 10, DATEADD(MONTH,-10,@hoy)),
 ('Papel manila',            'Resma para envolver vajilla y cristalería',             'Protección','Resma',   64, 15, DATEADD(MONTH,-10,@hoy)),
 ('Plástico burbuja',        'Rollo de 1.2 m x 100 m',                                'Protección','Rollo',   28,  8, DATEADD(MONTH,-9, @hoy)),
 ('Manta de mudanza',        'Cobertor acolchado para muebles',                       'Protección','Unidad',  75, 20, DATEADD(MONTH,-9, @hoy)),
 ('Cinta adhesiva 48mm',     'Rollo transparente de 100 m',                           'Sellado',   'Rollo',  150, 40, DATEADD(MONTH,-8, @hoy)),
 ('Cinta "Frágil"',          'Rollo impreso de advertencia',                          'Sellado',   'Rollo',   42, 12, DATEADD(MONTH,-8, @hoy)),
 ('Film stretch',            'Rollo para paletizado y protección de muebles',         'Protección','Rollo',   36, 10, DATEADD(MONTH,-6, @hoy)),
 ('Etiquetas de inventario', 'Bloque de 500 etiquetas numeradas',                     'Marcaje',   'Bloque',  18,  5, DATEADD(MONTH,-5, @hoy)),
 ('Marcador permanente',     'Punta gruesa, negro',                                   'Marcaje',   'Unidad',   9, 12, DATEADD(MONTH,-5, @hoy));  -- bajo el mínimo, a propósito

-- ---------------------------------------------------------------------
-- 5. Órdenes de trabajo (12 meses, mezcla de estados)
-- ---------------------------------------------------------------------
DECLARE @cJimenez INT = (SELECT id_cliente FROM Clientes WHERE nombre_cliente = 'Familia Jiménez Rojas');
DECLARE @cVargas  INT = (SELECT id_cliente FROM Clientes WHERE nombre_cliente = 'Marisol Vargas Céspedes');
DECLARE @cIntel   INT = (SELECT id_cliente FROM Clientes WHERE nombre_cliente = 'Intel Costa Rica');
DECLARE @cSolano  INT = (SELECT id_cliente FROM Clientes WHERE nombre_cliente = 'Roberto Solano Mora');
DECLARE @cEmbCan  INT = (SELECT id_cliente FROM Clientes WHERE nombre_cliente = 'Embajada de Canadá');
DECLARE @cAnaL    INT = (SELECT id_cliente FROM Clientes WHERE nombre_cliente = 'Ana Lucía Fernández');
DECLARE @cNacion  INT = (SELECT id_cliente FROM Clientes WHERE nombre_cliente = 'Grupo Nación');
DECLARE @cCastro  INT = (SELECT id_cliente FROM Clientes WHERE nombre_cliente = 'Familia Castro Umaña');
DECLARE @cCima    INT = (SELECT id_cliente FROM Clientes WHERE nombre_cliente = 'Hospital CIMA');
DECLARE @cMontero INT = (SELECT id_cliente FROM Clientes WHERE nombre_cliente = 'Javier Montero Alfaro');

INSERT INTO Ordenes_Trabajo (numero_ot, fecha_servicio, fecha, hora, nombre_cliente, telefono_celular,
                             compania, contacto, direccion_origen, direccion_destino, detalle_servicio,
                             facturar_a, hecho_por, fecha_creacion, estado, id_cliente)
VALUES
 ('OT-2025-0112', DATEADD(MONTH,-11,@hoy), DATEADD(MONTH,-11,@hoy), '08:00', 'Familia Jiménez Rojas',  '8812-4455', NULL,                     NULL,            'Curridabat, San José',            'Grecia, Alajuela',                 'Mudanza local completa, 3 habitaciones.',      'Familia Jiménez Rojas', 'C. Mora',  DATEADD(MONTH,-11,@hoy), 'Completado', @cJimenez),
 ('OT-2025-0118', DATEADD(MONTH,-10,@hoy), DATEADD(MONTH,-10,@hoy), '07:30', 'Intel Costa Rica',       '8890-1200', 'Intel Costa Rica',       'Laura Quesada', 'Zona Franca Belén, Heredia',      'Miami, FL, Estados Unidos',        'Traslado internacional de equipo de oficina.', 'Intel Costa Rica',      'A. Solís', DATEADD(MONTH,-10,@hoy), 'Completado', @cIntel),
 ('OT-2025-0124', DATEADD(MONTH,-10,@hoy), DATEADD(MONTH,-10,@hoy), '09:00', 'Marisol Vargas Céspedes','8730-9014', NULL,                     NULL,            'Heredia centro',                  'Santo Domingo, Heredia',           'Mudanza local, apartamento 2 habitaciones.',   'Marisol Vargas',        'C. Mora',  DATEADD(MONTH,-10,@hoy), 'Completado', @cVargas),
 ('OT-2025-0131', DATEADD(MONTH,-9, @hoy), DATEADD(MONTH,-9, @hoy), '08:00', 'Roberto Solano Mora',    '8455-3321', NULL,                     NULL,            'Escazú, San José',                'Madrid, España',                   'Embarque marítimo puerta a puerta.',           'Roberto Solano',        'A. Solís', DATEADD(MONTH,-9, @hoy), 'Completado', @cSolano),
 ('OT-2025-0140', DATEADD(MONTH,-8, @hoy), DATEADD(MONTH,-8, @hoy), '10:00', 'Embajada de Canadá',     '8801-7788', 'Embajada de Canadá',     'Pierre Gagnon', 'Sabana Sur, San José',            'Ottawa, Canadá',                   'Traslado diplomático, menaje completo.',       'Embajada de Canadá',    'C. Mora',  DATEADD(MONTH,-8, @hoy), 'Completado', @cEmbCan),
 ('OT-2025-0147', DATEADD(MONTH,-7, @hoy), DATEADD(MONTH,-7, @hoy), '07:00', 'Ana Lucía Fernández',    '8377-2094', NULL,                     NULL,            'Alajuela centro',                 'San Ramón, Alajuela',              'Mudanza local con empaque incluido.',          'Ana L. Fernández',      'J. Pérez', DATEADD(MONTH,-7, @hoy), 'Completado', @cAnaL),
 ('OT-2025-0155', DATEADD(MONTH,-6, @hoy), DATEADD(MONTH,-6, @hoy), '06:30', 'Grupo Nación',           '8912-0043', 'Grupo Nación GN S.A.',   'Diego Ramírez', 'Llorente de Tibás, San José',     'Barreal de Heredia',               'Traslado de oficinas, etapa 1 de 3.',          'Grupo Nación GN S.A.',  'A. Solís', DATEADD(MONTH,-6, @hoy), 'Completado', @cNacion),
 ('OT-2026-0002', DATEADD(MONTH,-5, @hoy), DATEADD(MONTH,-5, @hoy), '08:00', 'Grupo Nación',           '8912-0043', 'Grupo Nación GN S.A.',   'Diego Ramírez', 'Llorente de Tibás, San José',     'Barreal de Heredia',               'Traslado de oficinas, etapa 2 de 3.',          'Grupo Nación GN S.A.',  'A. Solís', DATEADD(MONTH,-5, @hoy), 'Completado', @cNacion),
 ('OT-2026-0009', DATEADD(MONTH,-4, @hoy), DATEADD(MONTH,-4, @hoy), '09:30', 'Familia Castro Umaña',   '8654-1122', NULL,                     NULL,            'Tres Ríos, Cartago',              'Pérez Zeledón, San José',          'Mudanza local, casa de 4 habitaciones.',       'Familia Castro Umaña',  'C. Mora',  DATEADD(MONTH,-4, @hoy), 'Completado', @cCastro),
 ('OT-2026-0015', DATEADD(MONTH,-3, @hoy), DATEADD(MONTH,-3, @hoy), '07:00', 'Hospital CIMA',          '8700-5566', 'Hospital CIMA San José', 'Karla Induni',  'Escazú, San José',                'Liberia, Guanacaste',              'Traslado de equipo médico especializado.',     'Hospital CIMA San José','J. Pérez', DATEADD(MONTH,-3, @hoy), 'Completado', @cCima),
 ('OT-2026-0021', DATEADD(MONTH,-2, @hoy), DATEADD(MONTH,-2, @hoy), '08:00', 'Intel Costa Rica',       '8890-1200', 'Intel Costa Rica',       'Laura Quesada', 'Zona Franca Belén, Heredia',      'Guadalajara, México',              'Embarque aéreo de laboratorio.',               'Intel Costa Rica',      'A. Solís', DATEADD(MONTH,-2, @hoy), 'Completado', @cIntel),
 ('OT-2026-0028', DATEADD(MONTH,-1, @hoy), DATEADD(MONTH,-1, @hoy), '10:00', 'Javier Montero Alfaro',  '8221-6699', NULL,                     NULL,            'Santa Ana, San José',             'Tamarindo, Guanacaste',            'Mudanza local con bodegaje temporal.',         'Javier Montero',        'C. Mora',  DATEADD(MONTH,-1, @hoy), 'Completado', @cMontero),
 ('OT-2026-0033', DATEADD(DAY, -12,@hoy), DATEADD(DAY, -12,@hoy), '07:30', 'Grupo Nación',           '8912-0043', 'Grupo Nación GN S.A.',   'Diego Ramírez', 'Llorente de Tibás, San José',     'Barreal de Heredia',               'Traslado de oficinas, etapa 3 de 3.',          'Grupo Nación GN S.A.',  'A. Solís', DATEADD(DAY, -12,@hoy), 'Pendiente',  @cNacion),
 ('OT-2026-0034', DATEADD(DAY,  -6,@hoy), DATEADD(DAY,  -6,@hoy), '08:00', 'Familia Jiménez Rojas',  '8812-4455', NULL,                     NULL,            'Grecia, Alajuela',                'Atenas, Alajuela',                 'Mudanza local, segunda etapa.',                'Familia Jiménez Rojas', 'C. Mora',  DATEADD(DAY,  -6,@hoy), 'Pendiente',  @cJimenez),
 ('OT-2026-0035', DATEADD(DAY,   3,@hoy), DATEADD(DAY,  -2,@hoy), '09:00', 'Hospital CIMA',          '8700-5566', 'Hospital CIMA San José', 'Karla Induni',  'Escazú, San José',                'Cartago centro',                   'Traslado de mobiliario clínico.',              'Hospital CIMA San José','J. Pérez', DATEADD(DAY,  -2,@hoy), 'Pendiente',  @cCima),
 ('OT-2026-0036', DATEADD(DAY,   9,@hoy), @hoy,                    '07:00', 'Roberto Solano Mora',    '8455-3321', NULL,                     NULL,            'Madrid, España',                  'Escazú, San José',                 'Importación de menaje, retorno del cliente.',  'Roberto Solano',        'A. Solís', @hoy,                    'Pendiente',  @cSolano);

-- ---------------------------------------------------------------------
-- 6. Importaciones (12 meses de historial, con país / cajas / kilos)
-- ---------------------------------------------------------------------
INSERT INTO Importaciones (nombre_cliente, referencia, fecha, pais, cajas, kilos, observaciones, fecha_creacion)
VALUES
 ('Roberto Solano Mora',   'IMP-2025-041', DATEADD(MONTH,-11,@hoy), 'España',         48,  1240.50, 'Menaje de casa, contenedor 20 pies.',    DATEADD(MONTH,-11,@hoy)),
 ('Familia Jiménez Rojas', 'IMP-2025-043', DATEADD(MONTH,-11,@hoy), 'Estados Unidos', 32,   860.00, NULL,                                      DATEADD(MONTH,-11,@hoy)),
 ('Intel Costa Rica',      'IMP-2025-047', DATEADD(MONTH,-10,@hoy), 'Estados Unidos', 76,  2180.75, 'Equipo de laboratorio.',                  DATEADD(MONTH,-10,@hoy)),
 ('Embajada de Canadá',    'IMP-2025-052', DATEADD(MONTH,-10,@hoy), 'Canadá',         55,  1495.00, NULL,                                      DATEADD(MONTH,-10,@hoy)),
 ('Marisol Vargas Céspedes','IMP-2025-058',DATEADD(MONTH,-9, @hoy), 'México',         28,   705.25, NULL,                                      DATEADD(MONTH,-9, @hoy)),
 ('Hospital CIMA',         'IMP-2025-061', DATEADD(MONTH,-9, @hoy), 'Alemania',       64,  2940.00, 'Equipo médico, manejo especial.',         DATEADD(MONTH,-9, @hoy)),
 ('Grupo Nación',          'IMP-2025-066', DATEADD(MONTH,-8, @hoy), 'Estados Unidos', 41,  1080.00, NULL,                                      DATEADD(MONTH,-8, @hoy)),
 ('Ana Lucía Fernández',   'IMP-2025-070', DATEADD(MONTH,-8, @hoy), 'Panamá',         19,   430.50, NULL,                                      DATEADD(MONTH,-8, @hoy)),
 ('Intel Costa Rica',      'IMP-2025-074', DATEADD(MONTH,-7, @hoy), 'Estados Unidos', 88,  2610.00, 'Segunda fase de equipamiento.',           DATEADD(MONTH,-7, @hoy)),
 ('Familia Castro Umaña',  'IMP-2025-079', DATEADD(MONTH,-7, @hoy), 'España',         36,   940.00, NULL,                                      DATEADD(MONTH,-7, @hoy)),
 ('Embajada de Canadá',    'IMP-2025-083', DATEADD(MONTH,-6, @hoy), 'Canadá',         47,  1320.00, NULL,                                      DATEADD(MONTH,-6, @hoy)),
 ('Roberto Solano Mora',   'IMP-2025-088', DATEADD(MONTH,-6, @hoy), 'España',         52,  1405.50, NULL,                                      DATEADD(MONTH,-6, @hoy)),
 ('Hospital CIMA',         'IMP-2026-004', DATEADD(MONTH,-5, @hoy), 'Alemania',       71,  3180.00, 'Ampliación de sala de imágenes.',         DATEADD(MONTH,-5, @hoy)),
 ('Javier Montero Alfaro', 'IMP-2026-009', DATEADD(MONTH,-5, @hoy), 'Estados Unidos', 24,   615.00, NULL,                                      DATEADD(MONTH,-5, @hoy)),
 ('Intel Costa Rica',      'IMP-2026-013', DATEADD(MONTH,-4, @hoy), 'Estados Unidos', 93,  2875.25, NULL,                                      DATEADD(MONTH,-4, @hoy)),
 ('Marisol Vargas Céspedes','IMP-2026-018',DATEADD(MONTH,-4, @hoy), 'México',         31,   790.00, NULL,                                      DATEADD(MONTH,-4, @hoy)),
 ('Grupo Nación',          'IMP-2026-022', DATEADD(MONTH,-3, @hoy), 'Estados Unidos', 58,  1560.00, 'Mobiliario de oficina.',                  DATEADD(MONTH,-3, @hoy)),
 ('Embajada de Canadá',    'IMP-2026-027', DATEADD(MONTH,-3, @hoy), 'Canadá',         44,  1215.75, NULL,                                      DATEADD(MONTH,-3, @hoy)),
 ('Familia Castro Umaña',  'IMP-2026-031', DATEADD(MONTH,-2, @hoy), 'España',         39,  1025.00, NULL,                                      DATEADD(MONTH,-2, @hoy)),
 ('Hospital CIMA',         'IMP-2026-035', DATEADD(MONTH,-2, @hoy), 'Alemania',       67,  2760.00, NULL,                                      DATEADD(MONTH,-2, @hoy)),
 ('Intel Costa Rica',      'IMP-2026-040', DATEADD(MONTH,-1, @hoy), 'Estados Unidos', 84,  2495.50, NULL,                                      DATEADD(MONTH,-1, @hoy)),
 ('Ana Lucía Fernández',   'IMP-2026-044', DATEADD(MONTH,-1, @hoy), 'Panamá',         22,   540.00, NULL,                                      DATEADD(MONTH,-1, @hoy)),
 ('Roberto Solano Mora',   'IMP-2026-049', DATEADD(DAY, -9, @hoy),  'España',         57,  1520.00, 'Retorno del cliente a Costa Rica.',       DATEADD(DAY, -9, @hoy)),
 ('Javier Montero Alfaro', 'IMP-2026-052', DATEADD(DAY, -3, @hoy),  'Estados Unidos', 29,   735.00, NULL,                                      DATEADD(DAY, -3, @hoy));

-- ---------------------------------------------------------------------
-- 7. Exportaciones (12 meses de historial)
-- ---------------------------------------------------------------------
INSERT INTO Exportaciones (nombre_cliente, referencia, fecha, cajas, kilos, observaciones, fecha_creacion)
VALUES
 ('Intel Costa Rica',       'EXP-2025-030', DATEADD(MONTH,-11,@hoy), 62, 1780.00, 'Equipo de oficina hacia Miami.',      DATEADD(MONTH,-11,@hoy)),
 ('Familia Jiménez Rojas',  'EXP-2025-033', DATEADD(MONTH,-11,@hoy), 21,  530.50, NULL,                                  DATEADD(MONTH,-11,@hoy)),
 ('Embajada de Canadá',     'EXP-2025-037', DATEADD(MONTH,-10,@hoy), 58, 1640.00, 'Traslado diplomático a Ottawa.',      DATEADD(MONTH,-10,@hoy)),
 ('Marisol Vargas Céspedes','EXP-2025-041', DATEADD(MONTH,-10,@hoy), 26,  665.00, NULL,                                  DATEADD(MONTH,-10,@hoy)),
 ('Roberto Solano Mora',    'EXP-2025-045', DATEADD(MONTH,-9, @hoy), 49, 1310.25, 'Embarque marítimo a Madrid.',         DATEADD(MONTH,-9, @hoy)),
 ('Grupo Nación',           'EXP-2025-049', DATEADD(MONTH,-9, @hoy), 33,  870.00, NULL,                                  DATEADD(MONTH,-9, @hoy)),
 ('Intel Costa Rica',       'EXP-2025-054', DATEADD(MONTH,-8, @hoy), 71, 2040.00, NULL,                                  DATEADD(MONTH,-8, @hoy)),
 ('Ana Lucía Fernández',    'EXP-2025-058', DATEADD(MONTH,-8, @hoy), 17,  405.00, NULL,                                  DATEADD(MONTH,-8, @hoy)),
 ('Hospital CIMA',          'EXP-2025-063', DATEADD(MONTH,-7, @hoy), 44, 1890.00, 'Devolución de equipo en garantía.',   DATEADD(MONTH,-7, @hoy)),
 ('Familia Castro Umaña',   'EXP-2025-067', DATEADD(MONTH,-7, @hoy), 30,  780.00, NULL,                                  DATEADD(MONTH,-7, @hoy)),
 ('Intel Costa Rica',       'EXP-2025-072', DATEADD(MONTH,-6, @hoy), 79, 2310.50, NULL,                                  DATEADD(MONTH,-6, @hoy)),
 ('Embajada de Canadá',     'EXP-2025-076', DATEADD(MONTH,-6, @hoy), 41, 1150.00, NULL,                                  DATEADD(MONTH,-6, @hoy)),
 ('Grupo Nación',           'EXP-2026-003', DATEADD(MONTH,-5, @hoy), 37,  985.00, NULL,                                  DATEADD(MONTH,-5, @hoy)),
 ('Javier Montero Alfaro',  'EXP-2026-007', DATEADD(MONTH,-5, @hoy), 20,  495.00, NULL,                                  DATEADD(MONTH,-5, @hoy)),
 ('Intel Costa Rica',       'EXP-2026-012', DATEADD(MONTH,-4, @hoy), 86, 2540.00, 'Embarque aéreo a Guadalajara.',       DATEADD(MONTH,-4, @hoy)),
 ('Roberto Solano Mora',    'EXP-2026-016', DATEADD(MONTH,-4, @hoy), 34,  895.00, NULL,                                  DATEADD(MONTH,-4, @hoy)),
 ('Hospital CIMA',          'EXP-2026-021', DATEADD(MONTH,-3, @hoy), 52, 2130.00, NULL,                                  DATEADD(MONTH,-3, @hoy)),
 ('Marisol Vargas Céspedes','EXP-2026-025', DATEADD(MONTH,-3, @hoy), 23,  590.00, NULL,                                  DATEADD(MONTH,-3, @hoy)),
 ('Intel Costa Rica',       'EXP-2026-030', DATEADD(MONTH,-2, @hoy), 91, 2690.00, NULL,                                  DATEADD(MONTH,-2, @hoy)),
 ('Familia Castro Umaña',   'EXP-2026-034', DATEADD(MONTH,-2, @hoy), 27,  700.00, NULL,                                  DATEADD(MONTH,-2, @hoy)),
 ('Embajada de Canadá',     'EXP-2026-039', DATEADD(MONTH,-1, @hoy), 46, 1290.00, NULL,                                  DATEADD(MONTH,-1, @hoy)),
 ('Grupo Nación',           'EXP-2026-043', DATEADD(MONTH,-1, @hoy), 39, 1040.00, 'Etapa 3 del traslado de oficinas.',   DATEADD(MONTH,-1, @hoy)),
 ('Intel Costa Rica',       'EXP-2026-048', DATEADD(DAY, -7, @hoy),  68, 1975.00, NULL,                                  DATEADD(DAY, -7, @hoy)),
 ('Ana Lucía Fernández',    'EXP-2026-051', DATEADD(DAY, -2, @hoy),  18,  445.00, NULL,                                  DATEADD(DAY, -2, @hoy));

-- ---------------------------------------------------------------------
-- 8. Checklists de documentos (importación y exportación)
--    Se completan los primeros documentos de cada embarque y se dejan
--    pendientes los últimos, para que el avance se vea parcial.
-- ---------------------------------------------------------------------
INSERT INTO Importaciones_Documentos (id_importacion, id_tipo_documento, completado, fecha_completado, tipo_checklist, observaciones)
SELECT i.id_importacion,
       d.id_tipo_documento,
       CASE WHEN d.orden_presentacion <= 7 THEN 1 ELSE 0 END,
       CASE WHEN d.orden_presentacion <= 7 THEN DATEADD(DAY, d.orden_presentacion, i.fecha) END,
       'WinMovers',
       NULL
FROM Importaciones i
CROSS JOIN Catalogo_Documentos d
WHERE d.aplica_importacion = 1 AND d.activo = 1;

INSERT INTO Exportaciones_Documentos (id_exportacion, id_tipo_documento, completado, fecha_completado, tipo_checklist, observaciones)
SELECT e.id_exportacion,
       d.id_tipo_documento,
       CASE WHEN d.orden_presentacion <= 8 THEN 1 ELSE 0 END,
       CASE WHEN d.orden_presentacion <= 8 THEN DATEADD(DAY, d.orden_presentacion, e.fecha) END,
       'WinMovers',
       NULL
FROM Exportaciones e
CROSS JOIN Catalogo_Documentos d
WHERE d.aplica_exportacion = 1 AND d.activo = 1;

-- ---------------------------------------------------------------------
-- 9. Control de visitas previas
-- ---------------------------------------------------------------------
INSERT INTO Control_Visitas (fecha_llamada, fecha_visita, hora, nombre_cliente, telefono_celular, telefono_habitacion,
                             empresa, direccion_origen, direccion_destino, observaciones,
                             puerta_a_puerta, puerta_a_puerto, empaque, mudanza_local,
                             origen, tramites_aduana, flete, destino, tarifa_total,
                             compania_maritima, corresponsal, hecho_por, fecha_creacion)
VALUES
 (DATEADD(MONTH,-11,@hoy), DATEADD(MONTH,-11,@hoy), '09:00','Familia Jiménez Rojas',  '8812-4455','2225-1180', NULL,                    'Curridabat, San José',     'Grecia, Alajuela',      'Casa de 3 habitaciones, sin piano.',        0,0,1,1, '$   350.00','$      0.00','$   420.00','$   180.00','$   950.00', NULL,           NULL,               'C. Mora',  DATEADD(MONTH,-11,@hoy)),
 (DATEADD(MONTH,-10,@hoy), DATEADD(MONTH,-10,@hoy), '10:30','Intel Costa Rica',       '8890-1200',NULL,        'Intel Costa Rica',      'Zona Franca Belén',        'Miami, FL, EE.UU.',     'Requiere inventario detallado por activo.', 1,0,1,0, '$ 1,850.00','$  980.00','$ 3,400.00','$ 1,120.00','$ 7,350.00','Maersk Line',  'Atlas Movers Miami','A. Solís', DATEADD(MONTH,-10,@hoy)),
 (DATEADD(MONTH,-9, @hoy), DATEADD(MONTH,-9, @hoy), '08:00','Roberto Solano Mora',    '8455-3321',NULL,        NULL,                    'Escazú, San José',         'Madrid, España',        'Contenedor de 20 pies compartido.',         1,0,1,0, '$ 1,200.00','$  640.00','$ 2,750.00','$   890.00','$ 5,480.00','MSC',          'Iberia Relocation','A. Solís', DATEADD(MONTH,-9, @hoy)),
 (DATEADD(MONTH,-8, @hoy), DATEADD(MONTH,-8, @hoy), '11:00','Embajada de Canadá',     '8801-7788',NULL,        'Embajada de Canadá',    'Sabana Sur, San José',     'Ottawa, Canadá',        'Valija diplomática aparte.',                1,0,1,0, '$ 1,600.00','$    0.00','$ 3,100.00','$ 1,050.00','$ 5,750.00','Hapag-Lloyd',  'Canada Moving',    'C. Mora',  DATEADD(MONTH,-8, @hoy)),
 (DATEADD(MONTH,-7, @hoy), DATEADD(MONTH,-7, @hoy), '07:30','Ana Lucía Fernández',    '8377-2094','2494-3312', NULL,                    'Alajuela centro',          'San Ramón, Alajuela',   NULL,                                        0,0,1,1, '$   280.00','$      0.00','$   310.00','$   140.00','$   730.00', NULL,           NULL,               'J. Pérez', DATEADD(MONTH,-7, @hoy)),
 (DATEADD(MONTH,-6, @hoy), DATEADD(MONTH,-6, @hoy), '06:30','Grupo Nación',           '8912-0043',NULL,        'Grupo Nación GN S.A.',  'Llorente de Tibás',        'Barreal de Heredia',    'Traslado por etapas, 3 fines de semana.',   0,0,1,1, '$   900.00','$      0.00','$ 1,150.00','$   480.00','$ 2,530.00', NULL,           NULL,               'A. Solís', DATEADD(MONTH,-6, @hoy)),
 (DATEADD(MONTH,-4, @hoy), DATEADD(MONTH,-4, @hoy), '09:30','Familia Castro Umaña',   '8654-1122',NULL,        NULL,                    'Tres Ríos, Cartago',       'Pérez Zeledón',         'Casa de 4 habitaciones con bodega.',        0,0,1,1, '$   520.00','$      0.00','$   640.00','$   260.00','$ 1,420.00', NULL,           NULL,               'C. Mora',  DATEADD(MONTH,-4, @hoy)),
 (DATEADD(MONTH,-3, @hoy), DATEADD(MONTH,-3, @hoy), '07:00','Hospital CIMA',          '8700-5566',NULL,        'Hospital CIMA San José','Escazú, San José',         'Liberia, Guanacaste',   'Equipo médico, requiere grúa.',             0,0,1,1, '$ 1,400.00','$      0.00','$ 1,900.00','$   700.00','$ 4,000.00', NULL,           NULL,               'J. Pérez', DATEADD(MONTH,-3, @hoy)),
 (DATEADD(MONTH,-2, @hoy), DATEADD(MONTH,-2, @hoy), '08:00','Intel Costa Rica',       '8890-1200',NULL,        'Intel Costa Rica',      'Zona Franca Belén',        'Guadalajara, México',   'Embarque aéreo urgente.',                   1,0,1,0, '$ 1,750.00','$  820.00','$ 4,200.00','$   980.00','$ 7,750.00','Copa Cargo',   'Movers MX',        'A. Solís', DATEADD(MONTH,-2, @hoy)),
 (DATEADD(MONTH,-1, @hoy), DATEADD(MONTH,-1, @hoy), '10:00','Javier Montero Alfaro',  '8221-6699',NULL,        NULL,                    'Santa Ana, San José',      'Tamarindo, Guanacaste', 'Requiere bodegaje por 2 semanas.',          0,0,1,1, '$   430.00','$      0.00','$   580.00','$   210.00','$ 1,220.00', NULL,           NULL,               'C. Mora',  DATEADD(MONTH,-1, @hoy)),
 (DATEADD(DAY, -10,@hoy),  DATEADD(DAY,  -4,@hoy),  '08:30','Marisol Vargas Céspedes','8730-9014','2260-4471', NULL,                    'Santo Domingo, Heredia',   'San Pablo, Heredia',    'Apartamento pequeño.',                      0,0,0,1, '$   190.00','$      0.00','$   240.00','$    90.00','$   520.00', NULL,           NULL,               'J. Pérez', DATEADD(DAY, -10,@hoy)),
 (DATEADD(DAY,  -5,@hoy),  DATEADD(DAY,   4,@hoy),  '09:00','Roberto Solano Mora',    '8455-3321',NULL,        NULL,                    'Madrid, España',           'Escazú, San José',      'Importación de retorno, visita virtual.',   1,0,0,0, '$ 1,100.00','$  700.00','$ 2,600.00','$   850.00','$ 5,250.00','MSC',          'Iberia Relocation','A. Solís', DATEADD(DAY,  -5,@hoy));

-- ---------------------------------------------------------------------
-- 10. Cotizaciones (los 5 tipos de servicio y los 5 estados)
-- ---------------------------------------------------------------------
INSERT INTO Cotizaciones (numero_cotizacion, fecha, id_cliente, nombre_cliente, compania, contacto, correo_cliente, telefono_celular,
                          tipo_servicio, origen, destino, volumen_m3, tipo_contenedor, compania_maritima, corresponsal,
                          dias_empaque, dias_transito, dias_desalmacenaje, dias_frecuencia_salidas,
                          costo_origen, costo_tramites_aduana, costo_flete, costo_destino,
                          incluye_seguro, valor_declarado, porcentaje_seguro, subtotal, monto_seguro, tarifa_total,
                          moneda, vigencia_dias, forma_pago, estado, fecha_envio, correo_envio, hecho_por, fecha_creacion)
VALUES
 ('COT-2025-021', DATEADD(MONTH,-11,@hoy), @cJimenez,'Familia Jiménez Rojas',  NULL,                    NULL,           'jimenezrojas@gmail.com','8812-4455','Mudanza Local',  'Curridabat, San José','Grecia, Alajuela',      18.50,NULL,        NULL,          NULL,                 2, 1,0, 0,  350.00,   0.00,  420.00, 180.00, 0,      0.00,0.00,  950.00,   0.00,   950.00,'USD',15,'50% adelanto, 50% contra entrega','Convertida',DATEADD(MONTH,-11,@hoy),'jimenezrojas@gmail.com','C. Mora',  DATEADD(MONTH,-11,@hoy)),
 ('COT-2025-024', DATEADD(MONTH,-10,@hoy), @cIntel,  'Intel Costa Rica',       'Intel Costa Rica',      'Laura Quesada','lquesada@intel-cr.com', '8890-1200','Puerta a Puerta','Zona Franca Belén',   'Miami, FL, EE.UU.',     62.00,'40 pies HC','Maersk Line', 'Atlas Movers Miami', 4,12,5,  7, 1850.00, 980.00, 3400.00,1120.00, 1, 85000.00,2.00, 7350.00,1700.00,  9050.00,'USD',30,'Transferencia a 30 días',         'Convertida',DATEADD(MONTH,-10,@hoy),'lquesada@intel-cr.com','A. Solís', DATEADD(MONTH,-10,@hoy)),
 ('COT-2025-029', DATEADD(MONTH,-9, @hoy), @cSolano, 'Roberto Solano Mora',    NULL,                    NULL,           'rsolano@gmail.com',     '8455-3321','Puerta a Puerta','Escazú, San José',    'Madrid, España',        34.00,'20 pies',   'MSC',         'Iberia Relocation',  3,22,6, 14, 1200.00, 640.00, 2750.00, 890.00, 1, 42000.00,2.00, 5480.00, 840.00,  6320.00,'USD',30,'50% adelanto, 50% al zarpar',     'Convertida',DATEADD(MONTH,-9, @hoy),'rsolano@gmail.com',   'A. Solís', DATEADD(MONTH,-9, @hoy)),
 ('COT-2025-035', DATEADD(MONTH,-8, @hoy), @cEmbCan, 'Embajada de Canadá',     'Embajada de Canadá',    'Pierre Gagnon','consular@canada-cr.org','8801-7788','Puerta a Puerta','Sabana Sur, San José','Ottawa, Canadá',        41.00,'40 pies',   'Hapag-Lloyd', 'Canada Moving',      3,18,4, 10, 1600.00,   0.00, 3100.00,1050.00, 1, 60000.00,1.50, 5750.00, 900.00,  6650.00,'USD',45,'Contra factura consular',         'Convertida',DATEADD(MONTH,-8, @hoy),'consular@canada-cr.org','C. Mora', DATEADD(MONTH,-8, @hoy)),
 ('COT-2025-042', DATEADD(MONTH,-7, @hoy), @cAnaL,   'Ana Lucía Fernández',    NULL,                    NULL,           'alfernandez@hotmail.com','8377-2094','Solo Empaque',  'Alajuela centro',     'San Ramón, Alajuela',    9.00,NULL,        NULL,          NULL,                 1, 0,0,  0,  280.00,   0.00,  310.00, 140.00, 0,      0.00,0.00,  730.00,   0.00,   730.00,'USD',15,'Contado',                         'Convertida',DATEADD(MONTH,-7, @hoy),'alfernandez@hotmail.com','J. Pérez',DATEADD(MONTH,-7, @hoy)),
 ('COT-2025-048', DATEADD(MONTH,-6, @hoy), @cNacion, 'Grupo Nación',           'Grupo Nación GN S.A.',  'Diego Ramírez','dramirez@nacion.com',   '8912-0043','Mudanza Local',  'Llorente de Tibás',   'Barreal de Heredia',    88.00,NULL,        NULL,          NULL,                 6, 3,0,  0,  900.00,   0.00, 1150.00, 480.00, 1, 35000.00,1.50, 2530.00, 525.00,  3055.00,'CRC',30,'Crédito 30 días',                 'Convertida',DATEADD(MONTH,-6, @hoy),'dramirez@nacion.com', 'A. Solís', DATEADD(MONTH,-6, @hoy)),
 ('COT-2026-003', DATEADD(MONTH,-5, @hoy), @cCastro, 'Familia Castro Umaña',   NULL,                    NULL,           'castroumana@gmail.com', '8654-1122','Mudanza Local',  'Tres Ríos, Cartago',  'Pérez Zeledón',         26.00,NULL,        NULL,          NULL,                 2, 1,0,  0,  520.00,   0.00,  640.00, 260.00, 0,      0.00,0.00, 1420.00,   0.00,  1420.00,'USD',15,'50% adelanto',                    'Convertida',DATEADD(MONTH,-5, @hoy),'castroumana@gmail.com','C. Mora', DATEADD(MONTH,-5, @hoy)),
 ('COT-2026-011', DATEADD(MONTH,-3, @hoy), @cCima,   'Hospital CIMA',          'Hospital CIMA San José','Karla Induni', 'kinduni@cimacr.com',    '8700-5566','Mudanza Local',  'Escazú, San José',    'Liberia, Guanacaste',   54.00,NULL,        NULL,          NULL,                 3, 2,0,  0, 1400.00,   0.00, 1900.00, 700.00, 1,120000.00,2.00, 4000.00,2400.00,  6400.00,'USD',30,'Transferencia a 30 días',         'Convertida',DATEADD(MONTH,-3, @hoy),'kinduni@cimacr.com',  'J. Pérez', DATEADD(MONTH,-3, @hoy)),
 ('COT-2026-017', DATEADD(MONTH,-2, @hoy), @cIntel,  'Intel Costa Rica',       'Intel Costa Rica',      'Laura Quesada','lquesada@intel-cr.com', '8890-1200','Puerta a Puerto','Zona Franca Belén',   'Guadalajara, México',   47.00,'40 pies',   'Copa Cargo',  'Movers MX',          3, 6,3,  4, 1750.00, 820.00, 4200.00, 980.00, 1, 95000.00,2.00, 7750.00,1900.00,  9650.00,'USD',30,'Transferencia a 30 días',         'Convertida',DATEADD(MONTH,-2, @hoy),'lquesada@intel-cr.com','A. Solís',DATEADD(MONTH,-2, @hoy)),
 ('COT-2026-023', DATEADD(MONTH,-1, @hoy), @cMontero,'Javier Montero Alfaro',  NULL,                    NULL,           'jmontero@gmail.com',    '8221-6699','Mudanza Local',  'Santa Ana, San José', 'Tamarindo, Guanacaste', 21.00,NULL,        NULL,          NULL,                 2, 1,0,  0,  430.00,   0.00,  580.00, 210.00, 0,      0.00,0.00, 1220.00,   0.00,  1220.00,'USD',15,'Contado',                         'Aceptada',  DATEADD(MONTH,-1, @hoy),'jmontero@gmail.com',  'C. Mora',  DATEADD(MONTH,-1, @hoy)),
 ('COT-2026-028', DATEADD(DAY, -14,@hoy),  @cVargas, 'Marisol Vargas Céspedes',NULL,                    NULL,           'mvargas@outlook.com',   '8730-9014','Mudanza Local',  'Santo Domingo, Heredia','San Pablo, Heredia',   11.00,NULL,        NULL,        NULL,                 1, 1,0,  0,  190.00,   0.00,  240.00,  90.00, 0,      0.00,0.00,  520.00,   0.00,   520.00,'USD',15,'Contado',                         'Aceptada',  DATEADD(DAY, -13,@hoy),'mvargas@outlook.com', 'J. Pérez', DATEADD(DAY, -14,@hoy)),
 ('COT-2026-031', DATEADD(DAY, -10,@hoy),  @cSolano, 'Roberto Solano Mora',    NULL,                    NULL,           'rsolano@gmail.com',     '8455-3321','Puerto a Puerta','Madrid, España',      'Escazú, San José',      36.00,'20 pies',   'MSC',         'Iberia Relocation',  0,24,7, 14, 1100.00, 700.00, 2600.00, 850.00, 1, 45000.00,2.00, 5250.00, 900.00,  6150.00,'USD',30,'50% adelanto, 50% a la llegada',  'Enviada',   DATEADD(DAY,  -9,@hoy),'rsolano@gmail.com',   'A. Solís', DATEADD(DAY, -10,@hoy)),
 ('COT-2026-033', DATEADD(DAY,  -7,@hoy),  @cNacion, 'Grupo Nación',           'Grupo Nación GN S.A.',  'Diego Ramírez','dramirez@nacion.com',   '8912-0043','Mudanza Local',  'Llorente de Tibás',   'Barreal de Heredia',    32.00,NULL,        NULL,          NULL,                 2, 1,0,  0,  380.00,   0.00,  490.00, 200.00, 0,      0.00,0.00, 1070.00,   0.00,  1070.00,'CRC',30,'Crédito 30 días',                 'Enviada',   DATEADD(DAY,  -6,@hoy),'dramirez@nacion.com', 'A. Solís', DATEADD(DAY,  -7,@hoy)),
 ('COT-2026-035', DATEADD(DAY,  -5,@hoy),  @cCima,   'Hospital CIMA',          'Hospital CIMA San José','Karla Induni', 'kinduni@cimacr.com',    '8700-5566','Puerta a Puerta','Escazú, San José',    'Bogotá, Colombia',      29.00,'20 pies',   'Hapag-Lloyd', 'Andina Movers',      2,14,5,  7, 1250.00, 690.00, 2400.00, 810.00, 1, 78000.00,2.00, 5150.00,1560.00,  6710.00,'USD',30,'Transferencia a 30 días',         'Rechazada', DATEADD(DAY,  -4,@hoy),'kinduni@cimacr.com',  'J. Pérez', DATEADD(DAY,  -5,@hoy)),
 ('COT-2026-037', DATEADD(DAY,  -3,@hoy),  @cAnaL,   'Ana Lucía Fernández',    NULL,                    NULL,           'alfernandez@hotmail.com','8377-2094','Solo Empaque',  'San Ramón, Alajuela', 'San Ramón, Alajuela',    7.00,NULL,        NULL,          NULL,                 1, 0,0,  0,  240.00,   0.00,  260.00, 120.00, 0,      0.00,0.00,  620.00,   0.00,   620.00,'USD',15,'Contado',                         'Borrador',  NULL,                  NULL,                  'C. Mora',  DATEADD(DAY,  -3,@hoy)),
 ('COT-2026-039', DATEADD(DAY,  -1,@hoy),  @cIntel,  'Intel Costa Rica',       'Intel Costa Rica',      'Laura Quesada','lquesada@intel-cr.com', '8890-1200','Puerta a Puerto','Zona Franca Belén',   'Penang, Malasia',       58.00,'40 pies HC','Maersk Line', 'Asia Pacific Movers',4,34,8, 21, 2100.00,1150.00, 5600.00,1340.00, 1,140000.00,2.00,10190.00,2800.00, 12990.00,'USD',30,'Transferencia a 30 días',         'Borrador',  NULL,                  NULL,                  'A. Solís', DATEADD(DAY,  -1,@hoy));

-- ---------------------------------------------------------------------
-- 11. Materiales asignados a órdenes (descuenta del inventario)
-- ---------------------------------------------------------------------
DECLARE @mCajaP INT = (SELECT id_material FROM Inventario WHERE nombre_material = 'Caja pequeña 40x30x30');
DECLARE @mCajaM INT = (SELECT id_material FROM Inventario WHERE nombre_material = 'Caja mediana 50x40x40');
DECLARE @mCajaG INT = (SELECT id_material FROM Inventario WHERE nombre_material = 'Caja grande 60x50x50');
DECLARE @mBurbu INT = (SELECT id_material FROM Inventario WHERE nombre_material = 'Plástico burbuja');
DECLARE @mManta INT = (SELECT id_material FROM Inventario WHERE nombre_material = 'Manta de mudanza');
DECLARE @mCinta INT = (SELECT id_material FROM Inventario WHERE nombre_material = 'Cinta adhesiva 48mm');

DECLARE @oNacion3 INT = (SELECT id_orden FROM Ordenes_Trabajo WHERE numero_ot = 'OT-2026-0033');
DECLARE @oJimenez2 INT = (SELECT id_orden FROM Ordenes_Trabajo WHERE numero_ot = 'OT-2026-0034');
DECLARE @oCima2   INT = (SELECT id_orden FROM Ordenes_Trabajo WHERE numero_ot = 'OT-2026-0035');

INSERT INTO OrdenTrabajoMaterial (id_orden, id_material, cantidad, fecha_asignacion) VALUES
 (@oNacion3,  @mCajaM, 45, DATEADD(DAY,-11,@hoy)),
 (@oNacion3,  @mCajaG, 20, DATEADD(DAY,-11,@hoy)),
 (@oNacion3,  @mCinta, 12, DATEADD(DAY,-11,@hoy)),
 (@oJimenez2, @mCajaP, 25, DATEADD(DAY, -5,@hoy)),
 (@oJimenez2, @mBurbu,  3, DATEADD(DAY, -5,@hoy)),
 (@oJimenez2, @mManta,  8, DATEADD(DAY, -5,@hoy)),
 (@oCima2,    @mCajaG, 15, DATEADD(DAY, -1,@hoy)),
 (@oCima2,    @mManta, 14, DATEADD(DAY, -1,@hoy));

-- Reflejar el consumo en las existencias.
UPDATE i
SET i.existencias = i.existencias - x.usado,
    i.fecha_actualizacion = GETDATE()
FROM Inventario i
JOIN (SELECT id_material, SUM(cantidad) AS usado
      FROM OrdenTrabajoMaterial GROUP BY id_material) x
  ON x.id_material = i.id_material;

-- ---------------------------------------------------------------------
-- 12. Bienes por mudanza (HU-INV-003)
-- ---------------------------------------------------------------------
INSERT INTO BienesMudanza (id_orden, nombre_bien, descripcion, cantidad, condicion, observaciones, fecha_registro) VALUES
 (@oJimenez2,'Refrigeradora',      'Samsung 2 puertas, gris',        1,'Bueno',  'Rayón leve en la puerta derecha.',   DATEADD(DAY,-5,@hoy)),
 (@oJimenez2,'Juego de comedor',   'Mesa de madera y 6 sillas',      1,'Bueno',  NULL,                                  DATEADD(DAY,-5,@hoy)),
 (@oJimenez2,'Sofá 3 puestos',     'Tela gris',                      1,'Regular','Descosido en el brazo izquierdo.',    DATEADD(DAY,-5,@hoy)),
 (@oJimenez2,'Televisor 55"',      'LG OLED, con base',              1,'Bueno',  'Se traslada en caja original.',       DATEADD(DAY,-5,@hoy)),
 (@oJimenez2,'Cama matrimonial',   'Base, colchón y respaldar',      1,'Bueno',  NULL,                                  DATEADD(DAY,-5,@hoy)),
 (@oJimenez2,'Lavadora',           'Whirlpool carga superior',       1,'Regular','Golpe en la tapa, funciona bien.',    DATEADD(DAY,-5,@hoy)),
 (@oJimenez2,'Cajas de libros',    'Rotuladas 1 a 12',              12,'Bueno',  NULL,                                  DATEADD(DAY,-5,@hoy)),
 (@oNacion3, 'Escritorio ejecutivo','Madera, con gavetas',           8,'Bueno',  NULL,                                  DATEADD(DAY,-11,@hoy)),
 (@oNacion3, 'Silla ergonómica',   'Malla negra, con apoyabrazos',  24,'Bueno',  NULL,                                  DATEADD(DAY,-11,@hoy)),
 (@oNacion3, 'Archivador metálico','4 gavetas, con llave',           6,'Regular','Dos con la cerradura forzada.',       DATEADD(DAY,-11,@hoy)),
 (@oNacion3, 'Monitor 27"',        'Dell, sin base',                18,'Bueno',  'Empacados en caja original.',         DATEADD(DAY,-11,@hoy)),
 (@oNacion3, 'Impresora multifuncional','Xerox WorkCentre',          2,'Bueno',  'Requiere dos personas.',              DATEADD(DAY,-11,@hoy)),
 (@oCima2,   'Camilla hidráulica', 'Acero inoxidable',               4,'Bueno',  'Manejo especial, no apilar.',         DATEADD(DAY,-1,@hoy)),
 (@oCima2,   'Vitrina de instrumental','Vidrio templado',            3,'Dañado', 'Vidrio lateral trizado, se documenta.',DATEADD(DAY,-1,@hoy)),
 (@oCima2,   'Carro de curaciones','Con ruedas y gavetas',           5,'Bueno',  NULL,                                  DATEADD(DAY,-1,@hoy));

-- ---------------------------------------------------------------------
-- 13. Listas de embalaje (HU-INV-002)
-- ---------------------------------------------------------------------
INSERT INTO ListasEmbalaje (id_orden, numero_lista, responsable, observaciones, estado, fecha_generacion, fecha_actualizacion) VALUES
 (@oJimenez2,'LE-OT-2026-0034-001','Carlos Mora',   'Carga en camión 2. Electrodomésticos al fondo.', 'Borrador',   DATEADD(DAY, -4,@hoy), DATEADD(DAY,-3,@hoy)),
 (@oNacion3, 'LE-OT-2026-0033-002','Ana Solís',     'Etapa 3. Rotulado por piso de destino.',          'Finalizada', DATEADD(DAY,-10,@hoy), DATEADD(DAY,-8,@hoy));

DECLARE @lJimenez INT = (SELECT id_lista FROM ListasEmbalaje WHERE id_orden = @oJimenez2);
DECLARE @lNacion  INT = (SELECT id_lista FROM ListasEmbalaje WHERE id_orden = @oNacion3);

-- La lista de la familia Jiménez lleva todos sus bienes.
INSERT INTO ListasEmbalajeDetalle (id_lista, id_bien, cantidad)
SELECT @lJimenez, b.id_bien, b.cantidad
FROM BienesMudanza b WHERE b.id_orden = @oJimenez2;

-- La de Grupo Nación va parcial: el archivador aún no se embala.
INSERT INTO ListasEmbalajeDetalle (id_lista, id_bien, cantidad)
SELECT @lNacion, b.id_bien, b.cantidad
FROM BienesMudanza b
WHERE b.id_orden = @oNacion3 AND b.nombre_bien <> 'Archivador metálico';

-- ---------------------------------------------------------------------
-- 14. Notas e historial de órdenes
-- ---------------------------------------------------------------------
DECLARE @admin INT = (SELECT TOP 1 id_usuario FROM Usuarios ORDER BY id_usuario);

INSERT INTO Ordenes_Trabajo_Notas (id_orden, contenido, id_usuario, fecha_creacion) VALUES
 (@oNacion3, 'El cliente solicita iniciar a las 6:30 a.m. para no interferir con la operación.', @admin, DATEADD(DAY,-11,@hoy)),
 (@oNacion3, 'Confirmado el acceso al parqueo de carga por la entrada sur.',                     @admin, DATEADD(DAY, -9,@hoy)),
 (@oJimenez2,'La señora Jiménez pidió empacar la vajilla aparte y rotularla como frágil.',       @admin, DATEADD(DAY, -5,@hoy)),
 (@oCima2,   'Coordinar con mantenimiento del hospital para el uso del ascensor de carga.',      @admin, DATEADD(DAY, -2,@hoy));

INSERT INTO Ordenes_Trabajo_Historial (id_orden, campo_modificado, valor_anterior, valor_nuevo, id_usuario, fecha_cambio) VALUES
 (@oNacion3, 'Estado',        'Pendiente', 'Pendiente',              @admin, DATEADD(DAY,-11,@hoy)),
 (@oNacion3, 'Hora',          '08:00',     '07:30',                  @admin, DATEADD(DAY, -9,@hoy)),
 (@oJimenez2,'DireccionDestino','Grecia, Alajuela','Atenas, Alajuela',@admin, DATEADD(DAY, -4,@hoy)),
 (@oCima2,   'FechaServicio', NULL,        CONVERT(NVARCHAR(30), DATEADD(DAY,3,@hoy), 103), @admin, DATEADD(DAY,-1,@hoy));

COMMIT TRANSACTION;

-- ---------------------------------------------------------------------
-- Resumen
-- ---------------------------------------------------------------------
SELECT 'Clientes' AS tabla, COUNT(*) AS filas FROM Clientes
UNION ALL SELECT 'Ordenes_Trabajo',          COUNT(*) FROM Ordenes_Trabajo
UNION ALL SELECT 'Control_Visitas',          COUNT(*) FROM Control_Visitas
UNION ALL SELECT 'Cotizaciones',             COUNT(*) FROM Cotizaciones
UNION ALL SELECT 'Importaciones',            COUNT(*) FROM Importaciones
UNION ALL SELECT 'Exportaciones',            COUNT(*) FROM Exportaciones
UNION ALL SELECT 'Catalogo_Documentos',      COUNT(*) FROM Catalogo_Documentos
UNION ALL SELECT 'Importaciones_Documentos', COUNT(*) FROM Importaciones_Documentos
UNION ALL SELECT 'Exportaciones_Documentos', COUNT(*) FROM Exportaciones_Documentos
UNION ALL SELECT 'Inventario',               COUNT(*) FROM Inventario
UNION ALL SELECT 'OrdenTrabajoMaterial',     COUNT(*) FROM OrdenTrabajoMaterial
UNION ALL SELECT 'BienesMudanza',            COUNT(*) FROM BienesMudanza
UNION ALL SELECT 'ListasEmbalaje',           COUNT(*) FROM ListasEmbalaje
UNION ALL SELECT 'ListasEmbalajeDetalle',    COUNT(*) FROM ListasEmbalajeDetalle
UNION ALL SELECT 'Ordenes_Trabajo_Notas',    COUNT(*) FROM Ordenes_Trabajo_Notas
UNION ALL SELECT 'Ordenes_Trabajo_Historial',COUNT(*) FROM Ordenes_Trabajo_Historial;
