using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class Verificar2FAViewModel
    {
        [Required(ErrorMessage = "Ingrese el código.")]
        [Display(Name = "Código autenticador")]
        public string Codigo { get; set; } = string.Empty;

        public bool RecordarMe { get; set; }
    }
}
