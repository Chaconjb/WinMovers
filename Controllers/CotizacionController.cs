using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;
using WinMovers.Services;

namespace WinMovers.Controllers
{
    // Roles: "asesor" de las historias = Empleado, "administrativo" = Administrador.
    [Authorize(Roles = $"{Roles.Administrador},{Roles.Empleado}")]
    public class CotizacionController : Controller
    {
        private readonly WinMoversContext _context;
        private readonly IQuoteService _quoteService;
        private readonly ICotizacionEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CotizacionController> _logger;

        public CotizacionController(
            WinMoversContext context,
            IQuoteService quoteService,
            ICotizacionEmailService emailService,
            UserManager<ApplicationUser> userManager,
            ILogger<CotizacionController> logger)
        {
            _context = context;
            _quoteService = quoteService;
            _emailService = emailService;
            _userManager = userManager;
            _logger = logger;
        }

        private int? ObtenerIdUsuarioActual()
        {
            var idTexto = _userManager.GetUserId(User);
            return idTexto != null ? int.Parse(idTexto) : null;
        }

        // =========================================================
        // HU-COT-002: registro de cotizaciones
        // =========================================================

        // GET: /Cotizacion
        public async Task<IActionResult> Index(string? termino, string? estado)
        {
            var consulta = _context.Cotizaciones
                .Include(c => c.Cliente)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(termino))
            {
                var t = termino.Trim();
                consulta = consulta.Where(c =>
                    c.NumeroCotizacion.Contains(t) ||
                    c.NombreCliente.Contains(t) ||
                    (c.Compania != null && c.Compania.Contains(t)));
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                consulta = consulta.Where(c => c.Estado == estado);
            }

            var modelo = new CotizacionBusquedaViewModel
            {
                Termino = termino,
                Estado = estado,
                Resultados = await consulta
                    .OrderByDescending(c => c.FechaCreacion)
                    .ToListAsync()
            };

            return View(modelo);
        }

        // GET: /Cotizacion/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Cliente)
                .Include(c => c.Usuario)
                .Include(c => c.OrdenGenerada)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id);

            if (cotizacion == null) return NotFound();

            ViewBag.TarifaEnLetras = _quoteService.MontoEnLetras(cotizacion.TarifaTotal);

            return View(cotizacion);
        }

        // =========================================================
        // HU-COT-001 + HU-COT-002: calcular y guardar
        // =========================================================

        // GET: /Cotizacion/Create
        public async Task<IActionResult> Create()
        {
            var cotizacion = new Cotizacion
            {
                NumeroCotizacion = await _quoteService.GenerarNumeroAsync(),
                Fecha = DateTime.Today,
                HechoPor = (await _userManager.GetUserAsync(User))?.NombreCompleto,
                Exclusiones = ExclusionesPorDefecto
            };

            await CargarCombosAsync();
            return View(cotizacion);
        }

        // POST: /Cotizacion/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cotizacion cotizacion)
        {
            ValidarReglasDeNegocio(cotizacion);

            if (!ModelState.IsValid)
            {
                await CargarCombosAsync();
                return View(cotizacion);
            }

            // El número lo genera el sistema; si vino vacío o duplicado desde el
            // formulario, se reasigna para no chocar con el índice único.
            if (string.IsNullOrWhiteSpace(cotizacion.NumeroCotizacion) ||
                await _context.Cotizaciones.AnyAsync(c => c.NumeroCotizacion == cotizacion.NumeroCotizacion))
            {
                cotizacion.NumeroCotizacion = await _quoteService.GenerarNumeroAsync();
            }

            await VincularClienteAsync(cotizacion);

            _quoteService.AplicarCalculo(cotizacion);

            cotizacion.Estado = EstadosCotizacion.Borrador;
            cotizacion.IdUsuario = ObtenerIdUsuarioActual();
            cotizacion.FechaCreacion = DateTime.Now;

            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Cotización {cotizacion.NumeroCotizacion} creada por " +
                                  $"{cotizacion.Moneda} {cotizacion.TarifaTotal:N2}.";

            return RedirectToAction(nameof(Details), new { id = cotizacion.IdCotizacion });
        }

        // GET: /Cotizacion/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);

            if (cotizacion == null) return NotFound();

            if (!cotizacion.SePuedeEditar)
            {
                TempData["Error"] = "Esta cotización ya fue convertida en orden de trabajo y no se puede editar.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await CargarCombosAsync();
            return View(cotizacion);
        }

        // POST: /Cotizacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cotizacion cotizacion)
        {
            if (id != cotizacion.IdCotizacion) return NotFound();

            ValidarReglasDeNegocio(cotizacion);

            if (!ModelState.IsValid)
            {
                await CargarCombosAsync();
                return View(cotizacion);
            }

            var actual = await _context.Cotizaciones.FirstOrDefaultAsync(c => c.IdCotizacion == id);

            if (actual == null) return NotFound();

            if (!actual.SePuedeEditar)
            {
                TempData["Error"] = "Esta cotización ya fue convertida en orden de trabajo y no se puede editar.";
                return RedirectToAction(nameof(Details), new { id });
            }

            // Cliente
            actual.Fecha = cotizacion.Fecha;
            actual.NombreCliente = cotizacion.NombreCliente;
            actual.Compania = cotizacion.Compania;
            actual.Contacto = cotizacion.Contacto;
            actual.CorreoCliente = cotizacion.CorreoCliente;
            actual.TelefonoCelular = cotizacion.TelefonoCelular;

            // Servicio
            actual.TipoServicio = cotizacion.TipoServicio;
            actual.Origen = cotizacion.Origen;
            actual.Destino = cotizacion.Destino;
            actual.VolumenM3 = cotizacion.VolumenM3;
            actual.TipoContenedor = cotizacion.TipoContenedor;
            actual.CompaniaMaritima = cotizacion.CompaniaMaritima;
            actual.Corresponsal = cotizacion.Corresponsal;

            // Cronograma
            actual.DiasEmpaque = cotizacion.DiasEmpaque;
            actual.DiasTransito = cotizacion.DiasTransito;
            actual.DiasDesalmacenaje = cotizacion.DiasDesalmacenaje;
            actual.DiasFrecuenciaSalidas = cotizacion.DiasFrecuenciaSalidas;

            // Rubros
            actual.CostoOrigen = cotizacion.CostoOrigen;
            actual.CostoTramitesAduana = cotizacion.CostoTramitesAduana;
            actual.CostoFlete = cotizacion.CostoFlete;
            actual.CostoDestino = cotizacion.CostoDestino;

            // Seguro
            actual.IncluyeSeguro = cotizacion.IncluyeSeguro;
            actual.ValorDeclarado = cotizacion.ValorDeclarado;
            actual.PorcentajeSeguro = cotizacion.PorcentajeSeguro;

            // Condiciones
            actual.VigenciaDias = cotizacion.VigenciaDias;
            actual.FormaPago = cotizacion.FormaPago;
            actual.Exclusiones = cotizacion.Exclusiones;
            actual.Observaciones = cotizacion.Observaciones;
            actual.HechoPor = cotizacion.HechoPor;

            await VincularClienteAsync(actual);

            _quoteService.AplicarCalculo(actual);

            actual.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Cotización {actual.NumeroCotizacion} actualizada. " +
                                  $"Nueva tarifa: {actual.Moneda} {actual.TarifaTotal:N2}.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Cotizacion/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);

            if (cotizacion == null) return NotFound();

            // Si ya generó una orden de trabajo, borrarla dejaría la orden
            // huérfana de su origen comercial.
            if (cotizacion.Estado == EstadosCotizacion.Convertida)
            {
                TempData["Error"] = "No se puede eliminar una cotización que ya fue convertida en orden de trabajo.";
                return RedirectToAction(nameof(Index));
            }

            _context.Cotizaciones.Remove(cotizacion);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Cotización {cotizacion.NumeroCotizacion} eliminada.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /Cotizacion/Calcular  — calculadora en vivo del formulario (HU-COT-001)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Calcular([FromBody] Cotizacion cotizacion)
        {
            var resultado = _quoteService.Calcular(cotizacion);

            return Json(new
            {
                subtotal = resultado.Subtotal,
                montoSeguro = resultado.MontoSeguro,
                tarifaTotal = resultado.TarifaTotal,
                enLetras = resultado.TarifaTotalEnLetras
            });
        }

        // =========================================================
        // HU-COT-003: enviar la cotización al cliente
        // =========================================================

        // GET: /Cotizacion/Enviar/5
        public async Task<IActionResult> Enviar(int id)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);

            if (cotizacion == null) return NotFound();

            var modelo = new EnviarCotizacionViewModel
            {
                IdCotizacion = cotizacion.IdCotizacion,
                NumeroCotizacion = cotizacion.NumeroCotizacion,
                NombreCliente = cotizacion.NombreCliente,
                Destinatario = cotizacion.CorreoCliente ?? string.Empty,
                Asunto = $"Cotización {cotizacion.NumeroCotizacion} - WinMovers"
            };

            return View(modelo);
        }

        // POST: /Cotizacion/Enviar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enviar(EnviarCotizacionViewModel modelo)
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.IdCotizacion == modelo.IdCotizacion);

            if (cotizacion == null) return NotFound();

            if (!ModelState.IsValid)
                return View(modelo);

            // Enviar una cotización sin tarifa sería enviarle al cliente un
            // machote en blanco.
            if (cotizacion.TarifaTotal <= 0)
            {
                ModelState.AddModelError("", "La cotización no tiene una tarifa calculada. " +
                                             "Edítela e ingrese los montos antes de enviarla.");
                return View(modelo);
            }

            try
            {
                await _emailService.EnviarCotizacionAsync(
                    cotizacion,
                    modelo.Destinatario.Trim(),
                    modelo.Asunto,
                    modelo.MensajeAdicional);
            }
            catch (Exception ex)
            {
                // Si el correo falla, la cotización NO debe quedar marcada como
                // enviada: el asesor creería que el cliente ya la tiene.
                _logger.LogError(ex, "Falló el envío de la cotización {Numero}", cotizacion.NumeroCotizacion);

                ModelState.AddModelError("", "No se pudo enviar el correo. Intente de nuevo o " +
                                             "revise la configuración de envío.");
                return View(modelo);
            }

            cotizacion.Estado = EstadosCotizacion.Enviada;
            cotizacion.FechaEnvio = DateTime.Now;
            cotizacion.CorreoEnvio = modelo.Destinatario.Trim();
            cotizacion.FechaActualizacion = DateTime.Now;

            // Si el cliente no tenía correo registrado, aprovechamos el que se usó.
            if (string.IsNullOrWhiteSpace(cotizacion.CorreoCliente))
                cotizacion.CorreoCliente = modelo.Destinatario.Trim();

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Cotización {cotizacion.NumeroCotizacion} enviada a {cotizacion.CorreoEnvio}.";
            return RedirectToAction(nameof(Details), new { id = cotizacion.IdCotizacion });
        }

        // GET: /Cotizacion/VistaPrevia/5 — ver el machote antes de enviarlo
        public async Task<IActionResult> VistaPrevia(int id)
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id);

            if (cotizacion == null) return NotFound();

            var html = await _emailService.ConstruirCuerpoAsync(cotizacion);
            return Content(html, "text/html");
        }

        // POST: /Cotizacion/CambiarEstado — el cliente respondió
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, string estado)
        {
            var cotizacion = await _context.Cotizaciones.FindAsync(id);

            if (cotizacion == null) return NotFound();

            var permitidos = new[] { EstadosCotizacion.Aceptada, EstadosCotizacion.Rechazada };

            if (!permitidos.Contains(estado))
            {
                TempData["Error"] = "Estado no válido.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (cotizacion.Estado == EstadosCotizacion.Convertida)
            {
                TempData["Error"] = "La cotización ya fue convertida en orden de trabajo.";
                return RedirectToAction(nameof(Details), new { id });
            }

            cotizacion.Estado = estado;
            cotizacion.FechaActualizacion = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Cotización marcada como {estado}.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // =========================================================
        // HU-COT-004: convertir la cotización en orden de mudanza
        // =========================================================

        // GET: /Cotizacion/Convertir/5
        public async Task<IActionResult> Convertir(int id)
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id);

            if (cotizacion == null) return NotFound();

            if (!cotizacion.SePuedeConvertir)
            {
                TempData["Error"] = cotizacion.Estado == EstadosCotizacion.Convertida
                    ? "Esta cotización ya fue convertida en orden de trabajo."
                    : "Una cotización rechazada no se puede convertir en orden de trabajo.";

                return RedirectToAction(nameof(Details), new { id });
            }

            ViewBag.NumeroOTSugerido = cotizacion.NumeroCotizacion.Replace("COT-", "OT-");
            return View(cotizacion);
        }

        // POST: /Cotizacion/Convertir/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Convertir(int id, string numeroOT, DateTime? fechaServicio, string? hora)
        {
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id);

            if (cotizacion == null) return NotFound();

            // Revalidamos en el POST: entre el GET y el envío del formulario
            // otro usuario pudo haberla convertido.
            if (!cotizacion.SePuedeConvertir)
            {
                TempData["Error"] = "Esta cotización ya no se puede convertir.";
                return RedirectToAction(nameof(Details), new { id });
            }

            if (string.IsNullOrWhiteSpace(numeroOT))
            {
                ModelState.AddModelError("", "El número de orden de trabajo es requerido.");
                ViewBag.NumeroOTSugerido = cotizacion.NumeroCotizacion.Replace("COT-", "OT-");
                return View(cotizacion);
            }

            numeroOT = numeroOT.Trim();

            if (await _context.OrdenesTrabajo.AnyAsync(o => o.NumeroOT == numeroOT))
            {
                ModelState.AddModelError("", $"Ya existe una orden de trabajo con el número {numeroOT}.");
                ViewBag.NumeroOTSugerido = numeroOT;
                return View(cotizacion);
            }

            // El módulo de órdenes bloquea dos servicios en la misma fecha y hora.
            if (fechaServicio.HasValue && !string.IsNullOrWhiteSpace(hora))
            {
                var conflicto = await _context.OrdenesTrabajo
                    .FirstOrDefaultAsync(o => o.FechaServicio == fechaServicio
                                           && o.Hora == hora
                                           && o.Estado != "Completado");

                if (conflicto != null)
                {
                    ModelState.AddModelError("",
                        $"Ya existe la orden {conflicto.NumeroOT} ({conflicto.NombreCliente}) " +
                        $"programada para esa fecha y hora. Elija otro horario.");

                    ViewBag.NumeroOTSugerido = numeroOT;
                    return View(cotizacion);
                }
            }

            var orden = new OrdenTrabajo
            {
                NumeroOT = numeroOT,
                Fecha = DateTime.Today,
                FechaServicio = fechaServicio,
                Hora = hora,
                NombreCliente = cotizacion.NombreCliente,
                IdCliente = cotizacion.IdCliente,
                TelefonoCelular = cotizacion.TelefonoCelular,
                Compania = cotizacion.Compania,
                Contacto = cotizacion.Contacto,
                DireccionOrigen = cotizacion.Origen,
                DireccionDestino = cotizacion.Destino,
                DetalleServicio = ArmarDetalleServicio(cotizacion),
                FacturarA = cotizacion.Compania ?? cotizacion.NombreCliente,
                HechoPor = cotizacion.HechoPor,
                Estado = "Pendiente",
                FechaCreacion = DateTime.Now
            };

            await using var transaccion = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.OrdenesTrabajo.Add(orden);
                await _context.SaveChangesAsync();

                cotizacion.Estado = EstadosCotizacion.Convertida;
                cotizacion.IdOrdenGenerada = orden.IdOrden;
                cotizacion.FechaActualizacion = DateTime.Now;

                // Trazabilidad: queda registrado de dónde salió la orden.
                _context.OrdenesTrabajoHistorial.Add(new OrdenTrabajoHistorial
                {
                    IdOrden = orden.IdOrden,
                    CampoModificado = "origen",
                    ValorAnterior = "(nueva)",
                    ValorNuevo = $"Generada desde la cotización {cotizacion.NumeroCotizacion} " +
                                 $"por {cotizacion.Moneda} {cotizacion.TarifaTotal:N2}",
                    IdUsuario = ObtenerIdUsuarioActual(),
                    FechaCambio = DateTime.Now
                });

                await _context.SaveChangesAsync();
                await transaccion.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaccion.RollbackAsync();
                _logger.LogError(ex, "Falló la conversión de la cotización {Numero}", cotizacion.NumeroCotizacion);

                TempData["Error"] = "No se pudo convertir la cotización. Intente de nuevo.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Success"] = $"Cotización {cotizacion.NumeroCotizacion} convertida en la orden {orden.NumeroOT}.";
            return RedirectToAction("Edit", "OrdenTrabajo", new { id = orden.IdOrden });
        }

        // =========================================================
        // AUXILIARES
        // =========================================================

        // Resume la cotización en el campo de texto de la orden de trabajo.
        private static string ArmarDetalleServicio(Cotizacion c)
        {
            var lineas = new List<string>
            {
                $"Servicio {c.TipoServicio} — cotización {c.NumeroCotizacion}."
            };

            if (!string.IsNullOrWhiteSpace(c.Origen) || !string.IsNullOrWhiteSpace(c.Destino))
                lineas.Add($"Origen: {c.Origen ?? "—"} / Destino: {c.Destino ?? "—"}");

            if (c.VolumenM3 > 0)
            {
                var contenedor = string.IsNullOrWhiteSpace(c.TipoContenedor)
                    ? ""
                    : $" en contenedor de {c.TipoContenedor}";

                lineas.Add($"Volumen estimado: {c.VolumenM3:N2} m³{contenedor}.");
            }

            lineas.Add($"Tarifa acordada: {c.Moneda} {c.TarifaTotal:N2}.");

            if (c.IncluyeSeguro && c.ValorDeclarado > 0)
                lineas.Add($"Incluye seguro {c.PorcentajeSeguro:N1}% sobre valor declarado " +
                           $"{c.Moneda} {c.ValorDeclarado:N2}.");

            if (!string.IsNullOrWhiteSpace(c.Observaciones))
                lineas.Add($"Observaciones: {c.Observaciones}");

            return string.Join(Environment.NewLine, lineas);
        }

        private void ValidarReglasDeNegocio(Cotizacion cotizacion)
        {
            // El seguro es 3.5% del valor declarado: sin valor declarado no hay
            // nada sobre qué calcularlo.
            if (cotizacion.IncluyeSeguro && (cotizacion.ValorDeclarado == null || cotizacion.ValorDeclarado <= 0))
            {
                ModelState.AddModelError(nameof(Cotizacion.ValorDeclarado),
                    "Para incluir el seguro debe indicar el valor declarado del menaje.");
            }

            var totalRubros = cotizacion.CostoOrigen + cotizacion.CostoTramitesAduana
                            + cotizacion.CostoFlete + cotizacion.CostoDestino;

            if (totalRubros <= 0)
            {
                ModelState.AddModelError(nameof(Cotizacion.CostoOrigen),
                    "Debe ingresar al menos un rubro de costo mayor a cero.");
            }
        }

        // Liga la cotización a un cliente existente si el nombre coincide,
        // igual que hace OrdenTrabajoController al crear una orden.
        private async Task VincularClienteAsync(Cotizacion cotizacion)
        {
            if (cotizacion.IdCliente.HasValue)
            {
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.IdCliente == cotizacion.IdCliente);

                if (cliente != null)
                {
                    cotizacion.NombreCliente = cliente.NombreCliente;

                    cotizacion.CorreoCliente ??= cliente.CorreoElectronico;
                    cotizacion.TelefonoCelular ??= cliente.TelefonoCelular;
                    cotizacion.Compania ??= cliente.Empresa;

                    return;
                }
            }

            var porNombre = await _context.Clientes
                .FirstOrDefaultAsync(c => c.NombreCliente == cotizacion.NombreCliente);

            if (porNombre != null)
                cotizacion.IdCliente = porNombre.IdCliente;
        }

        private async Task CargarCombosAsync()
        {
            ViewBag.Clientes = await _context.Clientes
                .Where(c => c.Activo)
                .OrderBy(c => c.NombreCliente)
                .Select(c => new { c.IdCliente, c.NombreCliente })
                .ToListAsync();
        }

        private const string ExclusionesPorDefecto =
            "Demoras portuarias, bodegajes en aduana, servicio de transbordo si son necesarios, " +
            "gastos portuarios, utilización de equipos especiales para la entrega después de 2do piso, " +
            "inspecciones extras de aduana si el embarque es solicitado.";
    }
}
