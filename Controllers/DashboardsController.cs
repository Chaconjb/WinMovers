using Microsoft.AspNetCore.Mvc;

namespace WinMovers.Controllers
{
    public class DashboardsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CargarVista(string nombre)
        {
            switch (nombre)
            {
                case "clientes":
                    return PartialView("~/Views/Dashboards/_Clientes.cshtml");

                case "cotizaciones":
                    return PartialView("~/Views/Dashboards/_Cotizaciones.cshtml");

                case "mudanzas":
                    return PartialView("~/Views/Dashboards/_Mudanzas.cshtml");

                case "inventario":
                    return PartialView("~/Views/Dashboards/_Inventario.cshtml");

                case "empleados":
                    return PartialView("~/Views/Dashboards/_Empleados.cshtml");

                default:
                    return Content("Vista no encontrada");
            }
        }
    }
}