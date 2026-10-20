using System.Diagnostics;
using ControlAcademico.Models;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;

namespace ControlAcademico.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardServicio _dashboard;

        public HomeController(ILogger<HomeController> logger, IDashboardServicio dashboard)
        {
            _logger = logger;
            _dashboard = dashboard;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _dashboard.ObtenerResumenAsync());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
