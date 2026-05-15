using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class OrdenTrabajoController : Controller
    {
        private readonly WinMoversContext _context;

        public OrdenTrabajoController(WinMoversContext context)
        {
            _context = context;
        }

        // GET: /OrdenTrabajo
        public async Task<IActionResult> Index()
        {
            var ordenes = await _context.OrdenesTrabajo
                .OrderByDescending(o => o.FechaCreacion)
                .ToListAsync();
            return View(ordenes);
        }

        // GET: /OrdenTrabajo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /OrdenTrabajo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrdenTrabajo orden)
        {
            if (ModelState.IsValid)
            {
                orden.FechaCreacion = DateTime.Now;
                _context.OrdenesTrabajo.Add(orden);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Orden de Trabajo creada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(orden);
        }

        // GET: /OrdenTrabajo/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var orden = await _context.OrdenesTrabajo.FindAsync(id);
            if (orden == null) return NotFound();
            return View(orden);
        }

        // POST: /OrdenTrabajo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrdenTrabajo orden)
        {
            if (id != orden.IdOrden) return NotFound();

            if (ModelState.IsValid)
            {
                orden.FechaActualizacion = DateTime.Now;
                _context.Update(orden);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Orden de Trabajo actualizada correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(orden);
        }

        // POST: /OrdenTrabajo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var orden = await _context.OrdenesTrabajo.FindAsync(id);
            if (orden != null)
            {
                _context.OrdenesTrabajo.Remove(orden);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Orden de Trabajo eliminada.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
