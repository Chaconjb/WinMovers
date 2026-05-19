using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class ExportacionDocumento
    {
        public int IdExpDoc { get; set; }

        public int IdExportacion { get; set; }

        [Display(Name = "Tipo Documento")]
        public int IdTipoDocumento { get; set; }

        [Display(Name = "Tipo Checklist")]
        [StringLength(100)]
        public string? TipoChecklist { get; set; }

        [Display(Name = "Completado")]
        public bool Completado { get; set; }

        [Display(Name = "Fecha Completado")]
        public DateTime? FechaCompletado { get; set; }

        public string? Observaciones { get; set; }

        public Exportacion? Exportacion { get; set; }
    }
}