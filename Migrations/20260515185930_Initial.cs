using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinMovers.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ControlVisitas",
                columns: table => new
                {
                    IdVisita = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaLlamada = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaVisita = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Hora = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NombreCliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TelefonoHabitacion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TelefonoCelular = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Empresa = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TelefonoCompania = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DireccionOrigen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DireccionDestino = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PuertaAPuerta = table.Column<bool>(type: "bit", nullable: false),
                    PuertaAPuerto = table.Column<bool>(type: "bit", nullable: false),
                    Empaque = table.Column<bool>(type: "bit", nullable: false),
                    MudanzaLocal = table.Column<bool>(type: "bit", nullable: false),
                    Origen = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TramitesAduana = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Flete = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Destino = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TarifaTotal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CompaniaMaritima = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Corresponsal = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    HechoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ControlVisitas", x => x.IdVisita);
                });

            migrationBuilder.CreateTable(
                name: "Exportaciones",
                columns: table => new
                {
                    IdExportacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exportaciones", x => x.IdExportacion);
                });

            migrationBuilder.CreateTable(
                name: "Importaciones",
                columns: table => new
                {
                    IdImportacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreCliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Referencia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Importaciones", x => x.IdImportacion);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesTrabajo",
                columns: table => new
                {
                    IdOrden = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroOT = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaServicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Hora = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    NombreCliente = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TelefonoCelular = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TelefonoResidencia = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Compania = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TelefonoEmpresa = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Contacto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DireccionOrigen = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DireccionDestino = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DetalleServicio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Materiales = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FacturarA = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    DireccionCobro = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HechoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesTrabajo", x => x.IdOrden);
                });

            migrationBuilder.CreateTable(
                name: "ExportacionesDocumentos",
                columns: table => new
                {
                    IdDocumento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdExportacion = table.Column<int>(type: "int", nullable: false),
                    NombreDocumento = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoAgente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Completado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportacionesDocumentos", x => x.IdDocumento);
                    table.ForeignKey(
                        name: "FK_ExportacionesDocumentos_Exportaciones_IdExportacion",
                        column: x => x.IdExportacion,
                        principalTable: "Exportaciones",
                        principalColumn: "IdExportacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportacionesDocumentos",
                columns: table => new
                {
                    IdDocumento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdImportacion = table.Column<int>(type: "int", nullable: false),
                    NombreDocumento = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoAgente = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Completado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportacionesDocumentos", x => x.IdDocumento);
                    table.ForeignKey(
                        name: "FK_ImportacionesDocumentos_Importaciones_IdImportacion",
                        column: x => x.IdImportacion,
                        principalTable: "Importaciones",
                        principalColumn: "IdImportacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExportacionesDocumentos_IdExportacion",
                table: "ExportacionesDocumentos",
                column: "IdExportacion");

            migrationBuilder.CreateIndex(
                name: "IX_ImportacionesDocumentos_IdImportacion",
                table: "ImportacionesDocumentos",
                column: "IdImportacion");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ControlVisitas");

            migrationBuilder.DropTable(
                name: "ExportacionesDocumentos");

            migrationBuilder.DropTable(
                name: "ImportacionesDocumentos");

            migrationBuilder.DropTable(
                name: "OrdenesTrabajo");

            migrationBuilder.DropTable(
                name: "Exportaciones");

            migrationBuilder.DropTable(
                name: "Importaciones");
        }
    }
}
