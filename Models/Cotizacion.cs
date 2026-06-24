namespace AvanceWinMovers.Models;

public class Cotizacion
{
    public int id_cotizacion { get; set; }
    public string numero_cotizacion { get; set; } = string.Empty;
    public string nombre_cliente { get; set; } = string.Empty;
    public string? origen { get; set; }
    public string? destino { get; set; }
    public decimal monto { get; set; }
    public string estado { get; set; } = "Pendiente";
    public DateTime? fecha_cotizacion { get; set; }
    public string? descripcion { get; set; }
    public DateTime fecha_creacion { get; set; }
    public DateTime? fecha_actualizacion { get; set; }
}