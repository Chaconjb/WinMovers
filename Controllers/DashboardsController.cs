using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;
using WinMovers.Models.ViewModels;
using WinMovers.Services;



namespace WinMovers.Controllers
{
    public class DashboardsController : Controller
    {
        private readonly WinMoversContext _context;
        private readonly ILogger<DashboardsController> _logger;
        private readonly IPronosticoService _pronosticoService;

        public DashboardsController(WinMoversContext context, ILogger<DashboardsController> logger, IPronosticoService pronosticoService)
        {
            _context = context;
            _logger = logger;
            _pronosticoService = pronosticoService;
        }

        // DASHBOARD
        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalOrdenes = await _context.OrdenesTrabajo.CountAsync(),
                TotalCotizaciones = await _context.Cotizaciones.CountAsync(),
                TotalVisitas = await _context.ControlVisitas.CountAsync(),
                TotalExportaciones = await _context.Exportaciones.CountAsync(),
                TotalImportaciones = await _context.Importaciones.CountAsync(),
                OrdenesRecientes = await _context.OrdenesTrabajo
                    .OrderByDescending(o => o.FechaCreacion)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // =====================================================
        // HU-DAS-001: Dashboard de importaciones/exportaciones
        // =====================================================
        public async Task<IActionResult> Analitica()
        {
            var modelo = new AnaliticaViewModel();

            try
            {
                // Escenario 2: si ninguna de las dos tablas tiene registros,
                // no tiene sentido armar gráficos vacíos.
                bool hayImportaciones = await _context.Importaciones.AnyAsync();
                bool hayExportaciones = await _context.Exportaciones.AnyAsync();

                if (!hayImportaciones && !hayExportaciones)
                {
                    modelo.HayDatos = false;
                    return View(modelo);
                }

                modelo.HayDatos = true;

                // Traemos los últimos 12 meses con datos, agrupados por año-mes.
                var importPorMes = await _context.Importaciones
                    .Where(i => i.Fecha != null)
                    .GroupBy(i => new { i.Fecha!.Value.Year, i.Fecha.Value.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        Cajas = g.Sum(x => x.Cajas),
                        Kilos = g.Sum(x => x.Kilos)
                    })
                    .ToListAsync();

                var exportPorMes = await _context.Exportaciones
                    .Where(e => e.Fecha != null)
                    .GroupBy(e => new { e.Fecha!.Value.Year, e.Fecha.Value.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        Cajas = g.Sum(x => x.Cajas),
                        Kilos = g.Sum(x => x.Kilos)
                    })
                    .ToListAsync();

                // Unificamos todos los pares año-mes que aparecen en cualquiera
                // de las dos tablas, para que ambas series compartan el mismo eje X.
                var todosLosMeses = importPorMes.Select(x => (x.Year, x.Month))
                    .Union(exportPorMes.Select(x => (x.Year, x.Month)))
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .TakeLast(12)
                    .ToList();

                var culturaEs = new System.Globalization.CultureInfo("es-CR");

                foreach (var (year, month) in todosLosMeses)
                {
                    var fechaMes = new DateTime(year, month, 1);
                    modelo.Meses.Add(fechaMes.ToString("MMM yyyy", culturaEs));

                    var imp = importPorMes.FirstOrDefault(x => x.Year == year && x.Month == month);
                    var exp = exportPorMes.FirstOrDefault(x => x.Year == year && x.Month == month);

                    modelo.CajasImportacion.Add(imp?.Cajas ?? 0);
                    modelo.CajasExportacion.Add(exp?.Cajas ?? 0);
                    modelo.KilosImportacion.Add(imp?.Kilos ?? 0);
                    modelo.KilosExportacion.Add(exp?.Kilos ?? 0);
                }

                // Top 5 países de origen en importaciones.
                modelo.TopPaises = await _context.Importaciones
                    .Where(i => i.Pais != null && i.Pais != "")
                    .GroupBy(i => i.Pais!)
                    .Select(g => new PaisConteo { Pais = g.Key, Cantidad = g.Count() })
                    .OrderByDescending(p => p.Cantidad)
                    .Take(5)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Escenario 3: cualquier error de consulta se captura aquí.
                modelo.HuboError = true;
                modelo.MensajeError = "Ocurrió un error al cargar la información del dashboard. Intenta de nuevo más tarde.";
                _logger.LogError(ex, "Error al cargar Analitica del dashboard");
            }

            return View(modelo);
        }

        // =====================================================
        // HU-DAS-002: Histórico de Órdenes de Trabajo
        // =====================================================
        public async Task<IActionResult> Historico(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var modelo = new HistoricoViewModel
            {
                FechaInicio = fechaInicio ?? DateTime.Today.AddMonths(-11).Date,
                FechaFin = fechaFin ?? DateTime.Today
            };

            try
            {
                var ordenesEnRango = await _context.OrdenesTrabajo
                    .Where(o => o.FechaCreacion.Date >= modelo.FechaInicio
                             && o.FechaCreacion.Date <= modelo.FechaFin)
                    .ToListAsync();

                if (!ordenesEnRango.Any())
                {
                    modelo.HayDatos = false;
                    return View(modelo);
                }

                modelo.HayDatos = true;
                modelo.TotalOrdenesPeriodo = ordenesEnRango.Count;
                modelo.TotalCompletadasPeriodo = ordenesEnRango.Count(o => o.Estado == "Completado");
                modelo.PorcentajeCompletado = modelo.TotalOrdenesPeriodo > 0
                    ? Math.Round((double)modelo.TotalCompletadasPeriodo / modelo.TotalOrdenesPeriodo * 100, 1)
                    : 0;

                var culturaEs = new System.Globalization.CultureInfo("es-CR");

                // Nota: "completadas por mes" se agrupa según el mes en que la
                // orden fue CREADA (no cuando se completó), ya que nuestro esquema
                // actual no guarda la fecha exacta en que cambió a "Completado", tendríamos que cambiar
                // la forma de guardar la actualización de fecha para más exactitud.
                var porMes = ordenesEnRango
                    .GroupBy(o => new { o.FechaCreacion.Year, o.FechaCreacion.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .ToList();

                foreach (var grupo in porMes)
                {
                    var fechaMes = new DateTime(grupo.Key.Year, grupo.Key.Month, 1);
                    modelo.Meses.Add(fechaMes.ToString("MMM yyyy", culturaEs));
                    modelo.OrdenesCreadas.Add(grupo.Count());
                    modelo.OrdenesCompletadas.Add(grupo.Count(o => o.Estado == "Completado"));
                }
            }
            catch (Exception ex)
            {
                modelo.HuboError = true;
                modelo.MensajeError = "Ocurrió un error al cargar el histórico de órdenes. Intenta de nuevo más tarde.";
                _logger.LogError(ex, "Error al cargar Historico del dashboard");
            }

            return View(modelo);
        }

        // =====================================================
        // HU-DAS-004: Estadísticas de objetos transportados
        // =====================================================
        public async Task<IActionResult> Estadisticas()
        {
            var modelo = new EstadisticasViewModel();

            try
            {
                bool hayImportaciones = await _context.Importaciones.AnyAsync();
                bool hayExportaciones = await _context.Exportaciones.AnyAsync();

                if (!hayImportaciones && !hayExportaciones)
                {
                    modelo.HayDatos = false;
                    return View(modelo);
                }

                modelo.HayDatos = true;

                modelo.TotalCajasImportacion = await _context.Importaciones.SumAsync(i => i.Cajas);
                modelo.TotalCajasExportacion = await _context.Exportaciones.SumAsync(e => e.Cajas);
                modelo.TotalKilosImportacion = await _context.Importaciones.SumAsync(i => i.Kilos);
                modelo.TotalKilosExportacion = await _context.Exportaciones.SumAsync(e => e.Kilos);

                int totalCajas = modelo.TotalCajasImportacion + modelo.TotalCajasExportacion;
                if (totalCajas > 0)
                {
                    modelo.PorcentajeCajasImportacion = Math.Round((double)modelo.TotalCajasImportacion / totalCajas * 100, 1);
                    modelo.PorcentajeCajasExportacion = Math.Round((double)modelo.TotalCajasExportacion / totalCajas * 100, 1);
                }

                var totalCotizaciones = await _context.Cotizaciones.CountAsync();
                if (totalCotizaciones > 0)
                {
                    modelo.PorTipoServicio = await _context.Cotizaciones
                        .GroupBy(c => c.TipoServicio)
                        .Select(g => new TipoServicioConteo
                        {
                            TipoServicio = g.Key,
                            Cantidad = g.Count()
                        })
                        .ToListAsync();

                    foreach (var item in modelo.PorTipoServicio)
                    {
                        item.Porcentaje = Math.Round((double)item.Cantidad / totalCotizaciones * 100, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                modelo.HuboError = true;
                modelo.MensajeError = "Ocurrió un error al cargar las estadísticas. Intenta de nuevo más tarde.";
                _logger.LogError(ex, "Error al cargar Estadisticas del dashboard");
            }

            return View(modelo);
        }

        // =====================================================
        // HU-DAS-003: Pronóstico de transporte (ML.NET)
        // =====================================================
        private const int MESES_MINIMOS_PARA_PRONOSTICO = 6;
        private const int HORIZONTE_MESES = 3;

        public async Task<IActionResult> Pronostico()
        {
            var modelo = new PronosticoViewModel
            {
                MesesMinimosRequeridos = MESES_MINIMOS_PARA_PRONOSTICO
            };

            try
            {
                // Cajas totales (importación + exportación) agrupadas por mes.
                var importPorMes = await _context.Importaciones
                    .Where(i => i.Fecha != null)
                    .GroupBy(i => new { i.Fecha!.Value.Year, i.Fecha.Value.Month })
                    .Select(g => new { g.Key.Year, g.Key.Month, Cajas = g.Sum(x => x.Cajas) })
                    .ToListAsync();

                var exportPorMes = await _context.Exportaciones
                    .Where(e => e.Fecha != null)
                    .GroupBy(e => new { e.Fecha!.Value.Year, e.Fecha.Value.Month })
                    .Select(g => new { g.Key.Year, g.Key.Month, Cajas = g.Sum(x => x.Cajas) })
                    .ToListAsync();

                var mesesCombinados = importPorMes.Select(x => (x.Year, x.Month, x.Cajas))
                    .Concat(exportPorMes.Select(x => (x.Year, x.Month, x.Cajas)))
                    .GroupBy(x => (x.Year, x.Month))
                    .Select(g => new { g.Key.Year, g.Key.Month, Cajas = g.Sum(x => x.Cajas) })
                    .OrderBy(x => x.Year).ThenBy(x => x.Month)
                    .ToList();

                modelo.MesesHistoricosUsados = mesesCombinados.Count;

                // Escenario 2: no hay suficiente historial para un pronóstico confiable.
                if (mesesCombinados.Count < MESES_MINIMOS_PARA_PRONOSTICO)
                {
                    modelo.HayDatosSuficientes = false;
                    return View(modelo);
                }

                modelo.HayDatosSuficientes = true;

                var culturaEs = new System.Globalization.CultureInfo("es-CR");
                foreach (var mes in mesesCombinados)
                {
                    var fecha = new DateTime(mes.Year, mes.Month, 1);
                    modelo.MesesHistoricos.Add(fecha.ToString("MMM yyyy", culturaEs));
                    modelo.CajasHistoricas.Add(mes.Cajas);
                }

                // Generamos las etiquetas de los próximos 3 meses, a partir del
                // último mes con datos reales.
                var ultimoMes = new DateTime(mesesCombinados.Last().Year, mesesCombinados.Last().Month, 1);
                for (int i = 1; i <= HORIZONTE_MESES; i++)
                {
                    modelo.MesesProyectados.Add(ultimoMes.AddMonths(i).ToString("MMM yyyy", culturaEs));
                }

                var resultado = _pronosticoService.GenerarPronostico(modelo.CajasHistoricas.ToArray(), HORIZONTE_MESES);

                modelo.CajasProyectadas = resultado.Forecast.ToList();
                modelo.LimiteInferior = resultado.LimiteInferior.ToList();
                modelo.LimiteSuperior = resultado.LimiteSuperior.ToList();
            }
            catch (Exception ex)
            {
                // Escenario 3: cualquier error en el proceso de cálculo.
                modelo.HuboError = true;
                modelo.MensajeError = "Ocurrió un error al generar el pronóstico. Intenta de nuevo más tarde.";
                _logger.LogError(ex, "Error al generar Pronostico del dashboard");
            }

            return View(modelo);
        }
    }
}