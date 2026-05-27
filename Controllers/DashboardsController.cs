using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WinMovers.Data;
using WinMovers.Models;
using WinMovers.Models.ViewModels;

namespace WinMovers.Controllers
{
    public class DashboardsController : Controller
    {
        private readonly WinMoversContext _context;

        public DashboardsController(WinMoversContext context)
        {
            _context = context;
        }

        // DASHBOARD
        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel
            {
                TotalOrdenes       = await _context.OrdenesTrabajo.CountAsync(),
                TotalVisitas       = await _context.ControlVisitas.CountAsync(),
                TotalExportaciones = await _context.Exportaciones.CountAsync(),
                TotalImportaciones = await _context.Importaciones.CountAsync(),
                OrdenesRecientes   = await _context.OrdenesTrabajo
                    .OrderByDescending(o => o.FechaCreacion)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
}
