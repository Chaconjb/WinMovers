using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class ExportacionController : Controller
    {
        private readonly WinMoversContext _context;

        private static readonly string[] DocsWinMovers =
        {
            "Reporte de Visita Previa", "Cotización", "Lista de inventario para el seguro",
            "Cotización con firma de aceptación", "Hoja de Trabajo", "Pre-Aviso al agente de destino",
            "Instrucciones del Embarque", "Carte de porte, AWA o B-L",
            "Certificado del seguro", "Lista de empaque firmada", "Factura", "Confirmación de Entrega"
        };

        private static readonly string[] DocsOtroAgente =
        {
            "Reporte de Visita Previa", "Lista de inventario para el seguro",
            "Pre-Aviso al agente de destino", "Instrucciones del Embarque",
            "Carte de porte, AWA o B-L", "Certificado del seguro",
            "Lista de empaque firmada", "Factura"
        };

        public ExportacionController(WinMoversContext context)
        {
            _context = context;
        }

        // GET: /Exportacion
        public async Task<IActionResult> Index()
        {
            var exportaciones = await _context.Exportaciones
                .Include(e => e.Documentos)
                .OrderByDescending(e => e.FechaCreacion)
                .ToListAsync();
            return View(exportaciones);
        }

        // GET: /Exportacion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Exportacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exportacion exportacion)
        {
            if (ModelState.IsValid)
            {
                exportacion.FechaCreacion = DateTime.Now;

                // Crear documentos predeterminados para WinMovers
                foreach (var doc in DocsWinMovers)
                    exportacion.Documentos.Add(new ExportacionDocumento { NombreDocumento = doc, TipoAgente = "WinMovers" });

                // Crear documentos predeterminados para Otro Agente
                foreach (var doc in DocsOtroAgente)
                    exportacion.Documentos.Add(new ExportacionDocumento { NombreDocumento = doc, TipoAgente = "OtroAgente" });

                _context.Exportaciones.Add(exportacion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Embarque de Exportación creado. Ahora puede gestionar el checklist de documentos.";
                return RedirectToAction(nameof(Checklist), new { id = exportacion.IdExportacion });
            }
            return View(exportacion);
        }

        // GET: /Exportacion/Checklist/5
        public async Task<IActionResult> Checklist(int id)
        {
            var exportacion = await _context.Exportaciones
                .Include(e => e.Documentos)
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
                .FirstOrDefaultAsync(e => e.IdExportacion == id);

            if (exportacion == null) return NotFound();

            foreach (var doc in exportacion.Documentos)
                doc.Completado = documentosCompletados.Contains(doc.IdDocumento);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Checklist actualizado correctamente.";
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
