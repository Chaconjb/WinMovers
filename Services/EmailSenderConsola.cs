namespace WinMovers.Services
{
    // Implementación temporal para desarrollo: en vez de enviar un correo real,
    // imprime el contenido en la consola/log. Así podemos probar todo el flujo
    // de HU-AUT-002 sin depender de configurar SMTP todavía.
    public class EmailSenderConsola : IEmailSender
    {
        private readonly ILogger<EmailSenderConsola> _logger;

        public EmailSenderConsola(ILogger<EmailSenderConsola> logger)
        {
            _logger = logger;
        }

        public Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            _logger.LogWarning("=== CORREO SIMULADO (no se envió de verdad) ===");
            _logger.LogWarning("Para: {Destinatario}", destinatario);
            _logger.LogWarning("Asunto: {Asunto}", asunto);
            _logger.LogWarning("Cuerpo: {Cuerpo}", cuerpoHtml);
            _logger.LogWarning("================================================");

            return Task.CompletedTask;
        }
    }
}