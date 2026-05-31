using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class ImportacionController : Controller
    {
        private readonly WinMoversContext _context;

        

        public ImportacionController(WinMoversContext context)
        {
            _context = context;
        }

        // GET: /Importacion
        public async Task<IActionResult> Index(string cliente, DateTime? fecha)
        {
            var importaciones = _context.Importaciones
                .Include(i => i.Documentos)
                .AsQueryable();

            if (!string.IsNullOrEmpty(cliente))
            {
                importaciones = importaciones.Where(i =>
                    i.NombreCliente.Contains(cliente));
            }

            if (fecha.HasValue)
            {
                importaciones = importaciones.Where(i =>
                    i.Fecha.HasValue &&
                    i.Fecha.Value.Date == fecha.Value.Date);
            }

            ViewBag.Cliente = cliente;
            ViewBag.Fecha = fecha?.ToString("yyyy-MM-dd");

            return View(await importaciones.ToListAsync());
        }

        // GET: /Importacion/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Importacion importacion)
        {
            // VALIDACION DEL PAIS
            if (string.IsNullOrWhiteSpace(importacion.Pais))
            {
                ModelState.AddModelError("Pais",
                    "El país es obligatorio");
            }

            if (ModelState.IsValid)
            {
                importacion.FechaCreacion = DateTime.Now;

                _context.Importaciones.Add(importacion);

                await _context.SaveChangesAsync();

                TempData["Success"] =
                    "Importación creada correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(importacion);
        }

        // GET: /Importacion/Checklist/5
        public async Task<IActionResult> Checklist(int id)
        {
            var importacion = await _context.Importaciones
                .Include(i => i.Documentos)
                .FirstOrDefaultAsync(i => i.IdImportacion == id);

            if (importacion == null) return NotFound();
            return View(importacion);
        }

        // POST: /Importacion/Checklist/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checklist(int id, List<int> documentosCompletados)
        {
            var importacion = await _context.Importaciones
                .Include(i => i.Documentos)
                .FirstOrDefaultAsync(i => i.IdImportacion == id);

            if (importacion == null) return NotFound();

            foreach (var doc in importacion.Documentos)
                doc.Completado = documentosCompletados.Contains(doc.IdImpDoc);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Checklist actualizado correctamente.";
            return RedirectToAction(nameof(Checklist), new { id });
        }

        // GET: /Importacion/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var importacion = await _context.Importaciones
                .FirstOrDefaultAsync(i => i.IdImportacion == id);

            if (importacion == null)
                return NotFound();

            return View(importacion);
        }

        // POST: /Importacion/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Importacion importacion)
        {
            if (id != importacion.IdImportacion)
                return NotFound();

            if (!ModelState.IsValid)
                return View(importacion);

            var embarque = await _context.Importaciones
                .FirstOrDefaultAsync(i => i.IdImportacion == id);

            if (embarque == null)
                return NotFound();

            embarque.NombreCliente = importacion.NombreCliente;
            embarque.Pais = importacion.Pais;
            embarque.Referencia = importacion.Referencia;
            embarque.Fecha = importacion.Fecha;
            embarque.Cajas = importacion.Cajas;
            embarque.Kilos = importacion.Kilos;
            embarque.Observaciones = importacion.Observaciones;
            embarque.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Checklist), new { id });
        }

        // POST: /Importacion/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var importacion = await _context.Importaciones.FindAsync(id);
            if (importacion != null)
            {
                _context.Importaciones.Remove(importacion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Embarque eliminado.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
