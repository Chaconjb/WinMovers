using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinMovers.Models
{
    public class Exportacion
    {
        public int IdExportacion { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [Display(Name = "Nombre del Cliente")]
        [StringLength(150)]
        public string NombreCliente { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Referencia { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Las cajas no pueden tener un valor negativo")]
        [Column("cajas")]
        public int Cajas { get; set; }


        [Range(0, double.MaxValue, ErrorMessage = "Los kilos no pueden ser negativos")]
        [Column("kilos", TypeName = "decimal(18,2)")]
        public decimal Kilos { get; set; }

        public string? Observaciones { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Display(Name = "Fecha de Actualización")]
        public DateTime? FechaActualizacion { get; set; }

        public ICollection<ExportacionDocumento> Documentos { get; set; } = new List<ExportacionDocumento>();
        public ICollection<ExportacionArchivo> Archivos { get; set; } = new List<ExportacionArchivo>();
    }
}