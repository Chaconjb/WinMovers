using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class ImportacionDocumento
    {
        public int IdImpDoc { get; set; }

        public int IdImportacion { get; set; }

        [Display(Name = "Tipo Documento")]
        public int? IdTipoDocumento { get; set; } /// cambiado
        [Display(Name = "Tipo Checklist")]
        [StringLength(100)]
        public string? TipoChecklist { get; set; }

        [Display(Name = "Completado")]
        public bool Completado { get; set; }

        [Display(Name = "Fecha Completado")]
        public DateTime? FechaCompletado { get; set; }

        public string? Observaciones { get; set; }

        public Importacion? Importacion { get; set; }
    }
}