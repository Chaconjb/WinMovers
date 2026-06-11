namespace WinMovers.Models
{
    public class ExportacionArchivo
    {
        public int IdArchivo { get; set; }
        public int IdExportacion { get; set; }
        public string NombreOriginal { get; set; } = string.Empty;
        public string NombreGuardado { get; set; } = string.Empty;
        public string TipoMime { get; set; } = string.Empty;
        public long TamanioBytes { get; set; }
        public DateTime FechaCarga { get; set; }

        public Exportacion? Exportacion { get; set; }
    }
}