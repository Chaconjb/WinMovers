namespace WinMovers.Models
{
    public class ImportacionArchivo
    {
        public int IdArchivo { get; set; }
        public int IdImportacion { get; set; }
        public string NombreOriginal { get; set; } = string.Empty;
        public string NombreGuardado { get; set; } = string.Empty;
        public string TipoMime { get; set; } = string.Empty;
        public long TamanioBytes { get; set; }
        public DateTime FechaCarga { get; set; }

        // Navegación
        public Importacion? Importacion { get; set; }
    }
}