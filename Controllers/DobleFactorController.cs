using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WinMovers.Models;
using QRCoder;
using System.Text;

namespace WinMovers.Controllers
{
    [Authorize]
    public class DobleFactorController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DobleFactorController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Configurar()
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
                return RedirectToAction("Login", "Account");

            var clave = await _userManager.GetAuthenticatorKeyAsync(usuario);

            if (string.IsNullOrEmpty(clave))
            {
                await _userManager.ResetAuthenticatorKeyAsync(usuario);
                clave = await _userManager.GetAuthenticatorKeyAsync(usuario);
            }

            var uri = GenerarUriAutenticador(usuario.Email!, clave);

            var modelo = new DobleFactorViewModel
            {
                ClaveCompartida = clave,
                UriAutenticador = uri,
                ImagenCodigoQR = GenerarCodigoQR(uri)
            };

            return View(modelo);
        }
        [HttpPost]
        public async Task<IActionResult> Activar(DobleFactorViewModel modelo)
        {
            var usuario = await _userManager.GetUserAsync(User);

            if (usuario == null)
            {
                return RedirectToAction("Index", "Dashboards");
            }


            if (!ModelState.IsValid)
            {
                // Volver a cargar los datos del QR
                var clave = await _userManager.GetAuthenticatorKeyAsync(usuario);

                var uri = GenerarUriAutenticador(usuario.Email!, clave!);

                modelo.ClaveCompartida = clave!;
                modelo.UriAutenticador = uri;
                modelo.ImagenCodigoQR = GenerarCodigoQR(uri);

                return View("Configurar", modelo);
            }


            // Validar el código generado por la aplicación autenticadora
            var valido = await _userManager.VerifyTwoFactorTokenAsync(
                usuario,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                modelo.CodigoVerificacion
            );


            if (!valido)
            {
                ModelState.AddModelError(
                    "CodigoVerificacion",
                    "El código ingresado no es válido."
                );


                var clave = await _userManager.GetAuthenticatorKeyAsync(usuario);

                var uri = GenerarUriAutenticador(usuario.Email!, clave!);

                modelo.ClaveCompartida = clave!;
                modelo.UriAutenticador = uri;
                modelo.ImagenCodigoQR = GenerarCodigoQR(uri);


                return View("Configurar", modelo);
            }


            // Activar el doble factor
            var resultado = await _userManager.SetTwoFactorEnabledAsync(usuario, true);

            if (!resultado.Succeeded)
            {
                ModelState.AddModelError("", "No fue posible activar el doble factor.");
                return View("Configurar", modelo);
            }

            TempData["Mensaje"] = "Autenticación de dos pasos activada correctamente.";


            return RedirectToAction("Index", "Dashboards");
        }

        private string GenerarUriAutenticador(string correo, string clave)
        {
            return $"otpauth://totp/WinMovers:{correo}?secret={clave}&issuer=WinMovers&digits=6";
        }

        private string GenerarCodigoQR(string texto)
        {
            using var qrGenerator = new QRCodeGenerator();

            using var qrData = qrGenerator.CreateQrCode(texto, QRCodeGenerator.ECCLevel.Q);

            var pngQr = new PngByteQRCode(qrData);

            byte[] bytes = pngQr.GetGraphic(20);

            return $"data:image/png;base64,{Convert.ToBase64String(bytes)}";
        }
    }
}
