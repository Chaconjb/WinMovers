using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    public class ImportacionController : Controller
    {
        private readonly WinMoversContext _context;
        private readonly IWebHostEnvironment _env;

        // Límite y tipos permitidos
        private const long MaxBytes = 10 * 1024 * 1024; // 10 MB
        private static readonly string[] TiposPermitidos =
            ["application/pdf", "image/jpeg", "image/png", "image/webp"];

        public ImportacionController(WinMoversContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
            if (string.IsNullOrWhiteSpace(importacion.Pais))
            {
                ModelState.AddModelError("Pais", "El país es obligatorio");
            }

            ///nuevo

            var referenciaExiste = await _context.OrdenesTrabajo
    .AnyAsync(o => o.NumeroOT == importacion.Referencia);

            if (!referenciaExiste)
            {
                ModelState.AddModelError("Referencia",
                    "La referencia ingresada no existe en una Orden de Trabajo.");
            }


            if (ModelState.IsValid)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_Importacion_Insertar @p0, @p1, @p2, @p3, @p4, @p5, @p6",
                    importacion.NombreCliente,
                    importacion.Pais,
                    importacion.Cajas,
                    importacion.Kilos,
                    importacion.Referencia,
                    importacion.Fecha,
                    importacion.Observaciones
                );

                TempData["Success"] = "Importación creada correctamente.";

                return RedirectToAction(nameof(Index));
            }

            return View(importacion);
        }

        // GET: /Importacion/Checklist/5
        public async Task<IActionResult> Checklist(int id)
        {
            var importacion = await _context.Importaciones
                .Include(i => i.Documentos)
                .ThenInclude(d => d.TipoDocumento)
                .Include(i => i.Archivos)
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
                .ThenInclude(d => d.TipoDocumento)
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
        //Métodos para subir archivos

        // POST: /Importacion/SubirArchivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubirArchivo(int idImportacion, IFormFile archivo)
        {
            // Escenario 2: tamaño excedido
            if (archivo == null || archivo.Length == 0)
                return Json(new { ok = false, mensaje = "No se recibió ningún archivo." });

            if (archivo.Length > MaxBytes)
                return Json(new { ok = false, mensaje = "El archivo supera el límite de 10 MB permitido." });

            // Validar tipo MIME
            if (!TiposPermitidos.Contains(archivo.ContentType))
                return Json(new { ok = false, mensaje = "Solo se permiten archivos PDF o imágenes (JPG, PNG, WEBP)." });

            // Escenario 1: guardar en disco
            var extension = Path.GetExtension(archivo.FileName);
            var nombreGuid = $"{Guid.NewGuid()}{extension}";
            var rutaCarpeta = Path.Combine(_env.WebRootPath, "uploads", "importaciones");

            Directory.CreateDirectory(rutaCarpeta); // no falla si ya existe

            var rutaCompleta = Path.Combine(rutaCarpeta, nombreGuid);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                await archivo.CopyToAsync(stream);

            // Guardar en BD usando EF (igual que el resto del controller)
            var registro = new ImportacionArchivo
            {
                IdImportacion = idImportacion,
                NombreOriginal = archivo.FileName,
                NombreGuardado = nombreGuid,
                TipoMime = archivo.ContentType,
                TamanioBytes = archivo.Length,
                FechaCarga = DateTime.Now
            };

            _context.ImportacionesArchivos.Add(registro);
            await _context.SaveChangesAsync();

            return Json(new { ok = true, mensaje = "Documento cargado correctamente." });
        }

        // POST: /Importacion/EliminarArchivo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarArchivo(int idArchivo)
        {
            // Escenario 3: eliminar
            var archivo = await _context.ImportacionesArchivos
                .FirstOrDefaultAsync(a => a.IdArchivo == idArchivo);

            if (archivo == null)
                return Json(new { ok = false, mensaje = "El archivo no existe." });

            // Primero eliminar físicamente del disco
            var rutaCompleta = Path.Combine(_env.WebRootPath, "uploads", "importaciones", archivo.NombreGuardado);

            if (System.IO.File.Exists(rutaCompleta))
                System.IO.File.Delete(rutaCompleta);

            // Luego eliminar de la BD
            _context.ImportacionesArchivos.Remove(archivo);
            await _context.SaveChangesAsync();

            return Json(new { ok = true, mensaje = "Documento eliminado correctamente." });
        }

        // GET: /Importacion/DescargarArchivo/5
        [HttpGet]
        public async Task<IActionResult> DescargarArchivo(int idArchivo)
        {
            var archivo = await _context.ImportacionesArchivos
                .FirstOrDefaultAsync(a => a.IdArchivo == idArchivo);

            if (archivo == null) return NotFound();

            var rutaCompleta = Path.Combine(_env.WebRootPath, "uploads", "importaciones", archivo.NombreGuardado);

            if (!System.IO.File.Exists(rutaCompleta)) return NotFound();

            var bytes = await System.IO.File.ReadAllBytesAsync(rutaCompleta);
            return File(bytes, archivo.TipoMime, archivo.NombreOriginal);
        }
    }
}
