namespace WinMovers.Models
{
    public class OrdenTrabajoNota
    {
        public int IdNota { get; set; }
        public int IdOrden { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public string? Usuario { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }

        public OrdenTrabajo? OrdenTrabajo { get; set; }
    }
}