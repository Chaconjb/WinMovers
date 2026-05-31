using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WinMovers.Models
{
    public class ImportacionDocumento
    {
        public int IdImpDoc { get; set; }

        [Column("id_importacion")]
        public int IdImportacion { get; set; }

        [Display(Name = "Tipo Documento")]
        public int IdTipoDocumento { get; set; }
        [Display(Name = "Tipo Checklist")]
        [StringLength(20)]
        public string? TipoChecklist { get; set; }

        [Display(Name = "Completado")]
        public bool Completado { get; set; }

        [Display(Name = "Fecha Completado")]
        public DateTime? FechaCompletado { get; set; }

        public string? Observaciones { get; set; }

        public Importacion? Importacion { get; set; }

        public CatalogoDocumento? TipoDocumento { get; set; }
    }
}