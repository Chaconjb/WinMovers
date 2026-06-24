namespace AvanceWinMovers.Models;

public class InventarioItem
{
    public int id_item { get; set; }
    public string nombre_material { get; set; } = string.Empty;
    public string? descripcion { get; set; }
    public int cantidad { get; set; }
    public string? unidad { get; set; }
    public string estado { get; set; } = "Activo";
    public DateTime fecha_creacion { get; set; }
    public DateTime? fecha_actualizacion { get; set; }
}