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

        [Required(ErrorMessage = "La referencia es obligatoria")]
        [StringLength(100)]
        public string Referencia { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; }

        [Range(0, int.MaxValue)]
        [Column("cajas")]
        public int Cajas { get; set; }

        [Range(0, double.MaxValue)]
        [Column("kilos", TypeName = "decimal(18,2)")]
        public decimal Kilos { get; set; }

        public string? Observaciones { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }

        public ICollection<ExportacionDocumento> Documentos { get; set; }
            = new List<ExportacionDocumento>();

        public ICollection<ExportacionArchivo> Archivos { get; set; }
            = new List<ExportacionArchivo>();

        // 🔥 FIX CRÍTICO
        [Column("id_orden")]
        public int? IdOrden { get; set; }

        [ForeignKey("IdOrden")]
        public OrdenTrabajo? OrdenTrabajo { get; set; }
    }
}