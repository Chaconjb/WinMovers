using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class ControlVisita
    {
        public int IdVisita { get; set; }

        [Display(Name = "Fecha de Llamada")]
        [DataType(DataType.Date)]
        public DateTime? FechaLlamada { get; set; }

        [Display(Name = "Fecha de Visita")]
        [DataType(DataType.Date)]
        public DateTime? FechaVisita { get; set; }

        [StringLength(10)]
        public string? Hora { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [Display(Name = "Nombre del Cliente")]
        [StringLength(150)]
        public string NombreCliente { get; set; } = string.Empty;

        [Display(Name = "Tel. Habitación")]
        [StringLength(30)]
        public string? TelefonoHabitacion { get; set; }

        [Display(Name = "Tel. Celular")]
        [StringLength(30)]
        public string? TelefonoCelular { get; set; }

        [StringLength(150)]
        public string? Empresa { get; set; }

        [Display(Name = "Tel. Compañía")]
        [StringLength(30)]
        public string? TelefonoCompania { get; set; }

        [Display(Name = "Dirección de Origen")]
        [StringLength(500)]
        public string? DireccionOrigen { get; set; }

        [Display(Name = "Dirección de Destino")]
        [StringLength(500)]
        public string? DireccionDestino { get; set; }

        public string? Observaciones { get; set; }

        // Tipo de servicio (checkboxes múltiples)
        [Display(Name = "Puerta a Puerta")]
        public bool PuertaAPuerta { get; set; }

        [Display(Name = "Puerta a Puerto")]
        public bool PuertaAPuerto { get; set; }

        public bool Empaque { get; set; }

        [Display(Name = "Mudanza Local")]
        public bool MudanzaLocal { get; set; }

        // Sección cotización
        [StringLength(150)]
        public string? Origen { get; set; }

        [Display(Name = "Trámites de Aduana")]
        [StringLength(100)]
        public string? TramitesAduana { get; set; }

        [StringLength(100)]
        public string? Flete { get; set; }

        [StringLength(150)]
        public string? Destino { get; set; }

        [Display(Name = "Tarifa Total")]
        [StringLength(100)]
        public string? TarifaTotal { get; set; }

        [Display(Name = "Compañía Marítima")]
        [StringLength(100)]
        public string? CompaniaMaritima { get; set; }

        [StringLength(100)]
        public string? Corresponsal { get; set; }

        [Display(Name = "Hecho Por")]
        [StringLength(100)]
        public string? HechoPor { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
