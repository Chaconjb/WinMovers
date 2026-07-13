using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsuarioController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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
    }
}