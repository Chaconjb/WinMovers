namespace AvanceWinMovers.Models;

public class OrdenTrabajo
{
    public int id_orden { get; set; }
    public string numero_ot { get; set; } = string.Empty;
    public DateTime? fecha_servicio { get; set; }
    public DateTime? fecha { get; set; }
    public string? hora { get; set; }
    public string nombre_cliente { get; set; } = string.Empty;
    public string? telefono_celular { get; set; }
    public string? telefono_residencia { get; set; }
    public string? compania { get; set; }
    public string? telefono_empresa { get; set; }
    public string? contacto { get; set; }
    public string? direccion_origen { get; set; }
    public string? direccion_destino { get; set; }
    public string? detalle_servicio { get; set; }
    public string? materiales { get; set; }
    public string? facturar_a { get; set; }
    public string? direccion_cobro { get; set; }
    public string? hecho_por { get; set; }
    public DateTime fecha_creacion { get; set; }
    public DateTime? fecha_actualizacion { get; set; }
}
