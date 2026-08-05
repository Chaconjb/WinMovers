using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    // HU-INV-003: bienes del cliente que se transportan en una mudanza.
    // No se confunde con Inventario, que es el material propio de WinMovers
    // (cajas, cinta) y sí maneja existencias; aquí sólo interesa la
    // trazabilidad de qué se llevó en cada orden y en qué condición.
    public class BienMudanza
    {
        [Key]
        public int IdBien { get; set; }

        public int IdOrden { get; set; }

        [Required(ErrorMessage = "El nombre del bien es requerido")]
        [Display(Name = "Bien")]
        [StringLength(150)]
        public string NombreBien { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        [StringLength(250)]
        public string? Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public int Cantidad { get; set; } = 1;

        [Required]
        [Display(Name = "Condición")]
        [StringLength(50)]
        public string Condicion { get; set; } = CondicionesBien.Bueno;

        [Display(Name = "Observaciones")]
        [StringLength(500)]
        public string? Observaciones { get; set; }

        [Display(Name = "Fecha de registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public OrdenTrabajo OrdenTrabajo { get; set; } = null!;
    }

    // Valores admitidos para BienMudanza.Condicion. Se dejan como constantes
    // en vez de enum porque la columna es NVARCHAR, igual que Estado en
    // OrdenTrabajo, y así el desplegable y la validación leen de un solo lugar.
    public static class CondicionesBien
    {
        public const string Bueno = "Bueno";
        public const string Regular = "Regular";
        public const string Danado = "Dañado";

        public static readonly string[] Todas = { Bueno, Regular, Danado };
    }
}
