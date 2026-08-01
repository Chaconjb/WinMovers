using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class Inventario
    {
        [Key]
        public int IdMaterial { get; set; }

        [Required]
        [Display(Name = "Material")]
        [StringLength(100)]
        public string NombreMaterial { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required]
        public string Categoria { get; set; } = string.Empty;

        [Required]
        public string Unidad { get; set; } = "Unidad";

        [Required]
        [Range(0, int.MaxValue)]
        public int Existencias { get; set; }

        [Display(Name = "Stock mínimo")]
        public int StockMinimo { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaActualizacion { get; set; }

        public ICollection<OrdenTrabajoMaterial> MaterialesAsignados
            = new List<OrdenTrabajoMaterial>();

        public ICollection<OrdenTrabajoMaterial> OrdenesMaterial
        { get; set; } = new List<OrdenTrabajoMaterial>();
    }
}