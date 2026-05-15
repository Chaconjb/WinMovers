using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class Importacion
    {
        public int IdImportacion { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [Display(Name = "Nombre del Cliente")]
        [StringLength(150)]
        public string NombreCliente { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Referencia { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Fecha { get; set; }

        public string? Observaciones { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<ImportacionDocumento> Documentos { get; set; } = new List<ImportacionDocumento>();
    }
}
