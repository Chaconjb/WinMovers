namespace WinMovers.Models
{
    public class OrdenTrabajoArchivo
    {
        public int IdArchivo { get; set; }
        public int IdOrden { get; set; }
        public string NombreOriginal { get; set; } = string.Empty;
        public string NombreGuardado { get; set; } = string.Empty;
        public string TipoMime { get; set; } = string.Empty;
        public long TamanioBytes { get; set; }
        public DateTime FechaCarga { get; set; }

        public OrdenTrabajo? OrdenTrabajo { get; set; }
    }
}