using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class ControlVisitaController : Controller
    {
        private readonly WinMoversContext _context;

        public ControlVisitaController(WinMoversContext context)
        {
            _context = context;
        }

        // GET: /ControlVisita
        public async Task<IActionResult> Index()
        {
            var visitas = await _context.ControlVisitas
                .OrderByDescending(v => v.FechaCreacion)
                .ToListAsync();
            return View(visitas);
        }

        // GET: /ControlVisita/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /ControlVisita/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ControlVisita visita)
        {
            if (ModelState.IsValid)
            {
                visita.FechaCreacion = DateTime.Now;
                _context.ControlVisitas.Add(visita);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Control de Visita registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(visita);
        }

        // GET: /ControlVisita/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var visita = await _context.ControlVisitas.FindAsync(id);
            if (visita == null) return NotFound();
            return View(visita);
        }

        // POST: /ControlVisita/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ControlVisita visita)
        {
            if (id != visita.IdVisita) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(visita);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Control de Visita actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            return View(visita);
        }

        // POST: /ControlVisita/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var visita = await _context.ControlVisitas.FindAsync(id);
            if (visita != null)
            {
                _context.ControlVisitas.Remove(visita);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Control de Visita eliminado.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
