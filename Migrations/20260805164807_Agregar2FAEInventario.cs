using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WinMovers.Migrations
{
    /// <inheritdoc />
    public partial class Agregar2FAEInventario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "clave_autenticador",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Inventario",
                columns: table => new
                {
                    id_material = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre_material = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    unidad = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    existencias = table.Column<int>(type: "int", nullable: false),
                    stock_minimo = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    fecha_creacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    fecha_actualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventario", x => x.id_material);
                });

            migrationBuilder.CreateTable(
                name: "OrdenTrabajoMaterial",
                columns: table => new
                {
                    id_orden_material = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_orden = table.Column<int>(type: "int", nullable: false),
                    id_material = table.Column<int>(type: "int", nullable: false),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    fecha_asignacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenTrabajoMaterial", x => x.id_orden_material);
                    table.ForeignKey(
                        name: "FK_OrdenTrabajoMaterial_Inventario_id_material",
                        column: x => x.id_material,
                        principalTable: "Inventario",
                        principalColumn: "id_material",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrdenTrabajoMaterial_Ordenes_Trabajo_id_orden",
                        column: x => x.id_orden,
                        principalTable: "Ordenes_Trabajo",
                        principalColumn: "id_orden",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajoMaterial_id_material",
                table: "OrdenTrabajoMaterial",
                column: "id_material");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenTrabajoMaterial_id_orden",
                table: "OrdenTrabajoMaterial",
                column: "id_orden");

            // Reglas de negocio que el scaffolding de EF no genera pero que el
            // script Database/WinMovers_Database.sql sí crea. Se replican aquí
            // para que ambas vías de aprovisionamiento den el mismo esquema.
            migrationBuilder.Sql(
                "ALTER TABLE Inventario ADD CONSTRAINT CK_Inventario_Existencias CHECK (existencias >= 0);");
            migrationBuilder.Sql(
                "ALTER TABLE OrdenTrabajoMaterial ADD CONSTRAINT CK_OrdenTrabajoMaterial_Cantidad CHECK (cantidad > 0);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdenTrabajoMaterial");

            migrationBuilder.DropTable(
                name: "Inventario");

            migrationBuilder.DropColumn(
                name: "clave_autenticador",
                table: "Usuarios");
        }
    }
}
