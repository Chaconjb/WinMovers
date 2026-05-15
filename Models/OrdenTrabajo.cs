using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class OrdenTrabajo
    {
        public int IdOrden { get; set; }

        [Required(ErrorMessage = "El número de O.T. es requerido")]
        [Display(Name = "O.T. #")]
        [StringLength(20)]
        public string NumeroOT { get; set; } = string.Empty;

        [Display(Name = "Fecha de Servicio")]
        [DataType(DataType.Date)]
        public DateTime? FechaServicio { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; }

        [StringLength(10)]
        public string? Hora { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [Display(Name = "Nombre del Cliente")]
        [StringLength(150)]
        public string NombreCliente { get; set; } = string.Empty;

        [Display(Name = "Tel. Celular")]
        [StringLength(30)]
        public string? TelefonoCelular { get; set; }

        [Display(Name = "Tel. Residencia")]
        [StringLength(30)]
        public string? TelefonoResidencia { get; set; }

        [Display(Name = "Compañía")]
        [StringLength(150)]
        public string? Compania { get; set; }

        [Display(Name = "Tel. Empresa")]
        [StringLength(30)]
        public string? TelefonoEmpresa { get; set; }

        [StringLength(150)]
        public string? Contacto { get; set; }

        [Display(Name = "Dirección de Origen")]
        [StringLength(500)]
        public string? DireccionOrigen { get; set; }

        [Display(Name = "Dirección de Destino")]
        [StringLength(500)]
        public string? DireccionDestino { get; set; }

        [Display(Name = "Detalle del Servicio")]
        public string? DetalleServicio { get; set; }

        [StringLength(500)]
        public string? Materiales { get; set; }

        [Display(Name = "Facturar a Nombre de")]
        [StringLength(150)]
        public string? FacturarA { get; set; }

        [Display(Name = "Dirección de Cobro")]
        [StringLength(500)]
        public string? DireccionCobro { get; set; }

        [Display(Name = "Hecho Por")]
        [StringLength(100)]
        public string? HechoPor { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }
    }
}
