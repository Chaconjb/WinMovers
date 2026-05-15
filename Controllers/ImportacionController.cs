using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class ImportacionController : Controller
    {
        private readonly WinMoversContext _context;

        private static readonly string[] DocsWinMovers =
        {
            "Cotización", "Lista de inventario para el seguro",
            "Cotización con firma de aceptación", "Hoja de Trabajo",
            "Instrucciones del Embarque", "Carte de porte, AWA o B-L",
            "Certificado del seguro", "Lista de empaque firmada",
            "Factura", "Confirmación de Entrega"
        };

        private static readonly string[] DocsOtroAgente =
        {
            "Lista de inventario para el seguro", "Hoja de Trabajo",
            "Instrucciones del Embarque", "Carte de porte, AWA o B-L",
            "Certificado del seguro", "Lista de empaque firmada",
            "Factura", "Confirmación de Entrega"
        };

        public ImportacionController(WinMoversContext context)
        {
            _context = context;
        }

        // GET: /Importacion
        public async Task<IActionResult> Index()
        {
            var importaciones = await _context.Importaciones
                .Include(i => i.Documentos)
                .OrderByDescending(i => i.FechaCreacion)
                .ToListAsync();
            return View(importaciones);
        }

        // GET: /Importacion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Importacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Importacion importacion)
        {
            if (ModelState.IsValid)
            {
                importacion.FechaCreacion = DateTime.Now;

                foreach (var doc in DocsWinMovers)
                    importacion.Documentos.Add(new ImportacionDocumento { NombreDocumento = doc, TipoAgente = "WinMovers" });

                foreach (var doc in DocsOtroAgente)
                    importacion.Documentos.Add(new ImportacionDocumento { NombreDocumento = doc, TipoAgente = "OtroAgente" });

                _context.Importaciones.Add(importacion);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Embarque de Importación creado. Ahora puede gestionar el checklist de documentos.";
                return RedirectToAction(nameof(Checklist), new { id = importacion.IdImportacion });
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
                doc.Completado = documentosCompletados.Contains(doc.IdDocumento);

            await _context.SaveChangesAsync();
            TempData["Success"] = "Checklist actualizado correctamente.";
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
