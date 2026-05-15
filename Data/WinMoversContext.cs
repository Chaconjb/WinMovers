using Microsoft.EntityFrameworkCore;
using WinMovers.Models;

namespace WinMovers.Data
{
    public class WinMoversContext : DbContext
    {
        public WinMoversContext(DbContextOptions<WinMoversContext> options) : base(options) { }

        public DbSet<OrdenTrabajo> OrdenesTrabajo { get; set; }
        public DbSet<ControlVisita> ControlVisitas { get; set; }
        public DbSet<Exportacion> Exportaciones { get; set; }
        public DbSet<ExportacionDocumento> ExportacionesDocumentos { get; set; }
        public DbSet<Importacion> Importaciones { get; set; }
        public DbSet<ImportacionDocumento> ImportacionesDocumentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrdenTrabajo>(e =>
            {
                e.HasKey(o => o.IdOrden);
                e.Property(o => o.NumeroOT).IsRequired();
                e.Property(o => o.NombreCliente).IsRequired();
                e.Property(o => o.FechaCreacion).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<ControlVisita>(e =>
            {
                e.HasKey(c => c.IdVisita);
                e.Property(c => c.NombreCliente).IsRequired();
                e.Property(c => c.FechaCreacion).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Exportacion>(e =>
            {
                e.HasKey(x => x.IdExportacion);
                e.Property(x => x.NombreCliente).IsRequired();
                e.Property(x => x.FechaCreacion).HasDefaultValueSql("GETDATE()");
                e.HasMany(x => x.Documentos)
                 .WithOne(d => d.Exportacion)
                 .HasForeignKey(d => d.IdExportacion)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExportacionDocumento>(e =>
            {
                e.HasKey(d => d.IdDocumento);
            });

            modelBuilder.Entity<Importacion>(e =>
            {
                e.HasKey(i => i.IdImportacion);
                e.Property(i => i.NombreCliente).IsRequired();
                e.Property(i => i.FechaCreacion).HasDefaultValueSql("GETDATE()");
                e.HasMany(i => i.Documentos)
                 .WithOne(d => d.Importacion)
                 .HasForeignKey(d => d.IdImportacion)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ImportacionDocumento>(e =>
            {
                e.HasKey(d => d.IdDocumento);
            });
        }
    }
}
