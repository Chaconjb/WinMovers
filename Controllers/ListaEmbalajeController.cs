using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;

namespace WinMovers.Controllers
{
    // HU-INV-002: lista de embalaje que documenta los bienes transportados
    // en una orden. Se apoya en los bienes registrados por HU-INV-003.
    public class ListaEmbalajeController : Controller
    {
        private readonly WinMoversContext _context;

        public ListaEmbalajeController(WinMoversContext context)
        {
            _context = context;
        }

        // GET: /ListaEmbalaje/Index/5   (5 = id de la orden de trabajo)
        public async Task<IActionResult> Index(int id)
        {
            var orden = await _context.OrdenesTrabajo
                .Include(o => o.Bienes)
                .Include(o => o.ListasEmbalaje)
                    .ThenInclude(l => l.Detalles)
                        .ThenInclude(d => d.Bien)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
                return NotFound();

            var lista = orden.ListasEmbalaje.FirstOrDefault();

            if (lista != null)
            {
                lista.Detalles = lista.Detalles
                    .OrderBy(d => d.Bien.NombreBien)
                    .ToList();
            }

            // Bienes de la orden que todavía no están en la lista: son los
            // únicos que tiene sentido ofrecer para agregar (Escenario 3).
            var yaIncluidos = lista?.Detalles.Select(d => d.IdBien).ToHashSet()
                              ?? new HashSet<int>();

            ViewBag.BienesDisponibles = orden.Bienes
                .Where(b => !yaIncluidos.Contains(b.IdBien))
                .OrderBy(b => b.NombreBien)
                .ToList();

            ViewBag.Lista = lista;

            return View(orden);
        }

        // POST: /ListaEmbalaje/Generar
        // Escenario 1 (generación correcta) y Escenario 2 (faltan obligatorios).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generar(int idOrden, string? responsable, string? observaciones)
        {
            var orden = await _context.OrdenesTrabajo
                .Include(o => o.Bienes)
                .Include(o => o.ListasEmbalaje)
                .FirstOrDefaultAsync(o => o.IdOrden == idOrden);

            if (orden == null)
            {
                TempData["Error"] = "La orden de trabajo no existe.";
                return RedirectToAction("Index", "OrdenTrabajo");
            }

            if (orden.ListasEmbalaje.Any())
            {
                TempData["Error"] = "Esta orden ya tiene una lista de embalaje.";
                return RedirectToAction(nameof(Index), new { id = idOrden });
            }

            // Escenario 2: sin bienes registrados no hay nada que documentar.
            var faltantes = new List<string>();

            if (string.IsNullOrWhiteSpace(responsable))
                faltantes.Add("Responsable");

            if (!orden.Bienes.Any())
                faltantes.Add("al menos un bien registrado en la orden");

            if (faltantes.Any())
            {
                TempData["Error"] =
                    "Faltan campos obligatorios: " + string.Join(", ", faltantes) + ".";

                return RedirectToAction(nameof(Index), new { id = idOrden });
            }

            // Escenario 1: se crea la lista asociada a la orden, precargada con
            // los bienes ya registrados. El operador luego ajusta si hace falta.
            var lista = new ListaEmbalaje
            {
                IdOrden = idOrden,
                NumeroLista = await GenerarNumeroListaAsync(orden),
                Responsable = responsable!.Trim(),
                Observaciones = string.IsNullOrWhiteSpace(observaciones)
                    ? null
                    : observaciones.Trim(),
                Estado = EstadosListaEmbalaje.Borrador,
                FechaGeneracion = DateTime.Now
            };

            foreach (var bien in orden.Bienes)
            {
                lista.Detalles.Add(new ListaEmbalajeDetalle
                {
                    IdBien = bien.IdBien,
                    Cantidad = bien.Cantidad
                });
            }

            _context.ListasEmbalaje.Add(lista);

            await _context.SaveChangesAsync();

            TempData["Success"] =
                $"Lista de embalaje {lista.NumeroLista} generada con {lista.Detalles.Count} bien(es).";

            return RedirectToAction(nameof(Index), new { id = idOrden });
        }

        // POST: /ListaEmbalaje/AgregarBien
        // Escenario 3: el operador agrega un bien a la lista existente.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgregarBien(int idLista, int idBien, int cantidad)
        {
            var lista = await _context.ListasEmbalaje
                .Include(l => l.Detalles)
                .FirstOrDefaultAsync(l => l.IdLista == idLista);

            if (lista == null)
            {
                TempData["Error"] = "La lista de embalaje no existe.";
                return RedirectToAction("Index", "OrdenTrabajo");
            }

            if (cantidad < 1)
            {
                TempData["Error"] = "La cantidad debe ser mayor a cero.";
                return RedirectToAction(nameof(Index), new { id = lista.IdOrden });
            }

            // El bien debe pertenecer a la misma orden que la lista.
            var bien = await _context.BienesMudanza
                .FirstOrDefaultAsync(b => b.IdBien == idBien &&
                                          b.IdOrden == lista.IdOrden);

            if (bien == null)
            {
                TempData["Error"] = "El bien no pertenece a esta orden.";
                return RedirectToAction(nameof(Index), new { id = lista.IdOrden });
            }

            if (lista.Detalles.Any(d => d.IdBien == idBien))
            {
                TempData["Error"] =
                    $"El bien \"{bien.NombreBien}\" ya está en la lista de embalaje.";

                return RedirectToAction(nameof(Index), new { id = lista.IdOrden });
            }

            _context.ListasEmbalajeDetalle.Add(new ListaEmbalajeDetalle
            {
                IdLista = idLista,
                IdBien = idBien,
                Cantidad = cantidad
            });

            lista.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Bien \"{bien.NombreBien}\" agregado a la lista.";

            return RedirectToAction(nameof(Index), new { id = lista.IdOrden });
        }

        // POST: /ListaEmbalaje/EliminarBien
        // Escenario 3: el operador quita un bien de la lista.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarBien(int idDetalle, int idOrden)
        {
            var detalle = await _context.ListasEmbalajeDetalle
                .Include(d => d.Bien)
                .Include(d => d.Lista)
                .FirstOrDefaultAsync(d => d.IdDetalle == idDetalle);

            if (detalle == null || detalle.Lista.IdOrden != idOrden)
            {
                TempData["Error"] = "El bien ya no está en la lista.";
                return RedirectToAction(nameof(Index), new { id = idOrden });
            }

            var nombre = detalle.Bien.NombreBien;

            detalle.Lista.FechaActualizacion = DateTime.Now;

            _context.ListasEmbalajeDetalle.Remove(detalle);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Bien \"{nombre}\" eliminado de la lista.";

            return RedirectToAction(nameof(Index), new { id = idOrden });
        }

        // POST: /ListaEmbalaje/Guardar
        // Escenario 2 en su otra cara: guardar la lista sin bienes o sin
        // responsable debe avisar de los campos obligatorios.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Guardar(int idLista, string? responsable,
                                                 string? observaciones, bool finalizar = false)
        {
            var lista = await _context.ListasEmbalaje
                .Include(l => l.Detalles)
                .FirstOrDefaultAsync(l => l.IdLista == idLista);

            if (lista == null)
            {
                TempData["Error"] = "La lista de embalaje no existe.";
                return RedirectToAction("Index", "OrdenTrabajo");
            }

            var faltantes = new List<string>();

            if (string.IsNullOrWhiteSpace(responsable))
                faltantes.Add("Responsable");

            if (!lista.Detalles.Any())
                faltantes.Add("al menos un bien en la lista");

            if (faltantes.Any())
            {
                TempData["Error"] =
                    "Faltan campos obligatorios: " + string.Join(", ", faltantes) + ".";

                return RedirectToAction(nameof(Index), new { id = lista.IdOrden });
            }

            lista.Responsable = responsable!.Trim();

            lista.Observaciones = string.IsNullOrWhiteSpace(observaciones)
                ? null
                : observaciones.Trim();

            if (finalizar)
                lista.Estado = EstadosListaEmbalaje.Finalizada;

            lista.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = finalizar
                ? $"Lista de embalaje {lista.NumeroLista} finalizada."
                : "Lista de embalaje actualizada correctamente.";

            return RedirectToAction(nameof(Index), new { id = lista.IdOrden });
        }

        // Correlativo por orden: LE-<O.T.>-<n>. Se calcula sobre el total de
        // listas existentes porque hoy sólo puede haber una por orden, pero
        // deja la puerta abierta si más adelante se admiten varias.
        private async Task<string> GenerarNumeroListaAsync(OrdenTrabajo orden)
        {
            var consecutivo = await _context.ListasEmbalaje.CountAsync() + 1;

            return $"LE-{orden.NumeroOT}-{consecutivo:D3}";
        }
    }
}
