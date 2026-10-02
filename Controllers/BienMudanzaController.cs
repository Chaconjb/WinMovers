using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;
using WinMovers.Models.ViewModels;

namespace WinMovers.Controllers
{
    // HU-INV-003: registro de los bienes del cliente por mudanza, para
    // trazabilidad de qué se transportó en cada orden de trabajo.
    public class BienMudanzaController : Controller
    {
        private readonly WinMoversContext _context;

        public BienMudanzaController(WinMoversContext context)
        {
            _context = context;
        }

        // GET: /BienMudanza/Index/5   (5 = id de la orden de trabajo)
        public async Task<IActionResult> Index(int id)
        {
            var orden = await _context.OrdenesTrabajo
                .Include(o => o.Bienes)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
                return NotFound();

            orden.Bienes = orden.Bienes
                .OrderBy(b => b.NombreBien)
                .ToList();

            return View(orden);
        }

        // POST: /BienMudanza/Registrar
        // Escenario 1 (alta correcta) y Escenario 2 (bien duplicado).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistrarBienViewModel bien)
        {
            var orden = await _context.OrdenesTrabajo
                .FirstOrDefaultAsync(o => o.IdOrden == bien.IdOrden);

            if (orden == null)
            {
                TempData["Error"] = "La orden de trabajo no existe.";
                return RedirectToAction("Index", "OrdenTrabajo");
            }

            if (!string.IsNullOrWhiteSpace(bien.Condicion) &&
                !CondicionesBien.Todas.Contains(bien.Condicion))
            {
                ModelState.AddModelError(nameof(bien.Condicion),
                    "La condición seleccionada no es válida.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = PrimerError();
                return RedirectToAction(nameof(Index), new { id = bien.IdOrden });
            }

            var nombre = bien.NombreBien.Trim();

            // Escenario 2: el bien ya está registrado en la MISMA orden. La
            // comparación es case-insensitive porque el cotejamiento por
            // defecto de SQL Server lo es, y así el mensaje coincide con lo
            // que haría el índice único al rechazar el INSERT.
            var duplicado = await _context.BienesMudanza
                .AnyAsync(b => b.IdOrden == bien.IdOrden &&
                               b.NombreBien == nombre);

            if (duplicado)
            {
                TempData["Error"] =
                    $"El bien \"{nombre}\" ya está registrado en esta orden.";

                return RedirectToAction(nameof(Index), new { id = bien.IdOrden });
            }

            // Escenario 1: datos válidos, se almacena.
            _context.BienesMudanza.Add(new BienMudanza
            {
                IdOrden = bien.IdOrden,
                NombreBien = nombre,
                Descripcion = bien.Descripcion?.Trim(),
                Cantidad = bien.Cantidad,
                Condicion = bien.Condicion,
                Observaciones = bien.Observaciones?.Trim(),
                FechaRegistro = DateTime.Now
            });

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Dos peticiones simultáneas para el mismo bien: el índice único
                // rechaza la segunda. Se reporta igual que el Escenario 2 en vez
                // de dejar salir un error 500.
                _context.ChangeTracker.Clear();

                if (!await ExisteBienAsync(bien.IdOrden, nombre))
                    throw;

                TempData["Error"] =
                    $"El bien \"{nombre}\" ya está registrado en esta orden.";

                return RedirectToAction(nameof(Index), new { id = bien.IdOrden });
            }

            TempData["Success"] = "Bien registrado correctamente.";

            return RedirectToAction(nameof(Index), new { id = bien.IdOrden });
        }

        // POST: /BienMudanza/Eliminar
        // Escenario 3: el operador confirma y el bien sale de la orden.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int idBien, int idOrden)
        {
            var bien = await _context.BienesMudanza
                .Include(b => b.DetallesLista)
                .FirstOrDefaultAsync(b => b.IdBien == idBien &&
                                          b.IdOrden == idOrden);

            if (bien == null)
            {
                TempData["Error"] = "El bien ya no existe en esta orden.";
                return RedirectToAction(nameof(Index), new { id = idOrden });
            }

            // La FK con la lista de embalaje es Restrict, así que los renglones
            // que referencian al bien se quitan primero. Se avisa al operador
            // porque su lista de embalaje cambia como efecto secundario.
            var estabaEnLista = bien.DetallesLista.Any();

            if (estabaEnLista)
                _context.ListasEmbalajeDetalle.RemoveRange(bien.DetallesLista);

            _context.BienesMudanza.Remove(bien);

            await _context.SaveChangesAsync();

            TempData["Success"] = estabaEnLista
                ? $"Bien \"{bien.NombreBien}\" eliminado de la orden y de su lista de embalaje."
                : $"Bien \"{bien.NombreBien}\" eliminado de la orden.";

            return RedirectToAction(nameof(Index), new { id = idOrden });
        }

        private Task<bool> ExisteBienAsync(int idOrden, string nombre) =>
            _context.BienesMudanza
                .AnyAsync(b => b.IdOrden == idOrden && b.NombreBien == nombre);

        private string PrimerError() =>
            ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault(m => !string.IsNullOrWhiteSpace(m))
            ?? "Los datos del bien no son válidos.";
    }
}
