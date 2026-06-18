using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinMovers.Models
{
    public class Cliente
    {
        [Key]
        [Column("id_cliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [Display(Name = "Nombre del Cliente")]
        [StringLength(200)]
        [Column("nombre_cliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [Display(Name = "Tel. Celular")]
        [StringLength(20)]
        [Column("telefono_celular")]
        public string? TelefonoCelular { get; set; }

        [Display(Name = "Tel. Residencia")]
        [StringLength(20)]
        [Column("telefono_residencia")]
        public string? TelefonoResidencia { get; set; }

        [Display(Name = "Tel. Empresa")]
        [StringLength(20)]
        [Column("telefono_empresa")]
        public string? TelefonoEmpresa { get; set; }

        [Display(Name = "Empresa")]
        [StringLength(200)]
        [Column("empresa")]
        public string? Empresa { get; set; }

        [Display(Name = "Contacto")]
        [StringLength(200)]
        [Column("contacto")]
        public string? Contacto { get; set; }

        [Display(Name = "Correo Electrónico")]
        [EmailAddress]
        [StringLength(200)]
        [Column("correo_electronico")]
        public string? CorreoElectronico { get; set; }

        [Required(ErrorMessage = "La dirección es requerida")]
        [Display(Name = "Dirección")]
        [StringLength(500)]
        [Column("direccion")]
        public string Direccion { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Display(Name = "Activo")]
        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Display(Name = "Fecha Registro")]
        [DataType(DataType.Date)]
        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Creación")]
        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column("fecha_actualizacion")]
        public DateTime? FechaActualizacion { get; set; }
    }
}