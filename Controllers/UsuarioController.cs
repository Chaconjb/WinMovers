using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    [Authorize(Roles = Roles.Administrador)]
    public class UsuarioController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsuarioController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Usuario/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var modelo = new CrearUsuarioViewModel
            {
                RolesDisponibles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync()
            };
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearUsuarioViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                modelo.RolesDisponibles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
                return View(modelo);
            }

            var existente = await _userManager.FindByEmailAsync(modelo.Correo);
            if (existente != null)
            {
                ModelState.AddModelError(nameof(modelo.Correo), "Ya existe un usuario con ese correo.");
                modelo.RolesDisponibles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
                return View(modelo);
            }

            // Genera una contraseña temporal simple pero que cumple la política
            // (mayúscula, minúscula, número). El usuario deberá cambiarla al
            // iniciar sesión (HU-AUT-002 Escenario 3).
            var contrasenaTemporal = $"Temp{Guid.NewGuid().ToString("N").Substring(0, 8)}!";

            var usuario = new ApplicationUser
            {
                UserName = modelo.Correo,
                Email = modelo.Correo,
                NombreCompleto = modelo.NombreCompleto,
                EmailConfirmed = true,
                Activo = true,
                DebeCambiarContrasena = true
            };

            var resultado = await _userManager.CreateAsync(usuario, contrasenaTemporal);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                modelo.RolesDisponibles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
                return View(modelo);
            }

            await _userManager.AddToRoleAsync(usuario, modelo.NombreRol);

            TempData["Success"] = $"Usuario creado. Contraseña temporal: {contrasenaTemporal}";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Usuario
        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users
                .OrderBy(u => u.NombreCompleto)
                .ToListAsync();

            // Cargamos el rol de cada usuario para mostrarlo en la lista.
            var lista = new List<(ApplicationUser Usuario, string Rol)>();
            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                lista.Add((usuario, roles.FirstOrDefault() ?? "Sin rol"));
            }

            return View(lista);
        }

        // POST: /Usuario/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario == null) return NotFound();

            // Protección: nadie puede eliminarse a sí mismo, para no quedar
            // bloqueado del sistema por accidente.
            var idActual = _userManager.GetUserId(User);
            if (idActual != null && int.Parse(idActual) == usuario.Id)
            {
                TempData["Error"] = "No puedes eliminar tu propia cuenta.";
                return RedirectToAction(nameof(Index));
            }

            var resultado = await _userManager.DeleteAsync(usuario);

            if (!resultado.Succeeded)
            {
                TempData["Error"] = "No se pudo eliminar el usuario. Es posible que tenga registros de auditoría asociados como responsable de cambios de roles.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = $"Usuario '{usuario.NombreCompleto}' eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}