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
                e.ToTable("Ordenes_Trabajo", "dbo");

                // PK
                e.HasKey(x => x.IdOrden);
                e.Property(x => x.IdOrden).HasColumnName("id_orden");

                // Identificacion
                e.Property(x => x.NumeroOT).HasColumnName("numero_ot");

                // Fechas
                e.Property(x => x.FechaServicio).HasColumnName("fecha_servicio");
                e.Property(x => x.Fecha).HasColumnName("fecha");
                e.Property(x => x.Hora).HasColumnName("hora");
                e.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion");
                e.Property(x => x.FechaActualizacion).HasColumnName("fecha_actualizacion");

                // Cliente
                e.Property(x => x.NombreCliente).HasColumnName("nombre_cliente");
                e.Property(x => x.TelefonoCelular).HasColumnName("telefono_celular");
                e.Property(x => x.TelefonoResidencia).HasColumnName("telefono_residencia");
                e.Property(x => x.Compania).HasColumnName("compania");
                e.Property(x => x.TelefonoEmpresa).HasColumnName("telefono_empresa");
                e.Property(x => x.Contacto).HasColumnName("contacto");

                // Direcciones
                e.Property(x => x.DireccionOrigen).HasColumnName("direccion_origen");
                e.Property(x => x.DireccionDestino).HasColumnName("direccion_destino");
                e.Property(x => x.DireccionCobro).HasColumnName("direccion_cobro");

                // Detalle
                e.Property(x => x.DetalleServicio).HasColumnName("detalle_servicio");
                e.Property(x => x.Materiales).HasColumnName("materiales");

                // Facturacion
                e.Property(x => x.FacturarA).HasColumnName("facturar_a");

                // Usuario
                e.Property(x => x.HechoPor).HasColumnName("hecho_por");

                e.HasKey(o => o.IdOrden);
                e.Property(o => o.NumeroOT).IsRequired();
                e.Property(o => o.NombreCliente).IsRequired();
                e.Property(o => o.FechaCreacion).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<ControlVisita>(e =>
            {
                e.ToTable("Control_Visitas", "dbo");

                e.HasKey(x => x.IdVisita);
                e.Property(x => x.IdVisita).HasColumnName("id_visita");

                e.Property(x => x.CompaniaMaritima).HasColumnName("compania_maritima");

                e.Property(x => x.DireccionOrigen).HasColumnName("direccion_origen");
                e.Property(x => x.DireccionDestino).HasColumnName("direccion_destino");

                e.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion");
                e.Property(x => x.FechaLlamada).HasColumnName("fecha_llamada");
                e.Property(x => x.FechaVisita).HasColumnName("fecha_visita");

                e.Property(x => x.HechoPor).HasColumnName("hecho_por");

                e.Property(x => x.MudanzaLocal).HasColumnName("mudanza_local");

                e.Property(x => x.NombreCliente).HasColumnName("nombre_cliente");

                e.Property(x => x.PuertaAPuerta).HasColumnName("puerta_a_puerta");
                e.Property(x => x.PuertaAPuerto).HasColumnName("puerta_a_puerto");

                e.Property(x => x.TarifaTotal).HasColumnName("tarifa_total");

                e.Property(x => x.TelefonoCelular).HasColumnName("telefono_celular");
                e.Property(x => x.TelefonoCompania).HasColumnName("telefono_compania");
                e.Property(x => x.TelefonoHabitacion).HasColumnName("telefono_habitacion");

                e.Property(x => x.TramitesAduana).HasColumnName("tramites_aduana");

                e.HasKey(c => c.IdVisita);
                e.Property(c => c.NombreCliente).IsRequired();
                e.Property(c => c.FechaCreacion).HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<Exportacion>(e =>
            {


                e.ToTable("Exportaciones", "dbo");

                e.HasKey(x => x.IdExportacion);

                e.Property(x => x.IdExportacion)
                    .HasColumnName("id_exportacion");

                e.Property(x => x.NombreCliente)
                    .HasColumnName("nombre_cliente");

                e.Property(x => x.Referencia)
                    .HasColumnName("referencia");

                e.Property(x => x.Fecha)
                    .HasColumnName("fecha");

                e.Property(x => x.Observaciones)
                    .HasColumnName("observaciones");

                e.Property(x => x.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("GETDATE()");


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

                e.ToTable("Exportaciones_Documentos", "dbo");

                e.HasKey(d => d.IdDocumento);

                e.Property(x => x.IdExportacion)
        .HasColumnName("id_exportacion");



                e.Property(x => x.IdDocumento)
                    .HasColumnName("id_documento");

                e.Property(x => x.NombreDocumento)
                    .HasColumnName("nombre_documento");

                e.Property(x => x.TipoAgente)
                    .HasColumnName("tipo_agente");

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
