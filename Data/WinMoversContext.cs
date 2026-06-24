using Microsoft.EntityFrameworkCore;
using AvanceWinMovers.Models;

namespace AvanceWinMovers.Data;

public class WinMoversContext : DbContext
{
    public WinMoversContext(DbContextOptions<WinMoversContext> options) : base(options)
    {
    }

    public DbSet<OrdenTrabajo> OrdenesTrabajo { get; set; }
    public DbSet<ControlVisita> ControlVisitas { get; set; }
    public DbSet<Cotizacion> Cotizaciones { get; set; }
    public DbSet<InventarioItem> InventarioItems { get; set; }
    public DbSet<Empleado> Empleados { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurar tabla Ordenes_Trabajo
        modelBuilder.Entity<OrdenTrabajo>(entity =>
        {
            entity.ToTable("Ordenes_Trabajo");
            entity.HasKey(e => e.id_orden);
            entity.Property(e => e.nombre_cliente).IsRequired().HasMaxLength(255);
        });

        // Configurar tabla Control_Visitas
        modelBuilder.Entity<ControlVisita>(entity =>
        {
            entity.ToTable("Control_Visitas");
            entity.HasKey(e => e.id_visita);
            entity.Property(e => e.nombre_cliente).IsRequired().HasMaxLength(255);
        });

        modelBuilder.Entity<Cotizacion>(entity =>
        {
            entity.ToTable("Cotizaciones");
            entity.HasKey(e => e.id_cotizacion);
            entity.Property(e => e.nombre_cliente).IsRequired().HasMaxLength(150);
            entity.Property(e => e.numero_cotizacion).IsRequired().HasMaxLength(20);
        });

        modelBuilder.Entity<InventarioItem>(entity =>
        {
            entity.ToTable("Inventario");
            entity.HasKey(e => e.id_item);
            entity.Property(e => e.nombre_material).IsRequired().HasMaxLength(150);
            entity.Property(e => e.unidad).HasMaxLength(30);
        });

        modelBuilder.Entity<Empleado>(entity =>
        {
            entity.ToTable("Empleados");
            entity.HasKey(e => e.id_empleado);
            entity.Property(e => e.nombre_completo).IsRequired().HasMaxLength(150);
            entity.Property(e => e.puesto).IsRequired().HasMaxLength(100);
        });
    }
}
