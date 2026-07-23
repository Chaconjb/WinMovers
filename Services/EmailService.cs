using WinMovers.Models;

namespace WinMovers.Services
{
    public interface ICotizacionEmailService
    {
        // Arma el machote en HTML sin enviarlo (sirve para la vista previa).
        Task<string> ConstruirCuerpoAsync(Cotizacion cotizacion, string? mensajeAdicional = null);

        // HU-COT-003: envía la cotización al cliente.
        Task EnviarCotizacionAsync(Cotizacion cotizacion, string destinatario, string? asunto, string? mensajeAdicional);
    }

    public class CotizacionEmailService : ICotizacionEmailService
    {
        private const string VistaMachote = "/Views/Emails/CotizacionTemplate.cshtml";

        private readonly IViewRenderService _viewRender;
        private readonly IEmailSender _emailSender;
        private readonly IQuoteService _quoteService;

        public CotizacionEmailService(
            IViewRenderService viewRender,
            IEmailSender emailSender,
            IQuoteService quoteService)
        {
            _viewRender = viewRender;
            _emailSender = emailSender;
            _quoteService = quoteService;
        }

        public async Task<string> ConstruirCuerpoAsync(Cotizacion cotizacion, string? mensajeAdicional = null)
        {
            var modelo = new CotizacionEmailViewModel
            {
                Cotizacion = cotizacion,
                // Se recalcula sobre los montos guardados, así el texto en
                // letras siempre concuerda con la cifra que se muestra.
                TarifaTotalEnLetras = _quoteService.MontoEnLetras(cotizacion.TarifaTotal),
                MensajeAdicional = mensajeAdicional,
                Emisor = new DatosEmisorCotizacion()
            };

            return await _viewRender.RenderizarAStringAsync(VistaMachote, modelo);
        }

        public async Task EnviarCotizacionAsync(
            Cotizacion cotizacion,
            string destinatario,
            string? asunto,
            string? mensajeAdicional)
        {
            var cuerpo = await ConstruirCuerpoAsync(cotizacion, mensajeAdicional);

            var asuntoFinal = string.IsNullOrWhiteSpace(asunto)
                ? $"Cotización {cotizacion.NumeroCotizacion} - WinMovers"
                : asunto;

            await _emailSender.EnviarAsync(destinatario, asuntoFinal, cuerpo);
        }
    }
}
