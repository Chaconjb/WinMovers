using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class CrearUsuarioViewModel
    {
        [Required(ErrorMessage = "El nombre completo es requerido")]
        [StringLength(200)]
        [Display(Name = "Nombre completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Selecciona un rol")]
        [Display(Name = "Rol")]
        public string NombreRol { get; set; } = string.Empty;

        public List<string> RolesDisponibles { get; set; } = new();
    }
}