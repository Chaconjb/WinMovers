using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WinMovers.Models;

namespace WinMovers.Data
{
    public class WinMoversContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
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
        public DbSet<AccesoAuditoria> AccesosAuditoria { get; set; }
        public DbSet<RolAuditoria> RolesAuditoria { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<Inventario> Inventario { get; set; }
        public DbSet<OrdenTrabajoMaterial> OrdenTrabajoMaterial { get; set; }

        public DbSet<BienMudanza> BienesMudanza { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================
            // IDENTITY - Renombrado de tablas a la convenci�n usada en el proyecto hasta ahora (sprint 2)
            // =========================================================
            modelBuilder.Entity<ApplicationUser>(e =>
            {
                e.ToTable("Usuarios");
                e.Property(x => x.Id).HasColumnName("id_usuario");
                e.Property(x => x.NombreCompleto).HasColumnName("nombre_completo").IsRequired().HasMaxLength(200);
                e.Property(x => x.Activo).HasColumnName("activo").HasDefaultValue(true);
                e.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("GETDATE()");
                e.Property(x => x.DebeCambiarContrasena).HasColumnName("debe_cambiar_contrasena").HasDefaultValue(false);
                e.Property(x => x.UserName).HasColumnName("nombre_usuario");
                e.Property(x => x.NormalizedUserName).HasColumnName("nombre_usuario_normalizado");
                e.Property(x => x.Email).HasColumnName("correo");
                e.Property(x => x.NormalizedEmail).HasColumnName("correo_normalizado");
                e.Property(x => x.EmailConfirmed).HasColumnName("correo_confirmado");
                e.Property(x => x.PasswordHash).HasColumnName("contrasena_hash");
                e.Property(x => x.PhoneNumber).HasColumnName("telefono");
                e.Property(x => x.PhoneNumberConfirmed).HasColumnName("telefono_confirmado");
                e.Property(x => x.TwoFactorEnabled).HasColumnName("doble_factor_habilitado");
                e.Property(x => x.clave_autenticador).HasColumnName("clave_autenticador");
                e.Property(x => x.LockoutEnd).HasColumnName("bloqueo_hasta");
                e.Property(x => x.LockoutEnabled).HasColumnName("bloqueo_habilitado");
                e.Property(x => x.AccessFailedCount).HasColumnName("intentos_fallidos");
                e.Property(x => x.SecurityStamp).HasColumnName("security_stamp");
                e.Property(x => x.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            });

            modelBuilder.Entity<ApplicationRole>(e =>
            {
                e.ToTable("Roles");
                e.Property(x => x.Id).HasColumnName("id_rol");
                e.Property(x => x.Name).HasColumnName("nombre");
                e.Property(x => x.NormalizedName).HasColumnName("nombre_normalizado");
                e.Property(x => x.Descripcion).HasColumnName("descripcion").HasMaxLength(250);
            });

            modelBuilder.Entity<IdentityUserRole<int>>(e => e.ToTable("Usuarios_Roles"));
            modelBuilder.Entity<IdentityUserClaim<int>>(e => e.ToTable("Usuarios_Claims"));
            modelBuilder.Entity<IdentityUserLogin<int>>(e => e.ToTable("Usuarios_Logins"));
            modelBuilder.Entity<IdentityRoleClaim<int>>(e => e.ToTable("Roles_Claims"));
            modelBuilder.Entity<IdentityUserToken<int>>(e => e.ToTable("Usuarios_Tokens"));

            // =========================================================
            // ACCESOS AUDITORIA (HU-AUT-001 Escenario 3)
            // =========================================================
            modelBuilder.Entity<AccesoAuditoria>(e =>
            {
                e.ToTable("Accesos_Auditoria");
                e.HasKey(x => x.IdAuditoria);
                e.Property(x => x.IdAuditoria).HasColumnName("id_auditoria");
                e.Property(x => x.IdUsuario).HasColumnName("id_usuario");
                e.Property(x => x.CorreoIntentado).HasColumnName("correo_intentado").IsRequired().HasMaxLength(256);
                e.Property(x => x.Exitoso).HasColumnName("exitoso");
                e.Property(x => x.Motivo).HasColumnName("motivo").HasMaxLength(200);
                e.Property(x => x.IpAddress).HasColumnName("ip_address").HasMaxLength(45);
                e.Property(x => x.Fecha).HasColumnName("fecha").HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.Usuario)
                    .WithMany()
                    .HasForeignKey(x => x.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // =========================================================
            // ROLES AUDITORIA (HU-AUT-003)
            // =========================================================
            modelBuilder.Entity<RolAuditoria>(e =>
            {
                e.ToTable("Roles_Auditoria");
                e.HasKey(x => x.IdAuditoria);
                e.Property(x => x.IdAuditoria).HasColumnName("id_auditoria");
                e.Property(x => x.Accion).HasColumnName("accion").IsRequired().HasMaxLength(50);
                e.Property(x => x.NombreRol).HasColumnName("nombre_rol").IsRequired().HasMaxLength(256);
                e.Property(x => x.IdUsuarioAfectado).HasColumnName("id_usuario_afectado");
                e.Property(x => x.IdUsuarioResponsable).HasColumnName("id_usuario_responsable");
                e.Property(x => x.Detalle).HasColumnName("detalle");
                e.Property(x => x.Fecha).HasColumnName("fecha").HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.UsuarioAfectado)
                    .WithMany()
                    .HasForeignKey(x => x.IdUsuarioAfectado)
                    .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.UsuarioResponsable)
                    .WithMany()
                    .HasForeignKey(x => x.IdUsuarioResponsable)
                    .OnDelete(DeleteBehavior.NoAction);
            });

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
        // Nueva llave for�nea
        e.Property(x => x.IdCliente)
            .HasColumnName("id_cliente");

        // Relaci�n con Cliente
        e.HasOne(x => x.Cliente)
            .WithMany(c => c.OrdenesTrabajo)
            .HasForeignKey(x => x.IdCliente)
            .OnDelete(DeleteBehavior.Restrict);
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
                //Relaci�n con TipoDocumento
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
                //Relaci�n con TipoDocumento
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
                e.Property(x => x.IdUsuario).HasColumnName("id_usuario");
                e.Property(x => x.FechaCambio).HasColumnName("fecha_cambio").HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.Usuario)
                    .WithMany(u => u.HistorialOrdenes)
                    .HasForeignKey(x => x.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull);

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

                e.Property(x => x.IdUsuario)
                    .HasColumnName("id_usuario");

                e.Property(x => x.FechaCambio)
                    .HasColumnName("fecha_cambio")
                    .HasDefaultValueSql("GETDATE()");

                e.HasOne(x => x.Usuario)
                    .WithMany(u => u.HistorialClientes)
                    .HasForeignKey(x => x.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull);

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
                e.Property(x => x.IdUsuario).HasColumnName("id_usuario");
                e.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("GETDATE()");
                e.Property(x => x.FechaActualizacion).HasColumnName("fecha_actualizacion");

                e.HasOne(x => x.Usuario)
                    .WithMany(u => u.Notas)
                    .HasForeignKey(x => x.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull);

                e.HasOne(x => x.OrdenTrabajo)
                    .WithMany(o => o.Notas)
                    .HasForeignKey(x => x.IdOrden)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // =========================================================
            // COTIZACIONES (HU-COT-001 a HU-COT-004)
            // =========================================================
            modelBuilder.Entity<Cotizacion>(e =>
            {
                e.ToTable("Cotizaciones");
                e.HasKey(x => x.IdCotizacion);

                e.Property(x => x.IdCotizacion).HasColumnName("id_cotizacion");

                e.Property(x => x.NumeroCotizacion)
                    .HasColumnName("numero_cotizacion")
                    .IsRequired()
                    .HasMaxLength(20);

                // El correlativo no se puede repetir: es la referencia que el
                // cliente ve en el correo.
                e.HasIndex(x => x.NumeroCotizacion).IsUnique();

                e.Property(x => x.Fecha).HasColumnName("fecha");

                // --- Cliente ---
                e.Property(x => x.IdCliente).HasColumnName("id_cliente");
                e.Property(x => x.NombreCliente).HasColumnName("nombre_cliente").IsRequired().HasMaxLength(200);
                e.Property(x => x.Compania).HasColumnName("compania").HasMaxLength(200);
                e.Property(x => x.Contacto).HasColumnName("contacto").HasMaxLength(200);
                e.Property(x => x.CorreoCliente).HasColumnName("correo_cliente").HasMaxLength(200);
                e.Property(x => x.TelefonoCelular).HasColumnName("telefono_celular").HasMaxLength(30);

                // --- Servicio ---
                e.Property(x => x.TipoServicio).HasColumnName("tipo_servicio").HasMaxLength(50);
                e.Property(x => x.Origen).HasColumnName("origen").HasMaxLength(200);
                e.Property(x => x.Destino).HasColumnName("destino").HasMaxLength(200);
                e.Property(x => x.VolumenM3).HasColumnName("volumen_m3").HasPrecision(10, 2);
                e.Property(x => x.TipoContenedor).HasColumnName("tipo_contenedor").HasMaxLength(30);
                e.Property(x => x.CompaniaMaritima).HasColumnName("compania_maritima").HasMaxLength(100);
                e.Property(x => x.Corresponsal).HasColumnName("corresponsal").HasMaxLength(100);

                // --- Cronograma ---
                e.Property(x => x.DiasEmpaque).HasColumnName("dias_empaque");
                e.Property(x => x.DiasTransito).HasColumnName("dias_transito");
                e.Property(x => x.DiasDesalmacenaje).HasColumnName("dias_desalmacenaje");
                e.Property(x => x.DiasFrecuenciaSalidas).HasColumnName("dias_frecuencia_salidas");

                // --- Rubros de costo (HU-COT-001) ---
                e.Property(x => x.CostoOrigen).HasColumnName("costo_origen").HasPrecision(18, 2);
                e.Property(x => x.CostoTramitesAduana).HasColumnName("costo_tramites_aduana").HasPrecision(18, 2);
                e.Property(x => x.CostoFlete).HasColumnName("costo_flete").HasPrecision(18, 2);
                e.Property(x => x.CostoDestino).HasColumnName("costo_destino").HasPrecision(18, 2);

                // --- Seguro ---
                e.Property(x => x.IncluyeSeguro).HasColumnName("incluye_seguro").HasDefaultValue(false);
                e.Property(x => x.ValorDeclarado).HasColumnName("valor_declarado").HasPrecision(18, 2);
                e.Property(x => x.PorcentajeSeguro).HasColumnName("porcentaje_seguro").HasPrecision(5, 2);

                // --- Totales ---
                e.Property(x => x.Subtotal).HasColumnName("subtotal").HasPrecision(18, 2);
                e.Property(x => x.MontoSeguro).HasColumnName("monto_seguro").HasPrecision(18, 2);
                e.Property(x => x.TarifaTotal).HasColumnName("tarifa_total").HasPrecision(18, 2);
                e.Property(x => x.Moneda).HasColumnName("moneda").HasMaxLength(3);

                // --- Condiciones comerciales ---
                e.Property(x => x.VigenciaDias).HasColumnName("vigencia_dias").HasDefaultValue(60);
                e.Property(x => x.FormaPago).HasColumnName("forma_pago").HasMaxLength(200);
                e.Property(x => x.Exclusiones).HasColumnName("exclusiones");
                e.Property(x => x.Observaciones).HasColumnName("observaciones");

                // --- Estado y trazabilidad ---
                e.Property(x => x.Estado).HasColumnName("estado").HasMaxLength(20);
                e.Property(x => x.FechaEnvio).HasColumnName("fecha_envio");
                e.Property(x => x.CorreoEnvio).HasColumnName("correo_envio").HasMaxLength(200);
                e.Property(x => x.IdOrdenGenerada).HasColumnName("id_orden_generada");
                e.Property(x => x.HechoPor).HasColumnName("hecho_por").HasMaxLength(100);
                e.Property(x => x.IdUsuario).HasColumnName("id_usuario");
                e.Property(x => x.FechaCreacion).HasColumnName("fecha_creacion").HasDefaultValueSql("GETDATE()");
                e.Property(x => x.FechaActualizacion).HasColumnName("fecha_actualizacion");

                // Borrar un cliente no debe arrastrarse las cotizaciones.
                e.HasOne(x => x.Cliente)
                    .WithMany(c => c.Cotizaciones)
                    .HasForeignKey(x => x.IdCliente)
                    .OnDelete(DeleteBehavior.Restrict);

                // La orden generada (HU-COT-004) queda ligada a la cotización
                // que le dio origen.
                e.HasOne(x => x.OrdenGenerada)
                    .WithMany()
                    .HasForeignKey(x => x.IdOrdenGenerada)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.Usuario)
                    .WithMany()
                    .HasForeignKey(x => x.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull);
            });
            // =========================================================
            // Inventario 
            // =========================================================
            modelBuilder.Entity<Inventario>(entity =>
            {
                entity.ToTable("Inventario");

                entity.HasKey(e => e.IdMaterial);

                entity.Property(e => e.IdMaterial)
                    .HasColumnName("id_material");

                entity.Property(e => e.NombreMaterial)
                    .HasColumnName("nombre_material")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion")
                    .HasMaxLength(250);

                entity.Property(e => e.Categoria)
                    .HasColumnName("categoria")
                    .HasMaxLength(50);

                entity.Property(e => e.Unidad)
                    .HasColumnName("unidad")
                    .HasMaxLength(30);

                entity.Property(e => e.Existencias)
                    .HasColumnName("existencias");

                entity.Property(e => e.StockMinimo)
                    .HasColumnName("stock_minimo");

                entity.Property(e => e.FechaCreacion)
                    .HasColumnName("fecha_creacion");

                entity.Property(e => e.FechaActualizacion)
                    .HasColumnName("fecha_actualizacion");
            });
            // =========================================================
            // OrdenTrabajoMaterial
            // =========================================================
            modelBuilder.Entity<OrdenTrabajoMaterial>(entity =>
            {
                entity.ToTable("OrdenTrabajoMaterial");

                entity.HasKey(e => e.IdOrdenMaterial);

                entity.Property(e => e.IdOrdenMaterial)
                    .HasColumnName("id_orden_material");

                entity.Property(e => e.IdOrden)
                    .HasColumnName("id_orden");

                entity.Property(e => e.IdMaterial)
                    .HasColumnName("id_material");

                entity.Property(e => e.Cantidad)
                    .HasColumnName("cantidad");

                entity.Property(e => e.FechaAsignacion)
                    .HasColumnName("fecha_asignacion");

                entity.HasOne(e => e.OrdenTrabajo)
                    .WithMany(o => o.MaterialesAsignados)
                    .HasForeignKey(e => e.IdOrden);

                entity.HasOne(e => e.Material)
                    .WithMany(i => i.OrdenesMaterial)
                    .HasForeignKey(e => e.IdMaterial);
            });
            // =========================================================
            // BienesMudanza (HU-INV-003)
            // =========================================================
            modelBuilder.Entity<BienMudanza>(entity =>
            {
                entity.ToTable("BienesMudanza");

                entity.HasKey(e => e.IdBien);

                entity.Property(e => e.IdBien)
                    .HasColumnName("id_bien");

                entity.Property(e => e.IdOrden)
                    .HasColumnName("id_orden");

                entity.Property(e => e.NombreBien)
                    .HasColumnName("nombre_bien")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.Descripcion)
                    .HasColumnName("descripcion")
                    .HasMaxLength(250);

                entity.Property(e => e.Cantidad)
                    .HasColumnName("cantidad");

                entity.Property(e => e.Condicion)
                    .HasColumnName("condicion")
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(e => e.Observaciones)
                    .HasColumnName("observaciones")
                    .HasMaxLength(500);

                entity.Property(e => e.FechaRegistro)
                    .HasColumnName("fecha_registro");

                // Escenario 2: un mismo bien no puede repetirse dentro de la
                // orden. El índice único respalda la validación del controlador
                // para que una carrera entre dos peticiones tampoco la burle.
                entity.HasIndex(e => new { e.IdOrden, e.NombreBien })
                    .IsUnique()
                    .HasDatabaseName("UX_BienesMudanza_Orden_Nombre");

                entity.HasOne(e => e.OrdenTrabajo)
                    .WithMany(o => o.Bienes)
                    .HasForeignKey(e => e.IdOrden)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}