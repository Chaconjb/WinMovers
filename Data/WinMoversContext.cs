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
        public DbSet<CatalogoDocumento> CatalogoDocumentos { get; set; }
        public DbSet<Exportacion> Exportaciones { get; set; }
        public DbSet<ExportacionDocumento> ExportacionesDocumentos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Importacion> Importaciones { get; set; }
        public DbSet<ImportacionDocumento> ImportacionesDocumentos { get; set; }
        public DbSet<ImportacionArchivo> ImportacionesArchivos { get; set; }
        public DbSet<ExportacionArchivo> ExportacionesArchivos { get; set; }
        public DbSet<OrdenTrabajoArchivo> OrdenesTrabajosArchivos { get; set; }
        public DbSet<OrdenTrabajoHistorial> OrdenesTrabajoHistorial { get; set; }
        public DbSet<ClienteHistorial> ClienteHistorial { get; set; }
        public DbSet<OrdenTrabajoNota> OrdenesTrabajoNotas { get; set; }
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
            // ORDENES TRABAJO ARCHIVOS
            // =========================================================
            modelBuilder.Entity<OrdenTrabajoArchivo>(e =>
            {
                e.ToTable("OrdenesTrabajo_Archivos");
                e.HasKey(x => x.IdArchivo);
                e.Property(x => x.IdArchivo).HasColumnName("id_archivo");
                e.Property(x => x.IdOrden).HasColumnName("id_orden");
                e.Property(x => x.NombreOriginal).HasColumnName("nombre_original").IsRequired();
                e.Property(x => x.NombreGuardado).HasColumnName("nombre_guardado").IsRequired();
                e.Property(x => x.TipoMime).HasColumnName("tipo_mime").IsRequired();
                e.Property(x => x.TamanioBytes).HasColumnName("tamanio_bytes");
                e.Property(x => x.FechaCarga).HasColumnName("fecha_carga").HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.OrdenTrabajo)
                    .WithMany(o => o.Archivos)
                    .HasForeignKey(x => x.IdOrden)
                    .OnDelete(DeleteBehavior.Cascade);
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
            // CATALOGO DOCUMENTOS
            // =========================================================
            modelBuilder.Entity<CatalogoDocumento>(e =>
            {
                e.ToTable("Catalogo_Documentos");

                e.HasKey(x => x.IdTipoDocumento);

                e.Property(x => x.IdTipoDocumento)
                    .HasColumnName("id_tipo_documento");

                e.Property(x => x.Nombre)
                    .HasColumnName("nombre")
                    .IsRequired();

                e.Property(x => x.AplicaExportacion)
                    .HasColumnName("aplica_exportacion");

                e.Property(x => x.AplicaImportacion)
                    .HasColumnName("aplica_importacion");

                e.Property(x => x.AplicaWinMovers)
                    .HasColumnName("aplica_winmovers");

                e.Property(x => x.AplicaOtroAgente)
                    .HasColumnName("aplica_otro_agente");

                e.Property(x => x.OrdenPresentacion)
                    .HasColumnName("orden_presentacion");

                e.Property(x => x.Activo)
                    .HasColumnName("activo");
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
                //Relación con TipoDocumento
                e.HasOne(x => x.TipoDocumento)
                    .WithMany()
                    .HasForeignKey(x => x.IdTipoDocumento)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // =========================================================
            // EXPORTACIONES ARCHIVOS
            // =========================================================
            modelBuilder.Entity<ExportacionArchivo>(e =>
            {
                e.ToTable("Exportaciones_Archivos");
                e.HasKey(x => x.IdArchivo);
                e.Property(x => x.IdArchivo).HasColumnName("id_archivo");
                e.Property(x => x.IdExportacion).HasColumnName("id_exportacion");
                e.Property(x => x.NombreOriginal).HasColumnName("nombre_original").IsRequired();
                e.Property(x => x.NombreGuardado).HasColumnName("nombre_guardado").IsRequired();
                e.Property(x => x.TipoMime).HasColumnName("tipo_mime").IsRequired();
                e.Property(x => x.TamanioBytes).HasColumnName("tamanio_bytes");
                e.Property(x => x.FechaCarga).HasColumnName("fecha_carga").HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.Exportacion)
                    .WithMany(i => i.Archivos)
                    .HasForeignKey(x => x.IdExportacion)
                    .OnDelete(DeleteBehavior.Cascade);
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
                //Relación con TipoDocumento
                e.HasOne(x => x.TipoDocumento)
                    .WithMany()
                    .HasForeignKey(x => x.IdTipoDocumento)
                    .OnDelete(DeleteBehavior.Restrict);
            });
            // =========================================================
            // IMPORTACIONES ARCHIVOS
            // =========================================================
            modelBuilder.Entity<ImportacionArchivo>(e =>
            {
                e.ToTable("Importaciones_Archivos");

                e.HasKey(x => x.IdArchivo);

                e.Property(x => x.IdArchivo)
                    .HasColumnName("id_archivo");

                e.Property(x => x.IdImportacion)
                    .HasColumnName("id_importacion");

                e.Property(x => x.NombreOriginal)
                    .HasColumnName("nombre_original")
                    .IsRequired();

                e.Property(x => x.NombreGuardado)
                    .HasColumnName("nombre_guardado")
                    .IsRequired();

                e.Property(x => x.TipoMime)
                    .HasColumnName("tipo_mime")
                    .IsRequired();

                e.Property(x => x.TamanioBytes)
                    .HasColumnName("tamanio_bytes");

                e.Property(x => x.FechaCarga)
                    .HasColumnName("fecha_carga")
                    .HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.Importacion)
                    .WithMany(i => i.Archivos)
                    .HasForeignKey(x => x.IdImportacion)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // Dentro de OnModelCreating, al final:

            // =========================================================
            // ORDENES TRABAJO HISTORIAL
            // =========================================================
            modelBuilder.Entity<OrdenTrabajoHistorial>(e =>
            {
                e.ToTable("Ordenes_Trabajo_Historial");
                e.HasKey(x => x.IdHistorial);
                e.Property(x => x.IdHistorial).HasColumnName("id_historial");
                e.Property(x => x.IdOrden).HasColumnName("id_orden");
                e.Property(x => x.CampoModificado).HasColumnName("campo_modificado").IsRequired();
                e.Property(x => x.ValorAnterior).HasColumnName("valor_anterior");
                e.Property(x => x.ValorNuevo).HasColumnName("valor_nuevo");
                e.Property(x => x.Usuario).HasColumnName("usuario");
                e.Property(x => x.FechaCambio).HasColumnName("fecha_cambio").HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.OrdenTrabajo)
                    .WithMany(o => o.Historial)
                    .HasForeignKey(x => x.IdOrden)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // =========================================================
            // CLIENTES HISTORIAL
            // =========================================================
            modelBuilder.Entity<ClienteHistorial>(e =>
            {
                e.ToTable("Clientes_Historial");

                e.HasKey(x => x.IdHistorial);

                e.Property(x => x.IdHistorial)
                    .HasColumnName("id_historial");

                e.Property(x => x.IdCliente)
                    .HasColumnName("id_cliente");

                e.Property(x => x.CampoModificado)
                    .HasColumnName("campo_modificado")
                    .IsRequired();

                e.Property(x => x.ValorAnterior)
                    .HasColumnName("valor_anterior");

                e.Property(x => x.ValorNuevo)
                    .HasColumnName("valor_nuevo");

                e.Property(x => x.Usuario)
                    .HasColumnName("usuario");

                e.Property(x => x.FechaCambio)
                    .HasColumnName("fecha_cambio")
                    .HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.Cliente)
                    .WithMany(c => c.Historial)
                    .HasForeignKey(x => x.IdCliente)
                    .OnDelete(DeleteBehavior.Cascade);
            });
            // =========================================================
            // ORDENES TRABAJO NOTAS
            // =========================================================
            modelBuilder.Entity<OrdenTrabajoNota>(e =>
            {
                e.ToTable("Ordenes_Trabajo_Notas");
                e.HasKey(x => x.IdNota);
                e.Property(x => x.IdNota).HasColumnName("id_nota");
                e.Property(x => x.IdOrden).HasColumnName("id_orden");
                e.Property(x => x.Contenido).HasColumnName("contenido").IsRequired();
                e.Property(x => x.Usuario).HasColumnName("usuario");
                e.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("GETDATE()");
                e.Property(x => x.FechaActualizacion).HasColumnName("fecha_actualizacion");

                e.HasOne(x => x.OrdenTrabajo)
                    .WithMany(o => o.Notas)
                    .HasForeignKey(x => x.IdOrden)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}