namespace AvanceWinMovers.Models;

public class ControlVisita
{
    public int id_visita { get; set; }
    public string nombre_cliente { get; set; } = string.Empty;
    public string? documento_cliente { get; set; }
    public DateTime fecha_visita { get; set; }
    public string? motivo { get; set; }
    public string? resultado { get; set; }
    public string? observaciones { get; set; }
    public DateTime? fecha_registro { get; set; }
}
