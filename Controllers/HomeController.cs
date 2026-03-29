using Microsoft.AspNetCore.Mvc;

namespace WinMovers.Controllers
{
    public class HomeController : Controller
    {
        // Página principal
        public IActionResult Index()
        {
            return View();
        }

        // Página de privacidad (opcional)
        public IActionResult Privacy()
        {
            return View();
        }
    }
}