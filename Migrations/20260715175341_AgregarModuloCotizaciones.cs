using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinMovers.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModuloCotizaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExportacionesDocumentos_Exportaciones_IdExportacion",
                table: "ExportacionesDocumentos");

            migrationBuilder.DropForeignKey(
                name: "FK_ImportacionesDocumentos_Importaciones_IdImportacion",
                table: "ImportacionesDocumentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrdenesTrabajo",
                table: "OrdenesTrabajo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ImportacionesDocumentos",
                table: "ImportacionesDocumentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ExportacionesDocumentos",
                table: "ExportacionesDocumentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ControlVisitas",
                table: "ControlVisitas");

            migrationBuilder.DropColumn(
                name: "NombreDocumento",
                table: "ImportacionesDocumentos");

            migrationBuilder.DropColumn(
                name: "TipoAgente",
                table: "ImportacionesDocumentos");

            migrationBuilder.DropColumn(
                name: "NombreDocumento",
                table: "ExportacionesDocumentos");

            migrationBuilder.DropColumn(
                name: "TipoAgente",
                table: "ExportacionesDocumentos");

            migrationBuilder.RenameTable(
                name: "OrdenesTrabajo",
                newName: "Ordenes_Trabajo");

            migrationBuilder.RenameTable(
                name: "ImportacionesDocumentos",
                newName: "Importaciones_Documentos");

            migrationBuilder.RenameTable(
                name: "ExportacionesDocumentos",
                newName: "Exportaciones_Documentos");

            migrationBuilder.RenameTable(
                name: "ControlVisitas",
                newName: "Control_Visitas");

            migrationBuilder.RenameColumn(
                name: "Referencia",
                table: "Importaciones",
                newName: "referencia");

            migrationBuilder.RenameColumn(
                name: "Observaciones",
                table: "Importaciones",
                newName: "observaciones");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Importaciones",
                newName: "fecha");

            migrationBuilder.RenameColumn(
                name: "NombreCliente",
                table: "Importaciones",
                newName: "nombre_cliente");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Importaciones",
                newName: "fecha_creacion");

            migrationBuilder.RenameColumn(
                name: "IdImportacion",
                table: "Importaciones",
                newName: "id_importacion");

            migrationBuilder.RenameColumn(
                name: "Referencia",
                table: "Exportaciones",
                newName: "referencia");

            migrationBuilder.RenameColumn(
                name: "Observaciones",
                table: "Exportaciones",
                newName: "observaciones");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Exportaciones",
                newName: "fecha");

            migrationBuilder.RenameColumn(
                name: "NombreCliente",
                table: "Exportaciones",
                newName: "nombre_cliente");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Exportaciones",
                newName: "fecha_creacion");

            migrationBuilder.RenameColumn(
                name: "IdExportacion",
                table: "Exportaciones",
                newName: "id_exportacion");

            migrationBuilder.RenameColumn(
                name: "Materiales",
                table: "Ordenes_Trabajo",
                newName: "materiales");

            migrationBuilder.RenameColumn(
                name: "Hora",
                table: "Ordenes_Trabajo",
                newName: "hora");

            migrationBuilder.RenameColumn(
                name: "Fecha",
                table: "Ordenes_Trabajo",
                newName: "fecha");

            migrationBuilder.RenameColumn(
                name: "Contacto",
                table: "Ordenes_Trabajo",
                newName: "contacto");

            migrationBuilder.RenameColumn(
                name: "Compania",
                table: "Ordenes_Trabajo",
                newName: "compania");

            migrationBuilder.RenameColumn(
                name: "TelefonoResidencia",
                table: "Ordenes_Trabajo",
                newName: "telefono_residencia");

            migrationBuilder.RenameColumn(
                name: "TelefonoEmpresa",
                table: "Ordenes_Trabajo",
                newName: "telefono_empresa");

            migrationBuilder.RenameColumn(
                name: "TelefonoCelular",
                table: "Ordenes_Trabajo",
                newName: "telefono_celular");

            migrationBuilder.RenameColumn(
                name: "NumeroOT",
                table: "Ordenes_Trabajo",
                newName: "numero_ot");

            migrationBuilder.RenameColumn(
                name: "NombreCliente",
                table: "Ordenes_Trabajo",
                newName: "nombre_cliente");

            migrationBuilder.RenameColumn(
                name: "HechoPor",
                table: "Ordenes_Trabajo",
                newName: "hecho_por");

            migrationBuilder.RenameColumn(
                name: "FechaServicio",
                table: "Ordenes_Trabajo",
                newName: "fecha_servicio");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Ordenes_Trabajo",
                newName: "fecha_creacion");

            migrationBuilder.RenameColumn(
                name: "FechaActualizacion",
                table: "Ordenes_Trabajo",
                newName: "fecha_actualizacion");

            migrationBuilder.RenameColumn(
                name: "FacturarA",
                table: "Ordenes_Trabajo",
                newName: "facturar_a");

            migrationBuilder.RenameColumn(
                name: "DireccionOrigen",
                table: "Ordenes_Trabajo",
                newName: "direccion_origen");

            migrationBuilder.RenameColumn(
                name: "DireccionDestino",
                table: "Ordenes_Trabajo",
                newName: "direccion_destino");

            migrationBuilder.RenameColumn(
                name: "DireccionCobro",
                table: "Ordenes_Trabajo",
                newName: "direccion_cobro");

            migrationBuilder.RenameColumn(
                name: "DetalleServicio",
                table: "Ordenes_Trabajo",
                newName: "detalle_servicio");

            migrationBuilder.RenameColumn(
                name: "IdOrden",
                table: "Ordenes_Trabajo",
                newName: "id_orden");

            migrationBuilder.RenameColumn(
                name: "Completado",
                table: "Importaciones_Documentos",
                newName: "completado");

            migrationBuilder.RenameColumn(
                name: "IdImportacion",
                table: "Importaciones_Documentos",
                newName: "id_importacion");

            migrationBuilder.RenameColumn(
                name: "IdDocumento",
                table: "Importaciones_Documentos",
                newName: "id_imp_doc");

            migrationBuilder.RenameIndex(
                name: "IX_ImportacionesDocumentos_IdImportacion",
                table: "Importaciones_Documentos",
                newName: "IX_Importaciones_Documentos_id_importacion");

            migrationBuilder.RenameColumn(
                name: "Completado",
                table: "Exportaciones_Documentos",
                newName: "completado");

            migrationBuilder.RenameColumn(
                name: "IdExportacion",
                table: "Exportaciones_Documentos",
                newName: "id_exportacion");

            migrationBuilder.RenameColumn(
                name: "IdDocumento",
                table: "Exportaciones_Documentos",
                newName: "id_exp_doc");

            migrationBuilder.RenameIndex(
                name: "IX_ExportacionesDocumentos_IdExportacion",
                table: "Exportaciones_Documentos",
                newName: "IX_Exportaciones_Documentos_id_exportacion");

            migrationBuilder.RenameColumn(
                name: "Origen",
                table: "Control_Visitas",
                newName: "origen");

            migrationBuilder.RenameColumn(
                name: "Observaciones",
                table: "Control_Visitas",
                newName: "observaciones");

            migrationBuilder.RenameColumn(
                name: "Hora",
                table: "Control_Visitas",
                newName: "hora");

            migrationBuilder.RenameColumn(
                name: "Flete",
                table: "Control_Visitas",
                newName: "flete");

            migrationBuilder.RenameColumn(
                name: "Empresa",
                table: "Control_Visitas",
                newName: "empresa");

            migrationBuilder.RenameColumn(
                name: "Empaque",
                table: "Control_Visitas",
                newName: "empaque");

            migrationBuilder.RenameColumn(
                name: "Destino",
                table: "Control_Visitas",
                newName: "destino");

            migrationBuilder.RenameColumn(
                name: "Corresponsal",
                table: "Control_Visitas",
                newName: "corresponsal");

            migrationBuilder.RenameColumn(
                name: "TramitesAduana",
                table: "Control_Visitas",
                newName: "tramites_aduana");

            migrationBuilder.RenameColumn(
                name: "TelefonoHabitacion",
                table: "Control_Visitas",
                newName: "telefono_habitacion");

            migrationBuilder.RenameColumn(
                name: "TelefonoCompania",
                table: "Control_Visitas",
                newName: "telefono_compania");

            migrationBuilder.RenameColumn(
                name: "TelefonoCelular",
                table: "Control_Visitas",
                newName: "telefono_celular");

            migrationBuilder.RenameColumn(
                name: "TarifaTotal",
                table: "Control_Visitas",
                newName: "tarifa_total");

            migrationBuilder.RenameColumn(
                name: "PuertaAPuerto",
                table: "Control_Visitas",
                newName: "puerta_a_puerto");

            migrationBuilder.RenameColumn(
                name: "PuertaAPuerta",
                table: "Control_Visitas",
                newName: "puerta_a_puerta");

            migrationBuilder.RenameColumn(
                name: "NombreCliente",
                table: "Control_Visitas",
                newName: "nombre_cliente");

            migrationBuilder.RenameColumn(
                name: "MudanzaLocal",
                table: "Control_Visitas",
                newName: "mudanza_local");

            migrationBuilder.RenameColumn(
                name: "HechoPor",
                table: "Control_Visitas",
                newName: "hecho_por");

            migrationBuilder.RenameColumn(
                name: "FechaVisita",
                table: "Control_Visitas",
                newName: "fecha_visita");

            migrationBuilder.RenameColumn(
                name: "FechaLlamada",
                table: "Control_Visitas",
                newName: "fecha_llamada");

            migrationBuilder.RenameColumn(
                name: "FechaCreacion",
                table: "Control_Visitas",
                newName: "fecha_creacion");

            migrationBuilder.RenameColumn(
                name: "DireccionOrigen",
                table: "Control_Visitas",
                newName: "direccion_origen");

            migrationBuilder.RenameColumn(
                name: "DireccionDestino",
                table: "Control_Visitas",
                newName: "direccion_destino");

            migrationBuilder.RenameColumn(
                name: "CompaniaMaritima",
                table: "Control_Visitas",
                newName: "compania_maritima");

            migrationBuilder.RenameColumn(
                name: "IdVisita",
                table: "Control_Visitas",
                newName: "id_visita");

            migrationBuilder.AlterColumn<string>(
                name: "referencia",
                table: "Importaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "fecha",
                table: "Importaciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "cajas",
                table: "Importaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_actualizacion",
                table: "Importaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_orden",
                table: "Importaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "kilos",
                table: "Importaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "pais",
                table: "Importaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "referencia",
                table: "Exportaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "cajas",
                table: "Exportaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_actualizacion",
                table: "Exportaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_orden",
                table: "Exportaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "kilos",
                table: "Exportaciones",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "materiales",
                table: "Ordenes_Trabajo",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "estado",
                table: "Ordenes_Trabajo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "id_cliente",
                table: "Ordenes_Trabajo",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_completado",
                table: "Importaciones_Documentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_tipo_documento",
                table: "Importaciones_Documentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "observaciones",
                table: "Importaciones_Documentos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo_checklist",
                table: "Importaciones_Documentos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_completado",
                table: "Exportaciones_Documentos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_tipo_documento",
                table: "Exportaciones_Documentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "observaciones",
                table: "Exportaciones_Documentos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo_checklist",
                table: "Exportaciones_Documentos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_actualizacion",
                table: "Control_Visitas",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ordenes_Trabajo",
                table: "Ordenes_Trabajo",
                column: "id_orden");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Importaciones_Documentos",
                table: "Importaciones_Documentos",
                column: "id_imp_doc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Exportaciones_Documentos",
                table: "Exportaciones_Documentos",
                column: "id_exp_doc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Control_Visitas",
                table: "Control_Visitas",
                column: "id_visita");

            migrationBuilder.CreateTable(
                name: "Catalogo_Documentos",
                columns: table => new
                {
                    id_tipo_documento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    aplica_exportacion = table.Column<bool>(type: "bit", nullable: false),
                    aplica_importacion = table.Column<bool>(type: "bit", nullable: false),
                    aplica_winmovers = table.Column<bool>(type: "bit", nullable: false),
                    aplica_otro_agente = table.Column<bool>(type: "bit", nullable: false),
                    orden_presentacion = table.Column<int>(type: "int", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogo_Documentos", x => x.id_tipo_documento);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    id_cliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_cliente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    telefono_celular = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    telefono_residencia = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    telefono_empresa = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    empresa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    contacto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    correo_electronico = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    direccion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    fecha_actualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.id_cliente);
                });

            migrationBuilder.CreateTable(
                name: "Exportaciones_Archivos",
                columns: table => new
                {
                    id_archivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_exportacion = table.Column<int>(type: "int", nullable: false),
                    nombre_original = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nombre_guardado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipo_mime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tamanio_bytes = table.Column<long>(type: "bigint", nullable: false),
                    fecha_carga = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exportaciones_Archivos", x => x.id_archivo);
                    table.ForeignKey(
                        name: "FK_Exportaciones_Archivos_Exportaciones_id_exportacion",
                        column: x => x.id_exportacion,
                        principalTable: "Exportaciones",
                        principalColumn: "id_exportacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Importaciones_Archivos",
                columns: table => new
                {
                    id_archivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_importacion = table.Column<int>(type: "int", nullable: false),
                    nombre_original = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nombre_guardado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipo_mime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tamanio_bytes = table.Column<long>(type: "bigint", nullable: false),
                    fecha_carga = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Importaciones_Archivos", x => x.id_archivo);
                    table.ForeignKey(
                        name: "FK_Importaciones_Archivos_Importaciones_id_importacion",
                        column: x => x.id_importacion,
                        principalTable: "Importaciones",
                        principalColumn: "id_importacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesTrabajo_Archivos",
                columns: table => new
                {
                    id_archivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_orden = table.Column<int>(type: "int", nullable: false),
                    nombre_original = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nombre_guardado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tipo_mime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tamanio_bytes = table.Column<long>(type: "bigint", nullable: false),
                    fecha_carga = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesTrabajo_Archivos", x => x.id_archivo);
                    table.ForeignKey(
                        name: "FK_OrdenesTrabajo_Archivos_Ordenes_Trabajo_id_orden",
                        column: x => x.id_orden,
                        principalTable: "Ordenes_Trabajo",
                        principalColumn: "id_orden",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    nombre = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    nombre_normalizado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    id_usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_completo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    debe_cambiar_contrasena = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    nombre_usuario = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    nombre_usuario_normalizado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    correo = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    correo_normalizado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    correo_confirmado = table.Column<bool>(type: "bit", nullable: false),
                    contrasena_hash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    security_stamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    telefono_confirmado = table.Column<bool>(type: "bit", nullable: false),
                    doble_factor_habilitado = table.Column<bool>(type: "bit", nullable: false),
                    bloqueo_hasta = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    bloqueo_habilitado = table.Column<bool>(type: "bit", nullable: false),
                    intentos_fallidos = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id_usuario);
                });

            migrationBuilder.CreateTable(
                name: "Roles_Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Claims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accesos_Auditoria",
                columns: table => new
                {
                    id_auditoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_usuario = table.Column<int>(type: "int", nullable: true),
                    correo_intentado = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    exitoso = table.Column<bool>(type: "bit", nullable: false),
                    motivo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ip_address = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accesos_Auditoria", x => x.id_auditoria);
                    table.ForeignKey(
                        name: "FK_Accesos_Auditoria_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Clientes_Historial",
                columns: table => new
                {
                    id_historial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_cliente = table.Column<int>(type: "int", nullable: false),
                    campo_modificado = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    valor_anterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    valor_nuevo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_usuario = table.Column<int>(type: "int", nullable: true),
                    fecha_cambio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes_Historial", x => x.id_historial);
                    table.ForeignKey(
                        name: "FK_Clientes_Historial_Clientes_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "Clientes",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Clientes_Historial_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    id_cotizacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numero_cotizacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    id_cliente = table.Column<int>(type: "int", nullable: true),
                    nombre_cliente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    compania = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    contacto = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    correo_cliente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    telefono_celular = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    tipo_servicio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    origen = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    destino = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    volumen_m3 = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    tipo_contenedor = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    compania_maritima = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    corresponsal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    dias_empaque = table.Column<int>(type: "int", nullable: true),
                    dias_transito = table.Column<int>(type: "int", nullable: true),
                    dias_desalmacenaje = table.Column<int>(type: "int", nullable: true),
                    dias_frecuencia_salidas = table.Column<int>(type: "int", nullable: true),
                    costo_origen = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    costo_tramites_aduana = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    costo_flete = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    costo_destino = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    incluye_seguro = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    valor_declarado = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    porcentaje_seguro = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    subtotal = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    monto_seguro = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    tarifa_total = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    moneda = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    vigencia_dias = table.Column<int>(type: "int", nullable: false, defaultValue: 60),
                    forma_pago = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    exclusiones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha_envio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    correo_envio = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    id_orden_generada = table.Column<int>(type: "int", nullable: true),
                    hecho_por = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    id_usuario = table.Column<int>(type: "int", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    fecha_actualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.id_cotizacion);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Clientes_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "Clientes",
                        principalColumn: "id_cliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Ordenes_Trabajo_id_orden_generada",
                        column: x => x.id_orden_generada,
                        principalTable: "Ordenes_Trabajo",
                        principalColumn: "id_orden",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Ordenes_Trabajo_Historial",
                columns: table => new
                {
                    id_historial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_orden = table.Column<int>(type: "int", nullable: false),
                    campo_modificado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    valor_anterior = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    valor_nuevo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_usuario = table.Column<int>(type: "int", nullable: true),
                    fecha_cambio = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordenes_Trabajo_Historial", x => x.id_historial);
                    table.ForeignKey(
                        name: "FK_Ordenes_Trabajo_Historial_Ordenes_Trabajo_id_orden",
                        column: x => x.id_orden,
                        principalTable: "Ordenes_Trabajo",
                        principalColumn: "id_orden",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordenes_Trabajo_Historial_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Ordenes_Trabajo_Notas",
                columns: table => new
                {
                    id_nota = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_orden = table.Column<int>(type: "int", nullable: false),
                    contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    id_usuario = table.Column<int>(type: "int", nullable: true),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    fecha_actualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordenes_Trabajo_Notas", x => x.id_nota);
                    table.ForeignKey(
                        name: "FK_Ordenes_Trabajo_Notas_Ordenes_Trabajo_id_orden",
                        column: x => x.id_orden,
                        principalTable: "Ordenes_Trabajo",
                        principalColumn: "id_orden",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ordenes_Trabajo_Notas_Usuarios_id_usuario",
                        column: x => x.id_usuario,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Roles_Auditoria",
                columns: table => new
                {
                    id_auditoria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    nombre_rol = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    id_usuario_afectado = table.Column<int>(type: "int", nullable: true),
                    id_usuario_responsable = table.Column<int>(type: "int", nullable: true),
                    detalle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles_Auditoria", x => x.id_auditoria);
                    table.ForeignKey(
                        name: "FK_Roles_Auditoria_Usuarios_id_usuario_afectado",
                        column: x => x.id_usuario_afectado,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Roles_Auditoria_Usuarios_id_usuario_responsable",
                        column: x => x.id_usuario_responsable,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios_Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Claims_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios_Logins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios_Logins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_Usuarios_Logins_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios_Roles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios_Roles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "id_rol",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios_Tokens",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios_Tokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_Usuarios_Tokens_Usuarios_UserId",
                        column: x => x.UserId,
                        principalTable: "Usuarios",
                        principalColumn: "id_usuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Importaciones_id_orden",
                table: "Importaciones",
                column: "id_orden");

            migrationBuilder.CreateIndex(
                name: "IX_Exportaciones_id_orden",
                table: "Exportaciones",
                column: "id_orden");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_Trabajo_id_cliente",
                table: "Ordenes_Trabajo",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Importaciones_Documentos_id_tipo_documento",
                table: "Importaciones_Documentos",
                column: "id_tipo_documento");

            migrationBuilder.CreateIndex(
                name: "IX_Exportaciones_Documentos_id_tipo_documento",
                table: "Exportaciones_Documentos",
                column: "id_tipo_documento");

            migrationBuilder.CreateIndex(
                name: "IX_Accesos_Auditoria_id_usuario",
                table: "Accesos_Auditoria",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Historial_id_cliente",
                table: "Clientes_Historial",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_Historial_id_usuario",
                table: "Clientes_Historial",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_id_cliente",
                table: "Cotizaciones",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_id_orden_generada",
                table: "Cotizaciones",
                column: "id_orden_generada");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_id_usuario",
                table: "Cotizaciones",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_numero_cotizacion",
                table: "Cotizaciones",
                column: "numero_cotizacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Exportaciones_Archivos_id_exportacion",
                table: "Exportaciones_Archivos",
                column: "id_exportacion");

            migrationBuilder.CreateIndex(
                name: "IX_Importaciones_Archivos_id_importacion",
                table: "Importaciones_Archivos",
                column: "id_importacion");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_Trabajo_Historial_id_orden",
                table: "Ordenes_Trabajo_Historial",
                column: "id_orden");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_Trabajo_Historial_id_usuario",
                table: "Ordenes_Trabajo_Historial",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_Trabajo_Notas_id_orden",
                table: "Ordenes_Trabajo_Notas",
                column: "id_orden");

            migrationBuilder.CreateIndex(
                name: "IX_Ordenes_Trabajo_Notas_id_usuario",
                table: "Ordenes_Trabajo_Notas",
                column: "id_usuario");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesTrabajo_Archivos_id_orden",
                table: "OrdenesTrabajo_Archivos",
                column: "id_orden");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "nombre_normalizado",
                unique: true,
                filter: "[nombre_normalizado] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Auditoria_id_usuario_afectado",
                table: "Roles_Auditoria",
                column: "id_usuario_afectado");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Auditoria_id_usuario_responsable",
                table: "Roles_Auditoria",
                column: "id_usuario_responsable");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Claims_RoleId",
                table: "Roles_Claims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Usuarios",
                column: "correo_normalizado");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Usuarios",
                column: "nombre_usuario_normalizado",
                unique: true,
                filter: "[nombre_usuario_normalizado] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Claims_UserId",
                table: "Usuarios_Claims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Logins_UserId",
                table: "Usuarios_Logins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Roles_RoleId",
                table: "Usuarios_Roles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Exportaciones_Ordenes_Trabajo_id_orden",
                table: "Exportaciones",
                column: "id_orden",
                principalTable: "Ordenes_Trabajo",
                principalColumn: "id_orden");

            migrationBuilder.AddForeignKey(
                name: "FK_Exportaciones_Documentos_Catalogo_Documentos_id_tipo_documento",
                table: "Exportaciones_Documentos",
                column: "id_tipo_documento",
                principalTable: "Catalogo_Documentos",
                principalColumn: "id_tipo_documento",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Exportaciones_Documentos_Exportaciones_id_exportacion",
                table: "Exportaciones_Documentos",
                column: "id_exportacion",
                principalTable: "Exportaciones",
                principalColumn: "id_exportacion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Importaciones_Ordenes_Trabajo_id_orden",
                table: "Importaciones",
                column: "id_orden",
                principalTable: "Ordenes_Trabajo",
                principalColumn: "id_orden");

            migrationBuilder.AddForeignKey(
                name: "FK_Importaciones_Documentos_Catalogo_Documentos_id_tipo_documento",
                table: "Importaciones_Documentos",
                column: "id_tipo_documento",
                principalTable: "Catalogo_Documentos",
                principalColumn: "id_tipo_documento",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Importaciones_Documentos_Importaciones_id_importacion",
                table: "Importaciones_Documentos",
                column: "id_importacion",
                principalTable: "Importaciones",
                principalColumn: "id_importacion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ordenes_Trabajo_Clientes_id_cliente",
                table: "Ordenes_Trabajo",
                column: "id_cliente",
                principalTable: "Clientes",
                principalColumn: "id_cliente",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Exportaciones_Ordenes_Trabajo_id_orden",
                table: "Exportaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Exportaciones_Documentos_Catalogo_Documentos_id_tipo_documento",
                table: "Exportaciones_Documentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Exportaciones_Documentos_Exportaciones_id_exportacion",
                table: "Exportaciones_Documentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Importaciones_Ordenes_Trabajo_id_orden",
                table: "Importaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Importaciones_Documentos_Catalogo_Documentos_id_tipo_documento",
                table: "Importaciones_Documentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Importaciones_Documentos_Importaciones_id_importacion",
                table: "Importaciones_Documentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Ordenes_Trabajo_Clientes_id_cliente",
                table: "Ordenes_Trabajo");

            migrationBuilder.DropTable(
                name: "Accesos_Auditoria");

            migrationBuilder.DropTable(
                name: "Catalogo_Documentos");

            migrationBuilder.DropTable(
                name: "Clientes_Historial");

            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "Exportaciones_Archivos");

            migrationBuilder.DropTable(
                name: "Importaciones_Archivos");

            migrationBuilder.DropTable(
                name: "Ordenes_Trabajo_Historial");

            migrationBuilder.DropTable(
                name: "Ordenes_Trabajo_Notas");

            migrationBuilder.DropTable(
                name: "OrdenesTrabajo_Archivos");

            migrationBuilder.DropTable(
                name: "Roles_Auditoria");

            migrationBuilder.DropTable(
                name: "Roles_Claims");

            migrationBuilder.DropTable(
                name: "Usuarios_Claims");

            migrationBuilder.DropTable(
                name: "Usuarios_Logins");

            migrationBuilder.DropTable(
                name: "Usuarios_Roles");

            migrationBuilder.DropTable(
                name: "Usuarios_Tokens");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Importaciones_id_orden",
                table: "Importaciones");

            migrationBuilder.DropIndex(
                name: "IX_Exportaciones_id_orden",
                table: "Exportaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ordenes_Trabajo",
                table: "Ordenes_Trabajo");

            migrationBuilder.DropIndex(
                name: "IX_Ordenes_Trabajo_id_cliente",
                table: "Ordenes_Trabajo");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Importaciones_Documentos",
                table: "Importaciones_Documentos");

            migrationBuilder.DropIndex(
                name: "IX_Importaciones_Documentos_id_tipo_documento",
                table: "Importaciones_Documentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Exportaciones_Documentos",
                table: "Exportaciones_Documentos");

            migrationBuilder.DropIndex(
                name: "IX_Exportaciones_Documentos_id_tipo_documento",
                table: "Exportaciones_Documentos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Control_Visitas",
                table: "Control_Visitas");

            migrationBuilder.DropColumn(
                name: "cajas",
                table: "Importaciones");

            migrationBuilder.DropColumn(
                name: "fecha_actualizacion",
                table: "Importaciones");

            migrationBuilder.DropColumn(
                name: "id_orden",
                table: "Importaciones");

            migrationBuilder.DropColumn(
                name: "kilos",
                table: "Importaciones");

            migrationBuilder.DropColumn(
                name: "pais",
                table: "Importaciones");

            migrationBuilder.DropColumn(
                name: "cajas",
                table: "Exportaciones");

            migrationBuilder.DropColumn(
                name: "fecha_actualizacion",
                table: "Exportaciones");

            migrationBuilder.DropColumn(
                name: "id_orden",
                table: "Exportaciones");

            migrationBuilder.DropColumn(
                name: "kilos",
                table: "Exportaciones");

            migrationBuilder.DropColumn(
                name: "estado",
                table: "Ordenes_Trabajo");

            migrationBuilder.DropColumn(
                name: "id_cliente",
                table: "Ordenes_Trabajo");

            migrationBuilder.DropColumn(
                name: "fecha_completado",
                table: "Importaciones_Documentos");

            migrationBuilder.DropColumn(
                name: "id_tipo_documento",
                table: "Importaciones_Documentos");

            migrationBuilder.DropColumn(
                name: "observaciones",
                table: "Importaciones_Documentos");

            migrationBuilder.DropColumn(
                name: "tipo_checklist",
                table: "Importaciones_Documentos");

            migrationBuilder.DropColumn(
                name: "fecha_completado",
                table: "Exportaciones_Documentos");

            migrationBuilder.DropColumn(
                name: "id_tipo_documento",
                table: "Exportaciones_Documentos");

            migrationBuilder.DropColumn(
                name: "observaciones",
                table: "Exportaciones_Documentos");

            migrationBuilder.DropColumn(
                name: "tipo_checklist",
                table: "Exportaciones_Documentos");

            migrationBuilder.DropColumn(
                name: "fecha_actualizacion",
                table: "Control_Visitas");

            migrationBuilder.RenameTable(
                name: "Ordenes_Trabajo",
                newName: "OrdenesTrabajo");

            migrationBuilder.RenameTable(
                name: "Importaciones_Documentos",
                newName: "ImportacionesDocumentos");

            migrationBuilder.RenameTable(
                name: "Exportaciones_Documentos",
                newName: "ExportacionesDocumentos");

            migrationBuilder.RenameTable(
                name: "Control_Visitas",
                newName: "ControlVisitas");

            migrationBuilder.RenameColumn(
                name: "referencia",
                table: "Importaciones",
                newName: "Referencia");

            migrationBuilder.RenameColumn(
                name: "observaciones",
                table: "Importaciones",
                newName: "Observaciones");

            migrationBuilder.RenameColumn(
                name: "fecha",
                table: "Importaciones",
                newName: "Fecha");

            migrationBuilder.RenameColumn(
                name: "nombre_cliente",
                table: "Importaciones",
                newName: "NombreCliente");

            migrationBuilder.RenameColumn(
                name: "fecha_creacion",
                table: "Importaciones",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "id_importacion",
                table: "Importaciones",
                newName: "IdImportacion");

            migrationBuilder.RenameColumn(
                name: "referencia",
                table: "Exportaciones",
                newName: "Referencia");

            migrationBuilder.RenameColumn(
                name: "observaciones",
                table: "Exportaciones",
                newName: "Observaciones");

            migrationBuilder.RenameColumn(
                name: "fecha",
                table: "Exportaciones",
                newName: "Fecha");

            migrationBuilder.RenameColumn(
                name: "nombre_cliente",
                table: "Exportaciones",
                newName: "NombreCliente");

            migrationBuilder.RenameColumn(
                name: "fecha_creacion",
                table: "Exportaciones",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "id_exportacion",
                table: "Exportaciones",
                newName: "IdExportacion");

            migrationBuilder.RenameColumn(
                name: "materiales",
                table: "OrdenesTrabajo",
                newName: "Materiales");

            migrationBuilder.RenameColumn(
                name: "hora",
                table: "OrdenesTrabajo",
                newName: "Hora");

            migrationBuilder.RenameColumn(
                name: "fecha",
                table: "OrdenesTrabajo",
                newName: "Fecha");

            migrationBuilder.RenameColumn(
                name: "contacto",
                table: "OrdenesTrabajo",
                newName: "Contacto");

            migrationBuilder.RenameColumn(
                name: "compania",
                table: "OrdenesTrabajo",
                newName: "Compania");

            migrationBuilder.RenameColumn(
                name: "telefono_residencia",
                table: "OrdenesTrabajo",
                newName: "TelefonoResidencia");

            migrationBuilder.RenameColumn(
                name: "telefono_empresa",
                table: "OrdenesTrabajo",
                newName: "TelefonoEmpresa");

            migrationBuilder.RenameColumn(
                name: "telefono_celular",
                table: "OrdenesTrabajo",
                newName: "TelefonoCelular");

            migrationBuilder.RenameColumn(
                name: "numero_ot",
                table: "OrdenesTrabajo",
                newName: "NumeroOT");

            migrationBuilder.RenameColumn(
                name: "nombre_cliente",
                table: "OrdenesTrabajo",
                newName: "NombreCliente");

            migrationBuilder.RenameColumn(
                name: "hecho_por",
                table: "OrdenesTrabajo",
                newName: "HechoPor");

            migrationBuilder.RenameColumn(
                name: "fecha_servicio",
                table: "OrdenesTrabajo",
                newName: "FechaServicio");

            migrationBuilder.RenameColumn(
                name: "fecha_creacion",
                table: "OrdenesTrabajo",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "fecha_actualizacion",
                table: "OrdenesTrabajo",
                newName: "FechaActualizacion");

            migrationBuilder.RenameColumn(
                name: "facturar_a",
                table: "OrdenesTrabajo",
                newName: "FacturarA");

            migrationBuilder.RenameColumn(
                name: "direccion_origen",
                table: "OrdenesTrabajo",
                newName: "DireccionOrigen");

            migrationBuilder.RenameColumn(
                name: "direccion_destino",
                table: "OrdenesTrabajo",
                newName: "DireccionDestino");

            migrationBuilder.RenameColumn(
                name: "direccion_cobro",
                table: "OrdenesTrabajo",
                newName: "DireccionCobro");

            migrationBuilder.RenameColumn(
                name: "detalle_servicio",
                table: "OrdenesTrabajo",
                newName: "DetalleServicio");

            migrationBuilder.RenameColumn(
                name: "id_orden",
                table: "OrdenesTrabajo",
                newName: "IdOrden");

            migrationBuilder.RenameColumn(
                name: "completado",
                table: "ImportacionesDocumentos",
                newName: "Completado");

            migrationBuilder.RenameColumn(
                name: "id_importacion",
                table: "ImportacionesDocumentos",
                newName: "IdImportacion");

            migrationBuilder.RenameColumn(
                name: "id_imp_doc",
                table: "ImportacionesDocumentos",
                newName: "IdDocumento");

            migrationBuilder.RenameIndex(
                name: "IX_Importaciones_Documentos_id_importacion",
                table: "ImportacionesDocumentos",
                newName: "IX_ImportacionesDocumentos_IdImportacion");

            migrationBuilder.RenameColumn(
                name: "completado",
                table: "ExportacionesDocumentos",
                newName: "Completado");

            migrationBuilder.RenameColumn(
                name: "id_exportacion",
                table: "ExportacionesDocumentos",
                newName: "IdExportacion");

            migrationBuilder.RenameColumn(
                name: "id_exp_doc",
                table: "ExportacionesDocumentos",
                newName: "IdDocumento");

            migrationBuilder.RenameIndex(
                name: "IX_Exportaciones_Documentos_id_exportacion",
                table: "ExportacionesDocumentos",
                newName: "IX_ExportacionesDocumentos_IdExportacion");

            migrationBuilder.RenameColumn(
                name: "origen",
                table: "ControlVisitas",
                newName: "Origen");

            migrationBuilder.RenameColumn(
                name: "observaciones",
                table: "ControlVisitas",
                newName: "Observaciones");

            migrationBuilder.RenameColumn(
                name: "hora",
                table: "ControlVisitas",
                newName: "Hora");

            migrationBuilder.RenameColumn(
                name: "flete",
                table: "ControlVisitas",
                newName: "Flete");

            migrationBuilder.RenameColumn(
                name: "empresa",
                table: "ControlVisitas",
                newName: "Empresa");

            migrationBuilder.RenameColumn(
                name: "empaque",
                table: "ControlVisitas",
                newName: "Empaque");

            migrationBuilder.RenameColumn(
                name: "destino",
                table: "ControlVisitas",
                newName: "Destino");

            migrationBuilder.RenameColumn(
                name: "corresponsal",
                table: "ControlVisitas",
                newName: "Corresponsal");

            migrationBuilder.RenameColumn(
                name: "tramites_aduana",
                table: "ControlVisitas",
                newName: "TramitesAduana");

            migrationBuilder.RenameColumn(
                name: "telefono_habitacion",
                table: "ControlVisitas",
                newName: "TelefonoHabitacion");

            migrationBuilder.RenameColumn(
                name: "telefono_compania",
                table: "ControlVisitas",
                newName: "TelefonoCompania");

            migrationBuilder.RenameColumn(
                name: "telefono_celular",
                table: "ControlVisitas",
                newName: "TelefonoCelular");

            migrationBuilder.RenameColumn(
                name: "tarifa_total",
                table: "ControlVisitas",
                newName: "TarifaTotal");

            migrationBuilder.RenameColumn(
                name: "puerta_a_puerto",
                table: "ControlVisitas",
                newName: "PuertaAPuerto");

            migrationBuilder.RenameColumn(
                name: "puerta_a_puerta",
                table: "ControlVisitas",
                newName: "PuertaAPuerta");

            migrationBuilder.RenameColumn(
                name: "nombre_cliente",
                table: "ControlVisitas",
                newName: "NombreCliente");

            migrationBuilder.RenameColumn(
                name: "mudanza_local",
                table: "ControlVisitas",
                newName: "MudanzaLocal");

            migrationBuilder.RenameColumn(
                name: "hecho_por",
                table: "ControlVisitas",
                newName: "HechoPor");

            migrationBuilder.RenameColumn(
                name: "fecha_visita",
                table: "ControlVisitas",
                newName: "FechaVisita");

            migrationBuilder.RenameColumn(
                name: "fecha_llamada",
                table: "ControlVisitas",
                newName: "FechaLlamada");

            migrationBuilder.RenameColumn(
                name: "fecha_creacion",
                table: "ControlVisitas",
                newName: "FechaCreacion");

            migrationBuilder.RenameColumn(
                name: "direccion_origen",
                table: "ControlVisitas",
                newName: "DireccionOrigen");

            migrationBuilder.RenameColumn(
                name: "direccion_destino",
                table: "ControlVisitas",
                newName: "DireccionDestino");

            migrationBuilder.RenameColumn(
                name: "compania_maritima",
                table: "ControlVisitas",
                newName: "CompaniaMaritima");

            migrationBuilder.RenameColumn(
                name: "id_visita",
                table: "ControlVisitas",
                newName: "IdVisita");

            migrationBuilder.AlterColumn<string>(
                name: "Referencia",
                table: "Importaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Fecha",
                table: "Importaciones",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Referencia",
                table: "Exportaciones",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Materiales",
                table: "OrdenesTrabajo",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NombreDocumento",
                table: "ImportacionesDocumentos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoAgente",
                table: "ImportacionesDocumentos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NombreDocumento",
                table: "ExportacionesDocumentos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoAgente",
                table: "ExportacionesDocumentos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrdenesTrabajo",
                table: "OrdenesTrabajo",
                column: "IdOrden");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ImportacionesDocumentos",
                table: "ImportacionesDocumentos",
                column: "IdDocumento");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ExportacionesDocumentos",
                table: "ExportacionesDocumentos",
                column: "IdDocumento");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ControlVisitas",
                table: "ControlVisitas",
                column: "IdVisita");

            migrationBuilder.AddForeignKey(
                name: "FK_ExportacionesDocumentos_Exportaciones_IdExportacion",
                table: "ExportacionesDocumentos",
                column: "IdExportacion",
                principalTable: "Exportaciones",
                principalColumn: "IdExportacion",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ImportacionesDocumentos_Importaciones_IdImportacion",
                table: "ImportacionesDocumentos",
                column: "IdImportacion",
                principalTable: "Importaciones",
                principalColumn: "IdImportacion",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
