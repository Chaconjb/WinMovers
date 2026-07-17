using System.ComponentModel.DataAnnotations;

namespace WinMovers.Models
{
    // Resultado del cálculo de HU-COT-001. Lo devuelve QuoteService y lo
    // consumen tanto el controlador como el endpoint AJAX de la calculadora.
    public class ResultadoCalculoCotizacion
    {
        public decimal Subtotal { get; set; }
        public decimal MontoSeguro { get; set; }
        public decimal TarifaTotal { get; set; }

        // "doce mil setecientos setenta y cinco con 00/100" — el machote
        // presenta la tarifa en números y en letras.
        public string TarifaTotalEnLetras { get; set; } = string.Empty;
    }

    // Datos que el machote necesita y que no viven en la entidad porque son
    // de la empresa, no de la cotización.
    public class DatosEmisorCotizacion
    {
        public string Empresa { get; set; } = "WinMovers";
        public string Firmante { get; set; } = "Edwin Obando";
        public string Puesto { get; set; } = "Presidente";
        public string Telefono { get; set; } = "(506) 2215-3536";
        public string Correo { get; set; } = "sales@winmovers.com";
    }

    // Modelo que se le pasa a Views/Emails/CotizacionTemplate.cshtml.
    public class CotizacionEmailViewModel
    {
        public Cotizacion Cotizacion { get; set; } = new();
        public string TarifaTotalEnLetras { get; set; } = string.Empty;
        public string? MensajeAdicional { get; set; }
        public DatosEmisorCotizacion Emisor { get; set; } = new();
    }

    // Modelo del formulario de envío (HU-COT-003).
    public class EnviarCotizacionViewModel
    {
        public int IdCotizacion { get; set; }

        public string NumeroCotizacion { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo del destinatario es requerido")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [Display(Name = "Enviar a")]
        public string Destinatario { get; set; } = string.Empty;

        [Display(Name = "Asunto")]
        [StringLength(200)]
        public string? Asunto { get; set; }

        [Display(Name = "Mensaje adicional (opcional)")]
        public string? MensajeAdicional { get; set; }
    }

    // Filtros del listado (HU-COT-002).
    public class CotizacionBusquedaViewModel
    {
        [Display(Name = "Buscar")]
        public string? Termino { get; set; }

        [Display(Name = "Estado")]
        public string? Estado { get; set; }

        public IEnumerable<Cotizacion> Resultados { get; set; } = new List<Cotizacion>();
    }
}
