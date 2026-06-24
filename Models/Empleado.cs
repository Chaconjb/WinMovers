namespace AvanceWinMovers.Models;

public class Empleado
{
    public int id_empleado { get; set; }
    public string nombre_completo { get; set; } = string.Empty;
    public string puesto { get; set; } = string.Empty;
    public string? telefono { get; set; }
    public string? correo { get; set; }
    public DateTime? fecha_contratacion { get; set; }
    public bool activo { get; set; } = true;
    public DateTime fecha_creacion { get; set; }
    public DateTime? fecha_actualizacion { get; set; }
}