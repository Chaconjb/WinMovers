using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WinMovers.Data;
using WinMovers.Models;
using WinMovers.Services;
using System.Text.Encodings.Web;

namespace WinMovers.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WinMoversContext _context;
        private readonly IEmailSender _emailSender;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            WinMoversContext context,
            IEmailSender emailSender)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
            _emailSender = emailSender;
        }

        // =====================================================
        // HU-AUT-001: Inicio de sesión
        // =====================================================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var usuario = await _userManager.FindByEmailAsync(modelo.Correo);

            // Escenario 2: credenciales inválidas - nunca revelamos si el correo
            // existe o no, siempre el mismo mensaje genérico.
            if (usuario == null || !usuario.Activo)
            {
                await RegistrarAuditoriaAsync(null, modelo.Correo, exitoso: false, motivo: "UsuarioNoExisteOInactivo");
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                return View(modelo);
            }

            var resultado = await _signInManager.PasswordSignInAsync(
                usuario.UserName!,
                modelo.Contrasena,
                modelo.RecordarMe,
                lockoutOnFailure: true);

            if (resultado.Succeeded)
            {
                await _signInManager.SignInWithClaimsAsync(usuario, modelo.RecordarMe,
                    new[] { new System.Security.Claims.Claim("NombreCompleto", usuario.NombreCompleto) });

                await RegistrarAuditoriaAsync(usuario.Id, modelo.Correo, exitoso: true, motivo: "Exitoso");

                // Escenario 3 de HU-AUT-002: contraseña temporal -> forzar cambio.
                if (usuario.DebeCambiarContrasena)
                {
                    return RedirectToAction(nameof(CambiarContrasenaTemporal));
                }

                if (!string.IsNullOrEmpty(modelo.ReturnUrl) && Url.IsLocalUrl(modelo.ReturnUrl))
                    return Redirect(modelo.ReturnUrl);

                return RedirectToAction("Index", "Dashboards");
            }

            if (resultado.IsLockedOut)
            {
                // Escenario 3: bloqueo temporal por múltiples intentos.
                await RegistrarAuditoriaAsync(usuario.Id, modelo.Correo, exitoso: false, motivo: "CuentaBloqueada");
                ModelState.AddModelError(string.Empty,
                    "Tu cuenta ha sido bloqueada temporalmente por múltiples intentos fallidos. Intenta de nuevo más tarde.");
                return View(modelo);
            }

            // Escenario 2: credenciales inválidas, mensaje genérico.
            await RegistrarAuditoriaAsync(usuario.Id, modelo.Correo, exitoso: false, motivo: "CredencialesInvalidas");
            ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

        private async Task RegistrarAuditoriaAsync(int? idUsuario, string correoIntentado, bool exitoso, string motivo)
        {
            _context.AccesosAuditoria.Add(new AccesoAuditoria
            {
                IdUsuario = idUsuario,
                CorreoIntentado = correoIntentado,
                Exitoso = exitoso,
                Motivo = motivo,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Fecha = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        // =====================================================
        // HU-AUT-002: Recuperación de contraseña
        // =====================================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var usuario = await _userManager.FindByEmailAsync(modelo.Correo);

            // Escenario 1 y 2 que muestran el MISMO mensaje genérico a propósito:
            // así no revelamos si el correo existe o no en el sistema.
            if (usuario != null && usuario.Activo)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                var tokenCodificado = HtmlEncoder.Default.Encode(token);
                var enlace = Url.Action(
                    nameof(ResetPassword), "Account",
                    new { correo = usuario.Email, token },
                    protocol: Request.Scheme);

                var cuerpo = $@"
                    <p>Hola {usuario.NombreCompleto},</p>
                    <p>Recibimos una solicitud para restablecer tu contraseña en WinMovers.</p>
                    <p><a href='{enlace}'>Haz clic aquí para crear una nueva contraseña</a></p>
                    <p>Si no solicitaste este cambio, ignora este correo.</p>
                    <p>Este enlace expira en 2 horas.</p>";

                await _emailSender.EnviarAsync(usuario.Email!, "Recuperación de contraseña - WinMovers", cuerpo);
            }

            TempData["Success"] = "Si el correo ingresado está registrado, te enviamos instrucciones para restablecer tu contraseña.";
            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation() => View();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string? correo, string? token)
        {
            if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(token))
                return RedirectToAction(nameof(Login));

            return View(new ResetPasswordViewModel { Correo = correo, Token = token });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            Console.WriteLine($"[DIAGNÓSTICO] Correo recibido en POST: '{modelo.Correo}'");

            var usuario = await _userManager.FindByEmailAsync(modelo.Correo);

            Console.WriteLine($"[DIAGNÓSTICO] ¿Usuario encontrado?: {usuario != null}");

            if (usuario == null)
            {
                // No revelamos si el correo existe; tratamos igual que token inválido/expirado.
                return RedirectToAction(nameof(ResetPasswordFallido));
            }

            var resultado = await _userManager.ResetPasswordAsync(usuario, modelo.Token, modelo.NuevaContrasena);

            if (!resultado.Succeeded)
            {
                // Si el token específicamente es inválido/expirado, mandamos a
                // la pantalla de "solicitar nueva recuperación" (Escenario 5).
                bool tokenInvalido = resultado.Errors.Any(e => e.Code == "InvalidToken");
                if (tokenInvalido)
                {
                    return RedirectToAction(nameof(ResetPasswordFallido));
                }

                // Cualquier otro error (ej. contraseña no cumple la política)
                // se muestra directamente en el mismo formulario, para que el
                // usuario corrija y lo intente de nuevo sin perder el token.
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }

            usuario.DebeCambiarContrasena = false;
            await _userManager.UpdateAsync(usuario);

            // Escenario 4: mensaje de éxito.
            TempData["Success"] = "La contraseña ha sido cambiada exitosamente.";
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation() => View();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordFallido() => View();

        // =====================================================
        // HU-AUT-002 Escenario 3: cambio obligatorio tras
        // iniciar sesión con contraseña temporal.
        // =====================================================

        [HttpGet]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult CambiarContrasenaTemporal() => View(new CambiarContrasenaTemporalViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> CambiarContrasenaTemporal(CambiarContrasenaTemporalViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return RedirectToAction(nameof(Login));

            var token = await _userManager.GeneratePasswordResetTokenAsync(usuario);
            var resultado = await _userManager.ResetPasswordAsync(usuario, token, modelo.NuevaContrasena);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(modelo);
            }

            usuario.DebeCambiarContrasena = false;
            await _userManager.UpdateAsync(usuario);

            TempData["Success"] = "La contraseña ha sido cambiada exitosamente.";
            return RedirectToAction("Index", "Dashboards");
        }
    }
}