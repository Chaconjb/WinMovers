using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    [Authorize(Roles = Roles.Administrador)]
    public class RolController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly WinMoversContext _context;

        public RolController(
            RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            WinMoversContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        // Obtiene el Id (int) del usuario autenticado actual, o null si no hay ninguna sesión activa.
        private int? ObtenerIdUsuarioActual()
        {
            var idTexto = _userManager.GetUserId(User);
            return idTexto != null ? int.Parse(idTexto) : null;
        }

        private async Task RegistrarAuditoriaAsync(string accion, string nombreRol, int? idUsuarioAfectado, string? detalle)
        {
            _context.RolesAuditoria.Add(new RolAuditoria
            {
                Accion = accion,
                NombreRol = nombreRol,
                IdUsuarioAfectado = idUsuarioAfectado,
                IdUsuarioResponsable = ObtenerIdUsuarioActual(),
                Detalle = detalle,
                Fecha = DateTime.Now
            });
            await _context.SaveChangesAsync();
        }

        // GET: /Rol
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        // =====================================================
        // HU-AUT-003 Escenario 1: Crear rol
        // =====================================================

        [HttpGet]
        public IActionResult Create() => View(new CrearRolViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CrearRolViewModel modelo)
        {
            if (!ModelState.IsValid)
                return View(modelo);

            var existe = await _roleManager.RoleExistsAsync(modelo.Nombre);
            if (existe)
            {
                ModelState.AddModelError(nameof(modelo.Nombre), "Ya existe un rol con ese nombre.");
                return View(modelo);
            }

            var rol = new ApplicationRole(modelo.Nombre) { Descripcion = modelo.Descripcion };
            var resultado = await _roleManager.CreateAsync(rol);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(modelo);
            }

            await RegistrarAuditoriaAsync("CrearRol", modelo.Nombre, null, $"Rol creado: {modelo.Nombre}");

            TempData["Success"] = $"Rol '{modelo.Nombre}' creado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // HU-AUT-003 Escenario 2: Asignar rol
        // =====================================================

        // GET: /Rol/AsignarRol/5  (5 = id del usuario)
        [HttpGet]
        public async Task<IActionResult> AsignarRol(int id)
        {
            var usuario = await _userManager.FindByIdAsync(id.ToString());
            if (usuario == null) return NotFound();

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            var todosLosRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();

            var modelo = new AsignarRolViewModel
            {
                IdUsuario = usuario.Id,
                NombreUsuarioMostrar = usuario.NombreCompleto,
                NombreRol = rolesActuales.FirstOrDefault() ?? string.Empty,
                RolesDisponibles = todosLosRoles
            };

            return View(modelo);
        }

        // POST: /Rol/AsignarRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarRol(AsignarRolViewModel modelo)
        {
            var usuario = await _userManager.FindByIdAsync(modelo.IdUsuario.ToString());
            if (usuario == null) return NotFound();

            if (!ModelState.IsValid)
            {
                modelo.NombreUsuarioMostrar = usuario.NombreCompleto;
                modelo.RolesDisponibles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
                return View(modelo);
            }

            // Quitamos los roles actuales del usuario y le asignamos el nuevo.
            // Este sistema maneja un rol activo a la vez por simplicidad.
            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            if (rolesActuales.Any())
            {
                await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            }

            var resultado = await _userManager.AddToRoleAsync(usuario, modelo.NombreRol);

            if (!resultado.Succeeded)
            {
                foreach (var error in resultado.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                modelo.NombreUsuarioMostrar = usuario.NombreCompleto;
                modelo.RolesDisponibles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
                return View(modelo);
            }

            await RegistrarAuditoriaAsync("AsignarRol", modelo.NombreRol, usuario.Id,
                $"Rol '{modelo.NombreRol}' asignado a {usuario.NombreCompleto}");

            TempData["Success"] = $"Rol asignado correctamente a {usuario.NombreCompleto}.";
            return RedirectToAction("Index", "Usuario");
        }

        // =====================================================
        // HU-AUT-003 Escenario 3: Eliminar rol
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var rol = await _roleManager.FindByIdAsync(id.ToString());
            if (rol == null) return NotFound();

            // Protección: los roles base del sistema nunca se pueden eliminar.
            if (rol.Name == Roles.Administrador || rol.Name == Roles.Empleado || rol.Name == Roles.SinRol)
            {
                TempData["Error"] = $"El rol '{rol.Name}' es un rol base del sistema y no puede eliminarse.";
                return RedirectToAction(nameof(Index));
            }

            // Buscamos todos los usuarios que tienen este rol, para moverlos
            // al rol por defecto ANTES de eliminar el rol.
            var usuariosConEsteRol = await _userManager.GetUsersInRoleAsync(rol.Name!);

            foreach (var usuario in usuariosConEsteRol)
            {
                await _userManager.RemoveFromRoleAsync(usuario, rol.Name!);
                await _userManager.AddToRoleAsync(usuario, Roles.SinRol);
            }

            var nombreRolEliminado = rol.Name!;
            var resultado = await _roleManager.DeleteAsync(rol);

            if (!resultado.Succeeded)
            {
                TempData["Error"] = "No se pudo eliminar el rol.";
                return RedirectToAction(nameof(Index));
            }

            await RegistrarAuditoriaAsync("EliminarRol", nombreRolEliminado, null,
                $"Rol '{nombreRolEliminado}' eliminado. {usuariosConEsteRol.Count} usuario(s) movido(s) a '{Roles.SinRol}'.");

            TempData["Success"] = $"Rol '{nombreRolEliminado}' eliminado. {usuariosConEsteRol.Count} usuario(s) movido(s) al rol '{Roles.SinRol}'.";
            return RedirectToAction(nameof(Index));
        }
    }
}