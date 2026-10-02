using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WinMovers.Data;
using WinMovers.Models;
using WinMovers.Services;
using System.Text.Encodings.Web;
using QRCoder;
using System.Text;

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


            if (usuario == null || !usuario.Activo)
            {
                await RegistrarAuditoriaAsync(null, modelo.Correo, exitoso: false, motivo: "UsuarioNoExisteOInactivo");
                ModelState.AddModelError(string.Empty, "Correo o contraseña incorrectos.");
                return View(modelo);
            }
            var resultado = await _signInManager.PasswordSignInAsync(
                usuario.UserName!,
                modelo.Contrasena,
                isPersistent: false,
                lockoutOnFailure: true);

            //if (resultado.RequiresTwoFactor)
            //{
            //    TempData["RecordarMe"] = modelo.RecordarMe;
            //   return RedirectToAction(nameof(Verificar2FA));
            //}

            if (resultado.RequiresTwoFactor)
            {
                Console.WriteLine("ENTRO AL FLUJO 2FA");

                TempData["RecordarMe"] = modelo.RecordarMe;

                return RedirectToAction(nameof(Verificar2FA));
            }

            if (resultado.Succeeded)
            {
                await RegistrarAuditoriaAsync(usuario.Id, modelo.Correo, true, "Exitoso");

                if (usuario.DebeCambiarContrasena)
                    return RedirectToAction(nameof(CambiarContrasenaTemporal));

                if (!string.IsNullOrEmpty(modelo.ReturnUrl) &&
                    Url.IsLocalUrl(modelo.ReturnUrl))
                    return Redirect(modelo.ReturnUrl);

                return RedirectToAction("Index", "Dashboards");
            }

            if (resultado.IsLockedOut)
            {
                await RegistrarAuditoriaAsync(usuario.Id, modelo.Correo, exitoso: false, motivo: "CuentaBloqueada");
                ModelState.AddModelError(string.Empty,
                    "Tu cuenta ha sido bloqueada temporalmente por múltiples intentos fallidos. Intenta de nuevo más tarde.");
                return View(modelo);
            }

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


        // Registro de nuevos usuarios


        [HttpGet]
        [AllowAnonymous]
        public IActionResult CrearCuenta()
        {
            return View(new CrearCuentaViewModel());
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCuenta(CrearCuentaViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }


            var usuarioExistente = await _userManager.FindByEmailAsync(modelo.Correo);

            if (usuarioExistente != null)
            {
                ModelState.AddModelError(
                    "Correo",
                    "Ya existe una cuenta registrada con este correo."
                );

                return View(modelo);
            }


            var usuario = new ApplicationUser
            {
                UserName = modelo.Correo,
                Email = modelo.Correo,
                NombreCompleto = modelo.NombreCompleto,
                Activo = true,
                EmailConfirmed = false
            };


            var resultado = await _userManager.CreateAsync(
                usuario,
                modelo.Contrasena
            );


            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description
                    );
                }

                return View(modelo);
            }


            await _userManager.AddToRoleAsync(
                usuario,
                Roles.SinRol
            );


            TempData["Success"] = "Cuenta creada correctamente. Ahora puede iniciar sesión.";


            return RedirectToAction(nameof(Login));
        }


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
                return RedirectToAction(nameof(ResetPasswordFallido));
            }

            var resultado = await _userManager.ResetPasswordAsync(usuario, modelo.Token, modelo.NuevaContrasena);

            if (!resultado.Succeeded)
            {

                bool tokenInvalido = resultado.Errors.Any(e => e.Code == "InvalidToken");
                if (tokenInvalido)
                {
                    return RedirectToAction(nameof(ResetPasswordFallido));
                }

                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(modelo);
            }

            usuario.DebeCambiarContrasena = false;
            await _userManager.UpdateAsync(usuario);

            TempData["Success"] = "La contraseña ha sido cambiada exitosamente.";
            return RedirectToAction(nameof(ResetPasswordConfirmation));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation() => View();

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordFallido() => View();


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
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Configurar2FA()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
                return RedirectToAction(nameof(Login));


            var key = await _userManager.GetAuthenticatorKeyAsync(usuario);


            if (string.IsNullOrEmpty(key))
            {
                await _userManager.ResetAuthenticatorKeyAsync(usuario);

                key = await _userManager.GetAuthenticatorKeyAsync(usuario);
            }


            var email = await _userManager.GetEmailAsync(usuario);


            var uri = $"otpauth://totp/WinMovers:{email}?secret={key}&issuer=WinMovers";


            using var qrGenerator = new QRCodeGenerator();

            using var qrCodeData = qrGenerator.CreateQrCode(
                uri,
                QRCodeGenerator.ECCLevel.Q
            );


            using var qrCode = new PngByteQRCode(qrCodeData);


            var imagen = qrCode.GetGraphic(20);


            ViewBag.QRCode =
                "data:image/png;base64," +
                Convert.ToBase64String(imagen);


            ViewBag.Key = key;


            return View("~/Views/DobleFactor/Configurar.cshtml");
        }
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verificar2FA(Verificar2FAViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var recordar = TempData["RecordarMe"] != null &&
                           Convert.ToBoolean(TempData["RecordarMe"]);

            var resultado = await _signInManager.TwoFactorAuthenticatorSignInAsync(
                modelo.Codigo.Replace(" ", "").Replace("-", ""),
                recordar,
                rememberClient: false);

            if (resultado.Succeeded)
            {
                return RedirectToAction("Index", "Dashboards");
            }

            if (resultado.IsLockedOut)
            {
                ModelState.AddModelError("", "Cuenta bloqueada.");
                return View(modelo);
            }

            ModelState.AddModelError("", "Código incorrecto.");
            return View(modelo);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Verificar2FA()
        {
            return View(new Verificar2FAViewModel());
        }

    
    [HttpPost]
        [Authorize]
        public async Task<IActionResult> Activar2FA(string codigo)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
                return RedirectToAction(nameof(Login));


            var valido = await _userManager.VerifyTwoFactorTokenAsync(
                usuario,
                TokenOptions.DefaultAuthenticatorProvider,
                codigo
            );


            if (valido)
            {
                usuario.TwoFactorEnabled = true;

                await _userManager.UpdateAsync(usuario);


                TempData["Success"] =
                "Autenticador activado correctamente";


                return RedirectToAction(nameof(Login));
            }


            ModelState.AddModelError(
                "",
                "Código incorrecto"
            );


            return View("Configurar2FA");
        }
    }
}