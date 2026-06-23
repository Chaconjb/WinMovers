namespace WinMovers.Models
{
    public class OrdenTrabajoHistorial
    {
        public int IdHistorial { get; set; }
        public int IdOrden { get; set; }
        public string CampoModificado { get; set; } = string.Empty; // "fecha_servicio" o "estado"
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public string? Usuario { get; set; }
        public DateTime FechaCambio { get; set; }

        public OrdenTrabajo? OrdenTrabajo { get; set; }
    }
}