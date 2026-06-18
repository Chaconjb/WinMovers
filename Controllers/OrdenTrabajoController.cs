using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class OrdenTrabajoController : Controller
    {
        private readonly WinMoversContext _context;
        private readonly IWebHostEnvironment _env;

        private const long MaxBytes = 10 * 1024 * 1024;
        private static readonly string[] TiposPermitidos =
            ["application/pdf", "image/jpeg", "image/png", "image/webp"];

        public OrdenTrabajoController(WinMoversContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
                // Verificar si ya existe una orden para la misma fecha
                bool fechaOcupada = await _context.OrdenesTrabajo
                    .AnyAsync(o => o.FechaServicio.HasValue &&
                                   orden.FechaServicio.HasValue &&
                                   o.FechaServicio.Value.Date == orden.FechaServicio.Value.Date);

                if (fechaOcupada)
                {
                    ModelState.AddModelError("FechaServicio",
                        "Ya existe una orden programada para esta fecha.");

                    return View(orden);
                }

                orden.FechaCreacion = DateTime.Now;

                _context.OrdenesTrabajo.Add(orden);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Orden de Trabajo creada correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(orden);
        }

        // GET: /OrdenTrabajo/Edit/5  — incluye archivos
        public async Task<IActionResult> Edit(int id)
        {
            var orden = await _context.OrdenesTrabajo
                .Include(o => o.Archivos)   // <-- nuevo
                .FirstOrDefaultAsync(o => o.IdOrden == id);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirArchivo(int idOrden, IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                return Json(new { ok = false, mensaje = "No se recibió ningún archivo." });
            }

            if (archivo.Length > MaxBytes)
            {
                return Json(new { ok = false, mensaje = "El archivo supera el límite de 10 MB permitido." });
            }

            if (!TiposPermitidos.Contains(archivo.ContentType))
            {
                return Json(new { ok = false, mensaje = "Solo se permiten PDF o imágenes (JPG, PNG, WEBP)." });
            }

            var extension = Path.GetExtension(archivo.FileName);
            var nombreGuid = $"{Guid.NewGuid()}{extension}";

            var rutaCarpeta = Path.Combine(_env.WebRootPath, "uploads", "ordenes");
            Directory.CreateDirectory(rutaCarpeta);

            var rutaCompleta = Path.Combine(rutaCarpeta, nombreGuid);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var registro = new OrdenTrabajoArchivo
            {
                IdOrden = idOrden,
                NombreOriginal = archivo.FileName,
                NombreGuardado = nombreGuid,
                TipoMime = archivo.ContentType,
                TamanioBytes = archivo.Length,
                FechaCarga = DateTime.Now
            };

            _context.OrdenesTrabajosArchivos.Add(registro);
            await _context.SaveChangesAsync();

            // ?? ACTUALIZAR ESTADO DE LA ORDEN
            var numeroOT = await ObtenerNumeroOT(idOrden);

            if (!string.IsNullOrEmpty(numeroOT))
            {
                await ActualizarEstadoOrden(numeroOT);
            }

            return Json(new
            {
                ok = true,
                mensaje = "Documento cargado correctamente."
            });
        }
        private async Task<string?> ObtenerNumeroOT(int idOrden)
        {
            return await _context.OrdenesTrabajo
                .Where(o => o.IdOrden == idOrden)
                .Select(o => o.NumeroOT)
                .FirstOrDefaultAsync();
        }
        // POST: /OrdenTrabajo/EliminarArchivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarArchivo(int idArchivo)
        {
            var archivo = await _context.OrdenesTrabajosArchivos
                .FirstOrDefaultAsync(a => a.IdArchivo == idArchivo);

            if (archivo == null)
                return Json(new { ok = false, mensaje = "El archivo no existe." });

            var rutaCompleta = Path.Combine(_env.WebRootPath, "uploads", "ordenes", archivo.NombreGuardado);

            if (System.IO.File.Exists(rutaCompleta))
                System.IO.File.Delete(rutaCompleta);

            _context.OrdenesTrabajosArchivos.Remove(archivo);
            await _context.SaveChangesAsync();

            return Json(new { ok = true, mensaje = "Documento eliminado correctamente." });
        }

        // GET: /OrdenTrabajo/DescargarArchivo/5
        [HttpGet]
        public async Task<IActionResult> DescargarArchivo(int idArchivo)
        {
            var archivo = await _context.OrdenesTrabajosArchivos
                .FirstOrDefaultAsync(a => a.IdArchivo == idArchivo);

            if (archivo == null) return NotFound();

            var rutaCompleta = Path.Combine(_env.WebRootPath, "uploads", "ordenes", archivo.NombreGuardado);

            if (!System.IO.File.Exists(rutaCompleta)) return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(rutaCompleta);
            return File(bytes, archivo.TipoMime, archivo.NombreOriginal);
        }
        
        

        private async Task ActualizarEstadoOrden(string numeroOT)
        {
            var importacion = await _context.Importaciones
                .Include(i => i.Documentos)
                .FirstOrDefaultAsync(i => i.Referencia == numeroOT);

            var exportacion = await _context.Exportaciones
                .Include(e => e.Documentos)
                .FirstOrDefaultAsync(e => e.Referencia == numeroOT);

            bool importacionCompleta =
                importacion != null &&
                importacion.Documentos.Any() &&
                importacion.Documentos.All(d => d.Completado);

            bool exportacionCompleta =
                exportacion != null &&
                exportacion.Documentos.Any() &&
                exportacion.Documentos.All(d => d.Completado);

            var orden = await _context.OrdenesTrabajo
                .FirstOrDefaultAsync(o => o.NumeroOT == numeroOT);

            if (orden != null)
            {
                orden.Estado = (importacionCompleta && exportacionCompleta)
                    ? "Completado"
                    : "Pendiente";

                orden.FechaActualizacion = DateTime.Now;

                await _context.SaveChangesAsync();
            }
        }
    }
}