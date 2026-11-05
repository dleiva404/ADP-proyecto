using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartProcessAI.Models;

namespace SmartProcessAI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("panel")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("tareas/registrar")]
        public IActionResult CargarDatos()
        {
            return View();
        }

        [Route("hallazgos")]
        public IActionResult Hallazgos()
        {
            return View();
        }

        [Route("historial")]
        public IActionResult Historial()
        {
            return View();
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
