using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinMovers.Migrations
{
    /// <inheritdoc />
    public partial class AgregarListasEmbalaje : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ListasEmbalaje",
                columns: table => new
                {
                    id_lista = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_orden = table.Column<int>(type: "int", nullable: false),
                    numero_lista = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    responsable = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fecha_generacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    fecha_actualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasEmbalaje", x => x.id_lista);
                    table.ForeignKey(
                        name: "FK_ListasEmbalaje_Ordenes_Trabajo_id_orden",
                        column: x => x.id_orden,
                        principalTable: "Ordenes_Trabajo",
                        principalColumn: "id_orden",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListasEmbalajeDetalle",
                columns: table => new
                {
                    id_detalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_lista = table.Column<int>(type: "int", nullable: false),
                    id_bien = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListasEmbalajeDetalle", x => x.id_detalle);
                    table.ForeignKey(
                        name: "FK_ListasEmbalajeDetalle_BienesMudanza_id_bien",
                        column: x => x.id_bien,
                        principalTable: "BienesMudanza",
                        principalColumn: "id_bien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ListasEmbalajeDetalle_ListasEmbalaje_id_lista",
                        column: x => x.id_lista,
                        principalTable: "ListasEmbalaje",
                        principalColumn: "id_lista",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UX_ListasEmbalaje_Orden",
                table: "ListasEmbalaje",
                column: "id_orden",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ListasEmbalajeDetalle_id_bien",
                table: "ListasEmbalajeDetalle",
                column: "id_bien");

            migrationBuilder.CreateIndex(
                name: "UX_ListasEmbalajeDetalle_Lista_Bien",
                table: "ListasEmbalajeDetalle",
                columns: new[] { "id_lista", "id_bien" },
                unique: true);

            // Misma convención que el resto del módulo: la regla vive también
            // en la base, no sólo en el modelo.
            migrationBuilder.Sql(
                "ALTER TABLE ListasEmbalajeDetalle ADD CONSTRAINT CK_ListasEmbalajeDetalle_Cantidad CHECK (cantidad > 0);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ListasEmbalajeDetalle");

            migrationBuilder.DropTable(
                name: "ListasEmbalaje");
        }
    }
}
