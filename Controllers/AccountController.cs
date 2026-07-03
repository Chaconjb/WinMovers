using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WinMoversContext _context;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            WinMoversContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        // =====================================================
        // HU-AUT-001: Inicio de sesión
        // =====================================================
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
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
    }
}