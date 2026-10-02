using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class InventarioController : Controller
    {
        private readonly WinMoversContext _context;

        public InventarioController(WinMoversContext context)
        {
            _context = context;
        }

        // GET: Inventario
        public async Task<IActionResult> Index(string? buscar)
        {
            var materiales = _context.Inventario.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                materiales = materiales.Where(m =>
                    m.NombreMaterial.Contains(buscar));
            }

            ViewBag.Buscar = buscar;

            return View(await materiales
                .OrderBy(m => m.NombreMaterial)
                .ToListAsync());
        }

        // GET: Inventario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Inventario/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Inventario inventario)
        {
            if (!ModelState.IsValid)
                return View(inventario);

            inventario.FechaCreacion = DateTime.Now;

            _context.Inventario.Add(inventario);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Material registrado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Inventario/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var material = await _context.Inventario.FindAsync(id);

            if (material == null)
                return NotFound();

            return View(material);
        }

        // POST: Inventario/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Inventario inventario)
        {
            if (id != inventario.IdMaterial)
                return NotFound();

            if (!ModelState.IsValid)
                return View(inventario);

            var material = await _context.Inventario.FindAsync(id);

            if (material == null)
                return NotFound();

            material.NombreMaterial = inventario.NombreMaterial;
            material.Descripcion = inventario.Descripcion;
            material.Categoria = inventario.Categoria;
            material.Unidad = inventario.Unidad;
            material.Existencias = inventario.Existencias;
            material.StockMinimo = inventario.StockMinimo;
            material.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Material actualizado correctamente.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Inventario/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var material = await _context.Inventario.FindAsync(id);

            if (material == null)
                return NotFound();

            return View(material);
        }

        // POST: Inventario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var material = await _context.Inventario.FindAsync(id);

            if (material != null)
            {
                _context.Inventario.Remove(material);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Material eliminado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}