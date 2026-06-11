using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class CatalogoDocumento
    {
        public int IdTipoDocumento { get; set; }

        [Required]
        [StringLength(150)]
        public string Nombre { get; set; } = string.Empty;

        public bool AplicaExportacion { get; set; }
        public bool AplicaImportacion { get; set; }
        public bool AplicaWinMovers { get; set; }
        public bool AplicaOtroAgente { get; set; }

        public int OrdenPresentacion { get; set; }

        public bool Activo { get; set; }
    }
}