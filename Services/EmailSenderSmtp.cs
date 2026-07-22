using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace WinMovers.Services
{
    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Contrasena { get; set; } = string.Empty;
        public string CorreoRemitente { get; set; } = string.Empty;
        public string NombreRemitente { get; set; } = "WinMovers";
        public bool UsarSsl { get; set; } = true;
    }

    // Implementación real de envío de correo vía SMTP (Por medio del Gmail winmovers.noreply).
    public class EmailSenderSmtp : IEmailSender
    {
        private readonly SmtpSettings _settings;
        private readonly ILogger<EmailSenderSmtp> _logger;

        public EmailSenderSmtp(IOptions<SmtpSettings> settings, ILogger<EmailSenderSmtp> logger)
        {
            _settings = settings.Value;
            _logger = logger;
        }

        public async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress(_settings.NombreRemitente, _settings.CorreoRemitente));
            mensaje.To.Add(MailboxAddress.Parse(destinatario));
            mensaje.Subject = asunto;
            mensaje.Body = new TextPart("html") { Text = cuerpoHtml };

            using var cliente = new SmtpClient();
            try
            {
                await cliente.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    _settings.UsarSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None);

                await cliente.AuthenticateAsync(_settings.Usuario, _settings.Contrasena);
                await cliente.SendAsync(mensaje);
                await cliente.DisconnectAsync(true);

                _logger.LogInformation("Correo enviado correctamente a {Destinatario}", destinatario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo a {Destinatario}", destinatario);
                throw;
            }
        }
    }
}