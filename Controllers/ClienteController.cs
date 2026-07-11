using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class ClienteController : Controller
    {
        private readonly WinMoversContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClienteController(WinMoversContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Obtiene el Id (int) del usuario autenticado actual, o null si no hay sesión.
        private int? ObtenerIdUsuarioActual()
        {
            var idTexto = _userManager.GetUserId(User);
            return idTexto != null ? int.Parse(idTexto) : null;
        }

        // GET: /Cliente
        public async Task<IActionResult> Index(string? nombreCliente)
        {
            var clientes = _context.Clientes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nombreCliente))
            {
                clientes = clientes.Where(c =>
                    c.NombreCliente.Contains(nombreCliente));
            }

            ViewBag.NombreCliente = nombreCliente;

            return View(await clientes
                .OrderBy(c => c.NombreCliente)
                .ToListAsync());
        }

        // GET: /Cliente/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Cliente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                cliente.FechaCreacion = DateTime.Now;
                cliente.FechaRegistro = DateTime.Now;

                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();

                _context.ClienteHistorial.Add(
                    new ClienteHistorial
                    {
                        IdCliente = cliente.IdCliente,
                        CampoModificado = "CREACION",
                        ValorNuevo = "Cliente creado",
                        IdUsuario = ObtenerIdUsuarioActual()
                    });

                await _context.SaveChangesAsync();

                TempData["Success"] = "Cliente creado correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(cliente);
        }

        // GET: /Cliente/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // POST: /Cliente/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cliente cliente)
        {
            if (id != cliente.IdCliente)
                return NotFound();

            if (!ModelState.IsValid)
                return View(cliente);

            var clienteActual = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (clienteActual == null)
                return NotFound();

            var cambios = new List<ClienteHistorial>();
            var idUsuarioActual = ObtenerIdUsuarioActual();

            if (clienteActual.NombreCliente != cliente.NombreCliente)
            {
                cambios.Add(new ClienteHistorial
                {
                    IdCliente = id,
                    CampoModificado = "nombre_cliente",
                    ValorAnterior = clienteActual.NombreCliente,
                    ValorNuevo = cliente.NombreCliente,
                    IdUsuario = idUsuarioActual
                });
            }

            if (clienteActual.TelefonoCelular != cliente.TelefonoCelular)
            {
                cambios.Add(new ClienteHistorial
                {
                    IdCliente = id,
                    CampoModificado = "telefono_celular",
                    ValorAnterior = clienteActual.TelefonoCelular,
                    ValorNuevo = cliente.TelefonoCelular,
                    IdUsuario = idUsuarioActual
                });
            }

            if (clienteActual.TelefonoResidencia != cliente.TelefonoResidencia)
            {
                cambios.Add(new ClienteHistorial
                {
                    IdCliente = id,
                    CampoModificado = "telefono_residencia",
                    ValorAnterior = clienteActual.TelefonoResidencia,
                    ValorNuevo = cliente.TelefonoResidencia,
                    IdUsuario = idUsuarioActual
                });
            }

            if (clienteActual.TelefonoEmpresa != cliente.TelefonoEmpresa)
            {
                cambios.Add(new ClienteHistorial
                {
                    IdCliente = id,
                    CampoModificado = "telefono_empresa",
                    ValorAnterior = clienteActual.TelefonoEmpresa,
                    ValorNuevo = cliente.TelefonoEmpresa,
                    IdUsuario = idUsuarioActual
                });
            }

            if (clienteActual.Empresa != cliente.Empresa)
            {
                cambios.Add(new ClienteHistorial
                {
                    IdCliente = id,
                    CampoModificado = "empresa",
                    ValorAnterior = clienteActual.Empresa,
                    ValorNuevo = cliente.Empresa,
                    IdUsuario = idUsuarioActual
                });
            }

            if (clienteActual.Contacto != cliente.Contacto)
            {
                cambios.Add(new ClienteHistorial
                {
                    IdCliente = id,
                    CampoModificado = "contacto",
                    ValorAnterior = clienteActual.Contacto,
                    ValorNuevo = cliente.Contacto,
                    IdUsuario = idUsuarioActual
                });
            }

            if (clienteActual.Direccion != cliente.Direccion)
            {
                cambios.Add(new ClienteHistorial
                {
                    IdCliente = id,
                    CampoModificado = "direccion",
                    ValorAnterior = clienteActual.Direccion,
                    ValorNuevo = cliente.Direccion,
                    IdUsuario = idUsuarioActual
                });
            }

            // Actualizar datos
            clienteActual.NombreCliente = cliente.NombreCliente;
            clienteActual.TelefonoCelular = cliente.TelefonoCelular;
            clienteActual.TelefonoResidencia = cliente.TelefonoResidencia;
            clienteActual.TelefonoEmpresa = cliente.TelefonoEmpresa;
            clienteActual.Empresa = cliente.Empresa;
            clienteActual.Contacto = cliente.Contacto;
            clienteActual.Direccion = cliente.Direccion;
            clienteActual.FechaActualizacion = DateTime.Now;

            if (cambios.Any())
            {
                _context.ClienteHistorial.AddRange(cambios);
            }

            await _context.SaveChangesAsync();

            TempData["Success"] = "Cliente actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Historial(int id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Historial)
                    .ThenInclude(h => h.Usuario)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }

        // POST: /Cliente/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cliente eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Cliente/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
                return NotFound();

            return View(cliente);
        }
    }
}
