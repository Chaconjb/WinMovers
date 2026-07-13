using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class CrearRolViewModel
    {
        [Required(ErrorMessage = "El nombre del rol es requerido")]
        [StringLength(256, MinimumLength = 2)]
        [Display(Name = "Nombre del rol")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(250)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
    }

    public class AsignarRolViewModel
    {
        [Required]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Selecciona un rol")]
        [Display(Name = "Rol")]
        public string NombreRol { get; set; } = string.Empty;

        // Solo para mostrar en pantalla, no se envía en el formulario.
        public string? NombreUsuarioMostrar { get; set; }
        public List<string> RolesDisponibles { get; set; } = new();
    }
}