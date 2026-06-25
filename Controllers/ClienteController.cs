using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class ClienteController : Controller
    {
        private readonly WinMoversContext _context;

        public ClienteController(WinMoversContext context)
        {
            _context = context;
        }

        // GET: /Cliente
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes
                .OrderBy(c => c.NombreCliente)
                .ToListAsync();

            return View(clientes);
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
                        Usuario = User?.Identity?.Name ?? "Sistema"
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

            if (clienteActual.NombreCliente != cliente.NombreCliente)
            {
                cambios.Add(new ClienteHistorial
                {
                    IdCliente = id,
                    CampoModificado = "nombre_cliente",
                    ValorAnterior = clienteActual.NombreCliente,
                    ValorNuevo = cliente.NombreCliente,
                    Usuario = User?.Identity?.Name ?? "Sistema"
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
                    Usuario = User?.Identity?.Name ?? "Sistema"
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
                    Usuario = User?.Identity?.Name ?? "Sistema"
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
                    Usuario = User?.Identity?.Name ?? "Sistema"
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
                    Usuario = User?.Identity?.Name ?? "Sistema"
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
                    Usuario = User?.Identity?.Name ?? "Sistema"
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
                    Usuario = User?.Identity?.Name ?? "Sistema"
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
