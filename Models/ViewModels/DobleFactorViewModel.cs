using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    public class DobleFactorViewModel
    {
        public string ClaveCompartida { get; set; } = string.Empty;

        public string UriAutenticador { get; set; } = string.Empty;

        public string ImagenCodigoQR { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar el código de verificación.")]
        [Display(Name = "Código de verificación")]
        public string CodigoVerificacion { get; set; } = string.Empty;
    }
}
