using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models.ViewModels
{
    // HU-INV-003. Se enlaza este modelo en vez de la entidad BienMudanza
    // porque su propiedad de navegación OrdenTrabajo es no anulable y el
    // binder la marcaría como requerida, invalidando siempre el formulario.
    public class RegistrarBienViewModel
    {
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

        [Required(ErrorMessage = "La condición es requerida")]
        [Display(Name = "Condición")]
        [StringLength(50)]
        public string Condicion { get; set; } = CondicionesBien.Bueno;

        [Display(Name = "Observaciones")]
        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
