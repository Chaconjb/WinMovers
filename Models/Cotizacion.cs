using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinMovers.Models
{
    // Estados por los que pasa una cotización. El flujo normal es:
    // Borrador -> Enviada -> Aceptada -> Convertida (a orden de trabajo).
    public static class EstadosCotizacion
    {
        public const string Borrador = "Borrador";
        public const string Enviada = "Enviada";
        public const string Aceptada = "Aceptada";
        public const string Rechazada = "Rechazada";
        public const string Convertida = "Convertida";
    }

    public class Cotizacion
    {
        [Key]
        [Column("id_cotizacion")]
        public int IdCotizacion { get; set; }

        [Required(ErrorMessage = "El número de cotización es requerido")]
        [Display(Name = "Cotización #")]
        [StringLength(20)]
        [Column("numero_cotizacion")]
        public string NumeroCotizacion { get; set; } = string.Empty;

        [Display(Name = "Fecha")]
        [DataType(DataType.Date)]
        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.Today;

        // =========================================================
        // CLIENTE
        // =========================================================

        [Column("id_cliente")]
        public int? IdCliente { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public Cliente? Cliente { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [Display(Name = "Nombre del Cliente")]
        [StringLength(200)]
        [Column("nombre_cliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [Display(Name = "Compañía")]
        [StringLength(200)]
        [Column("compania")]
        public string? Compania { get; set; }

        [Display(Name = "Contacto")]
        [StringLength(200)]
        [Column("contacto")]
        public string? Contacto { get; set; }

        [Display(Name = "Correo del Cliente")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(200)]
        [Column("correo_cliente")]
        public string? CorreoCliente { get; set; }

        [Display(Name = "Tel. Celular")]
        [StringLength(30)]
        [Column("telefono_celular")]
        public string? TelefonoCelular { get; set; }

        // =========================================================
        // SERVICIO
        // =========================================================

        [Display(Name = "Tipo de Servicio")]
        [StringLength(50)]
        [Column("tipo_servicio")]
        public string TipoServicio { get; set; } = "Puerta a Puerta";

        [Display(Name = "Origen")]
        [StringLength(200)]
        [Column("origen")]
        public string? Origen { get; set; }

        [Display(Name = "Destino")]
        [StringLength(200)]
        [Column("destino")]
        public string? Destino { get; set; }

        [Display(Name = "Volumen (m³)")]
        [Range(0, 10000, ErrorMessage = "El volumen debe ser mayor o igual a 0")]
        [Column("volumen_m3")]
        public decimal? VolumenM3 { get; set; }

        [Display(Name = "Contenedor")]
        [StringLength(30)]
        [Column("tipo_contenedor")]
        public string? TipoContenedor { get; set; }

        [Display(Name = "Compañía Marítima")]
        [StringLength(100)]
        [Column("compania_maritima")]
        public string? CompaniaMaritima { get; set; }

        [Display(Name = "Corresponsal en Destino")]
        [StringLength(100)]
        [Column("corresponsal")]
        public string? Corresponsal { get; set; }

        // =========================================================
        // CRONOGRAMA (machote: "El empaque tendrá una duración de xx días...")
        // =========================================================

        [Display(Name = "Días de Empaque")]
        [Range(0, 365)]
        [Column("dias_empaque")]
        public int? DiasEmpaque { get; set; }

        [Display(Name = "Días de Tránsito")]
        [Range(0, 365)]
        [Column("dias_transito")]
        public int? DiasTransito { get; set; }

        [Display(Name = "Días de Desalmacenaje")]
        [Range(0, 365)]
        [Column("dias_desalmacenaje")]
        public int? DiasDesalmacenaje { get; set; }

        [Display(Name = "Frecuencia de Salidas (días)")]
        [Range(0, 365)]
        [Column("dias_frecuencia_salidas")]
        public int? DiasFrecuenciaSalidas { get; set; }

        // =========================================================
        // RUBROS DE COSTO (HU-COT-001)
        // Los cuatro rubros del machote. El asesor los digita y el
        // sistema calcula subtotal, seguro y tarifa total.
        // =========================================================

        [Display(Name = "Servicios en Origen (empaque)")]
        [Range(0, 9999999, ErrorMessage = "El monto no puede ser negativo")]
        [Column("costo_origen")]
        public decimal CostoOrigen { get; set; }

        [Display(Name = "Trámites de Aduana")]
        [Range(0, 9999999, ErrorMessage = "El monto no puede ser negativo")]
        [Column("costo_tramites_aduana")]
        public decimal CostoTramitesAduana { get; set; }

        [Display(Name = "Flete Marítimo Internacional")]
        [Range(0, 9999999, ErrorMessage = "El monto no puede ser negativo")]
        [Column("costo_flete")]
        public decimal CostoFlete { get; set; }

        [Display(Name = "Servicios en Destino")]
        [Range(0, 9999999, ErrorMessage = "El monto no puede ser negativo")]
        [Column("costo_destino")]
        public decimal CostoDestino { get; set; }

        // =========================================================
        // SEGURO (machote: "3.5% del valor declarado")
        // =========================================================

        [Display(Name = "Incluir Seguro")]
        [Column("incluye_seguro")]
        public bool IncluyeSeguro { get; set; }

        [Display(Name = "Valor Declarado")]
        [Range(0, 99999999, ErrorMessage = "El valor declarado no puede ser negativo")]
        [Column("valor_declarado")]
        public decimal? ValorDeclarado { get; set; }

        [Display(Name = "% de Seguro")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        [Column("porcentaje_seguro")]
        public decimal PorcentajeSeguro { get; set; } = 3.5m;

        // =========================================================
        // TOTALES CALCULADOS
        // Se persisten (no son [NotMapped]) para que la cotización enviada
        // conserve los montos con los que se le presentó al cliente, aunque
        // después cambien las reglas de cálculo.
        // =========================================================

        [Display(Name = "Subtotal")]
        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Display(Name = "Monto del Seguro")]
        [Column("monto_seguro")]
        public decimal MontoSeguro { get; set; }

        [Display(Name = "Tarifa Total")]
        [Column("tarifa_total")]
        public decimal TarifaTotal { get; set; }

        [Display(Name = "Moneda")]
        [StringLength(3)]
        [Column("moneda")]
        public string Moneda { get; set; } = "USD";

        // =========================================================
        // CONDICIONES COMERCIALES
        // =========================================================

        [Display(Name = "Vigencia (días)")]
        [Range(1, 365, ErrorMessage = "La vigencia debe estar entre 1 y 365 días")]
        [Column("vigencia_dias")]
        public int VigenciaDias { get; set; } = 60;

        [Display(Name = "Forma de Pago")]
        [StringLength(200)]
        [Column("forma_pago")]
        public string? FormaPago { get; set; } = "Trámite de factura";

        [Display(Name = "Nuestra tarifa no incluye")]
        [Column("exclusiones")]
        public string? Exclusiones { get; set; }

        [Display(Name = "Observaciones")]
        [Column("observaciones")]
        public string? Observaciones { get; set; }

        // =========================================================
        // ESTADO Y TRAZABILIDAD
        // =========================================================

        [Display(Name = "Estado")]
        [StringLength(20)]
        [Column("estado")]
        public string Estado { get; set; } = EstadosCotizacion.Borrador;

        [Display(Name = "Fecha de Envío")]
        [Column("fecha_envio")]
        public DateTime? FechaEnvio { get; set; }

        [Display(Name = "Enviada a")]
        [StringLength(200)]
        [Column("correo_envio")]
        public string? CorreoEnvio { get; set; }

        // Orden de trabajo generada al convertir la cotización (HU-COT-004).
        [Column("id_orden_generada")]
        public int? IdOrdenGenerada { get; set; }

        [ForeignKey(nameof(IdOrdenGenerada))]
        public OrdenTrabajo? OrdenGenerada { get; set; }

        [Display(Name = "Hecho Por")]
        [StringLength(100)]
        [Column("hecho_por")]
        public string? HechoPor { get; set; }

        [Column("id_usuario")]
        public int? IdUsuario { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public ApplicationUser? Usuario { get; set; }

        [Display(Name = "Fecha de Creación")]
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("fecha_actualizacion")]
        public DateTime? FechaActualizacion { get; set; }

        // =========================================================
        // PROPIEDADES DERIVADAS (no se guardan en BD)
        // =========================================================

        [NotMapped]
        [Display(Name = "Vence el")]
        public DateTime FechaVencimiento => Fecha.AddDays(VigenciaDias);

        [NotMapped]
        public bool EstaVencida =>
            Estado != EstadosCotizacion.Convertida &&
            Estado != EstadosCotizacion.Rechazada &&
            DateTime.Today > FechaVencimiento;

        // Una cotización ya convertida no se puede seguir editando: la orden
        // de trabajo ya salió con esos montos.
        [NotMapped]
        public bool SePuedeEditar => Estado != EstadosCotizacion.Convertida;

        [NotMapped]
        public bool SePuedeConvertir =>
            Estado != EstadosCotizacion.Convertida &&
            Estado != EstadosCotizacion.Rechazada;
    }
}
