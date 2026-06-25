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

            if (ModelState.IsValid)
            {
                cliente.FechaActualizacion = DateTime.Now;

                _context.Update(cliente);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cliente actualizado correctamente.";

                return RedirectToAction(nameof(Index));
            }

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
