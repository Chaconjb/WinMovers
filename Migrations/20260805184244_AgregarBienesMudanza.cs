using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinMovers.Migrations
{
    /// <inheritdoc />
    public partial class AgregarBienesMudanza : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BienesMudanza",
                columns: table => new
                {
                    id_bien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_orden = table.Column<int>(type: "int", nullable: false),
                    nombre_bien = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    condicion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    fecha_registro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BienesMudanza", x => x.id_bien);
                    table.ForeignKey(
                        name: "FK_BienesMudanza_Ordenes_Trabajo_id_orden",
                        column: x => x.id_orden,
                        principalTable: "Ordenes_Trabajo",
                        principalColumn: "id_orden",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UX_BienesMudanza_Orden_Nombre",
                table: "BienesMudanza",
                columns: new[] { "id_orden", "nombre_bien" },
                unique: true);

            // Misma convención que Inventario y OrdenTrabajoMaterial: la regla
            // de negocio vive también en la base, no sólo en el modelo.
            migrationBuilder.Sql(
                "ALTER TABLE BienesMudanza ADD CONSTRAINT CK_BienesMudanza_Cantidad CHECK (cantidad > 0);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BienesMudanza");
        }
    }
}
