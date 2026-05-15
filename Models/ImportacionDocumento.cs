using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class ImportacionDocumento
    {
        public int IdDocumento { get; set; }

        public int IdImportacion { get; set; }

        [Required]
        [Display(Name = "Documento")]
        [StringLength(200)]
        public string NombreDocumento { get; set; } = string.Empty;

        // "WinMovers" o "OtroAgente"
        [StringLength(20)]
        public string TipoAgente { get; set; } = "WinMovers";

        [Display(Name = "Completado")]
        public bool Completado { get; set; }

        public Importacion? Importacion { get; set; }
    }
}
