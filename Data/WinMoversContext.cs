using Microsoft.EntityFrameworkCore;
using WinMovers.Models;

namespace WinMovers.Data
{
    public class WinMoversContext : DbContext
    {
        public WinMoversContext(DbContextOptions<WinMoversContext> options)
            : base(options)
        {
        }
        public DbSet<OrdenTrabajo> OrdenesTrabajo { get; set; }
        public DbSet<ControlVisita> ControlVisitas { get; set; }
        public DbSet<Exportacion> Exportaciones { get; set; }
        public DbSet<ExportacionDocumento> ExportacionesDocumentos { get; set; }
        public DbSet<Importacion> Importaciones { get; set; }
        public DbSet<ImportacionDocumento> ImportacionesDocumentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // =========================================================
            // ORDENES DE TRABAJO
            // =========================================================
            modelBuilder.Entity<OrdenTrabajo>(e =>
    {
        e.ToTable("Ordenes_Trabajo");

        e.HasKey(x => x.IdOrden);

        e.Property(x => x.IdOrden)
            .HasColumnName("id_orden");

        e.Property(x => x.NumeroOT)
            .HasColumnName("numero_ot")
            .IsRequired();

        e.Property(x => x.FechaServicio)
            .HasColumnName("fecha_servicio");

        e.Property(x => x.Fecha)
            .HasColumnName("fecha");

        e.Property(x => x.Hora)
            .HasColumnName("hora");

        e.Property(x => x.NombreCliente)
            .HasColumnName("nombre_cliente")
            .IsRequired();

        e.Property(x => x.TelefonoCelular)
            .HasColumnName("telefono_celular");

        e.Property(x => x.TelefonoResidencia)
            .HasColumnName("telefono_residencia");

        e.Property(x => x.Compania)
            .HasColumnName("compania");

        e.Property(x => x.TelefonoEmpresa)
            .HasColumnName("telefono_empresa");

        e.Property(x => x.Contacto)
            .HasColumnName("contacto");

        e.Property(x => x.DireccionOrigen)
            .HasColumnName("direccion_origen");

        e.Property(x => x.DireccionDestino)
            .HasColumnName("direccion_destino");

        e.Property(x => x.DetalleServicio)
            .HasColumnName("detalle_servicio");

        e.Property(x => x.Materiales)
            .HasColumnName("materiales");

        e.Property(x => x.FacturarA)
            .HasColumnName("facturar_a");

        e.Property(x => x.DireccionCobro)
            .HasColumnName("direccion_cobro");

        e.Property(x => x.HechoPor)
            .HasColumnName("hecho_por");

        e.Property(x => x.FechaCreacion)
            .HasColumnName("fecha_creacion")
            .HasDefaultValueSql("GETDATE()");

        e.Property(x => x.FechaActualizacion)
            .HasColumnName("fecha_actualizacion");
    });

            // =========================================================
            // CONTROL VISITAS
            // =========================================================
            modelBuilder.Entity<ControlVisita>(e =>
            {
                e.ToTable("Control_Visitas");

                e.HasKey(x => x.IdVisita);

                e.Property(x => x.IdVisita)
                    .HasColumnName("id_visita");

                e.Property(x => x.FechaLlamada)
                    .HasColumnName("fecha_llamada");

                e.Property(x => x.FechaVisita)
                    .HasColumnName("fecha_visita");

                e.Property(x => x.Hora)
                    .HasColumnName("hora");

                e.Property(x => x.NombreCliente)
                    .HasColumnName("nombre_cliente")
                    .IsRequired();

                e.Property(x => x.TelefonoHabitacion)
                    .HasColumnName("telefono_habitacion");

                e.Property(x => x.TelefonoCelular)
                    .HasColumnName("telefono_celular");

                e.Property(x => x.Empresa)
                    .HasColumnName("empresa");

                e.Property(x => x.TelefonoCompania)
                    .HasColumnName("telefono_compania");

                e.Property(x => x.DireccionOrigen)
                    .HasColumnName("direccion_origen");

                e.Property(x => x.DireccionDestino)
                    .HasColumnName("direccion_destino");

                e.Property(x => x.Observaciones)
                    .HasColumnName("observaciones");

                e.Property(x => x.PuertaAPuerta)
                    .HasColumnName("puerta_a_puerta");

                e.Property(x => x.PuertaAPuerto)
                    .HasColumnName("puerta_a_puerto");

                e.Property(x => x.Empaque)
                    .HasColumnName("empaque");

                e.Property(x => x.MudanzaLocal)
                    .HasColumnName("mudanza_local");

                e.Property(x => x.Origen)
                    .HasColumnName("origen");

                e.Property(x => x.TramitesAduana)
                    .HasColumnName("tramites_aduana");

                e.Property(x => x.Flete)
                    .HasColumnName("flete");

                e.Property(x => x.Destino)
                    .HasColumnName("destino");

                e.Property(x => x.TarifaTotal)
                    .HasColumnName("tarifa_total");

                e.Property(x => x.CompaniaMaritima)
                    .HasColumnName("compania_maritima");

                e.Property(x => x.Corresponsal)
                    .HasColumnName("corresponsal");

                e.Property(x => x.HechoPor)
                    .HasColumnName("hecho_por");

                e.Property(x => x.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("GETDATE()");

                e.Property(x => x.FechaActualizacion)
                    .HasColumnName("fecha_actualizacion");
            });

            // =========================================================
            // EXPORTACIONES
            // =========================================================
            modelBuilder.Entity<Exportacion>(e =>
            {
                e.ToTable("Exportaciones");

                e.HasKey(x => x.IdExportacion);

                e.Property(x => x.IdExportacion)
                    .HasColumnName("id_exportacion");

                e.Property(x => x.NombreCliente)
                    .HasColumnName("nombre_cliente")
                    .IsRequired();

                e.Property(x => x.Referencia)
                    .HasColumnName("referencia");

                e.Property(x => x.Fecha)
                    .HasColumnName("fecha");

                e.Property(x => x.Observaciones)
                    .HasColumnName("observaciones");

                e.Property(x => x.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("GETDATE()");

                e.Property(x => x.FechaActualizacion)
                    .HasColumnName("fecha_actualizacion");

                e.HasMany(x => x.Documentos)
                    .WithOne(d => d.Exportacion)
                    .HasForeignKey(d => d.IdExportacion)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================================================
            // EXPORTACIONES DOCUMENTOS
            // =========================================================
            modelBuilder.Entity<ExportacionDocumento>(e =>
            {
                e.ToTable("Exportaciones_Documentos");

                e.HasKey(x => x.IdExpDoc);

                e.Property(x => x.IdExpDoc)
                    .HasColumnName("id_exp_doc");

                e.Property(x => x.IdExportacion)
                    .HasColumnName("id_exportacion");

                e.Property(x => x.IdTipoDocumento)
                    .HasColumnName("id_tipo_documento");

                e.Property(x => x.TipoChecklist)
                    .HasColumnName("tipo_checklist");

                e.Property(x => x.Completado)
                    .HasColumnName("completado");

                e.Property(x => x.FechaCompletado)
                    .HasColumnName("fecha_completado");

                e.Property(x => x.Observaciones)
                    .HasColumnName("observaciones");
            });

            // =========================================================
            // IMPORTACIONES
            // =========================================================
            modelBuilder.Entity<Importacion>(e =>
            {
                e.ToTable("Importaciones");

                e.HasKey(x => x.IdImportacion);

                e.Property(x => x.IdImportacion)
                    .HasColumnName("id_importacion");

                e.Property(x => x.NombreCliente)
                    .HasColumnName("nombre_cliente")
                    .IsRequired();

                e.Property(x => x.Referencia)
                    .HasColumnName("referencia");

                e.Property(x => x.Fecha)
                    .HasColumnName("fecha");

                e.Property(x => x.Observaciones)
                    .HasColumnName("observaciones");

                e.Property(x => x.FechaCreacion)
                    .HasColumnName("fecha_creacion")
                    .HasDefaultValueSql("GETDATE()");

                e.Property(x => x.FechaActualizacion)
                    .HasColumnName("fecha_actualizacion");

                e.HasMany(x => x.Documentos)
                    .WithOne(d => d.Importacion)
                    .HasForeignKey(d => d.IdImportacion)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================================================
            // IMPORTACIONES DOCUMENTOS
            // =========================================================
            modelBuilder.Entity<ImportacionDocumento>(e =>
            {
                e.ToTable("Importaciones_Documentos");

                e.HasKey(x => x.IdImpDoc);

                e.Property(x => x.IdImpDoc)
                    .HasColumnName("id_imp_doc");

                e.Property(x => x.IdImportacion)
                    .HasColumnName("id_importacion");

                e.Property(x => x.IdTipoDocumento)
                    .HasColumnName("id_tipo_documento");

                e.Property(x => x.TipoChecklist)
                    .HasColumnName("tipo_checklist");

                e.Property(x => x.Completado)
                    .HasColumnName("completado");

                e.Property(x => x.FechaCompletado)
                    .HasColumnName("fecha_completado");

                e.Property(x => x.Observaciones)
                    .HasColumnName("observaciones");
            });
        }
    }
}