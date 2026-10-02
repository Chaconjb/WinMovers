using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{   
    public class OrdenTrabajoMaterial
    {
        [Key]
        public int IdOrdenMaterial { get; set; }

        public int IdOrden { get; set; }

        public int IdMaterial { get; set; }

        public int Cantidad { get; set; }

        public DateTime FechaAsignacion { get; set; }

        public OrdenTrabajo OrdenTrabajo { get; set; } = null!;

        public Inventario Material { get; set; } = null!;
    }
}
