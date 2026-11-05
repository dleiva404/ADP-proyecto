using Microsoft.AspNetCore.Mvc;

namespace SmartProcessAI.Controllers
{
    public class LoginController : Controller
    {
        // muestra la pantalla de login
        public IActionResult Index()
        {
            return View();
        }
    }
}