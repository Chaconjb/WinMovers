using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class ExportacionController : Controller
    {
        private readonly WinMoversContext _context;

        public ExportacionController(WinMoversContext context)
        {
            _context = context;
        }

        // GET: /Exportacion
        public async Task<IActionResult> Index(string cliente, DateTime? fecha)
        {
            var exportaciones = _context.Exportaciones
                .Include(e => e.Documentos)
                .AsQueryable();

            if (!string.IsNullOrEmpty(cliente))
            {
                exportaciones = exportaciones.Where(e =>
                    e.NombreCliente.Contains(cliente));
            }

            if (fecha.HasValue)
            {
                exportaciones = exportaciones.Where(e =>
                    e.Fecha.HasValue &&
                    e.Fecha.Value.Date == fecha.Value.Date);
            }

            ViewBag.Cliente = cliente;
            ViewBag.Fecha = fecha?.ToString("yyyy-MM-dd");

            return View(await exportaciones.ToListAsync());
        }

        // GET: /Exportacion/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exportacion exportacion)
        {
            if (ModelState.IsValid)
            {
                exportacion.FechaCreacion = DateTime.Now;

                _context.Exportaciones.Add(exportacion);

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Exportacion_Insertar @p0, @p1, @p2, @p3, @p4, @p5",
                    exportacion.NombreCliente,
                    exportacion.Cajas,
                    exportacion.Kilos,
                    exportacion.Referencia,
                    exportacion.Fecha,
                    exportacion.Observaciones
                );

                TempData["Success"] = "Exportación creada correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(exportacion);
        }

        // GET: /Exportacion/Checklist/5
        public async Task<IActionResult> Checklist(int id)
        {
            var exportacion = await _context.Exportaciones
                .Include(e => e.Documentos)
                .ThenInclude(d => d.TipoDocumento)
                .FirstOrDefaultAsync(e => e.IdExportacion == id);

            if (exportacion == null) return NotFound();
            return View(exportacion);
        }

        // POST: /Exportacion/Checklist/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checklist(int id, List<int> documentosCompletados)
        {
            var exportacion = await _context.Exportaciones
                .Include(e => e.Documentos)
                .ThenInclude(d => d.TipoDocumento)
                .FirstOrDefaultAsync(e => e.IdExportacion == id);

            if (exportacion == null) return NotFound();

            foreach (var doc in exportacion.Documentos)
                doc.Completado = documentosCompletados.Contains(doc.IdExpDoc);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Checklist actualizado correctamente.";
            return RedirectToAction(nameof(Checklist), new { id });
        }

        // GET: /Exportacion/Edit
        public async Task<IActionResult> Edit(int id)
        {
            var importacion = await _context.Exportaciones
                .FirstOrDefaultAsync(e => e.IdExportacion == id);

            if (importacion == null)
                return NotFound();

            return View(importacion);
        }

        // POST: /Exportacion/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exportacion exportacion)
        {
            if (id != exportacion.IdExportacion)
                return NotFound();

            if (!ModelState.IsValid)
                return View(exportacion);

            var embarque = await _context.Exportaciones
                .FirstOrDefaultAsync(e => e.IdExportacion == id);

            if (embarque == null)
                return NotFound();

            embarque.NombreCliente = exportacion.NombreCliente;
            embarque.Referencia = exportacion.Referencia;
            embarque.Fecha = exportacion.Fecha;
            embarque.Cajas = exportacion.Cajas;
            embarque.Kilos = exportacion.Kilos;
            embarque.Observaciones = exportacion.Observaciones;
            embarque.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Checklist), new { id });
        }

        // POST: /Exportacion/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var exportacion = await _context.Exportaciones.FindAsync(id);
            if (exportacion != null)
            {
                _context.Exportaciones.Remove(exportacion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Embarque eliminado.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
