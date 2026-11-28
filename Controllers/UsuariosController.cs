using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartProcessAI.Models;
using SmartProcessAI.Services;
using SmartProcessAI.ViewModels;
namespace SmartProcessAI.Controllers
{
    [Authorize(Roles = RolesSistema.AdminTI)]
    public class UsuariosController : Controller
    {
        private readonly UsuarioService _servicio;
        public UsuariosController(UsuarioService servicio)
        {
            _servicio = servicio;
        }
        private int UsuarioActualId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }
        private void Resultado(string? error, string exito)
        {
            if (error == null)
                TempData["Aviso"] = exito;
            else
                TempData["Error"] = error;
        }
        [HttpGet("usuarios")]
        public async Task<IActionResult> Index()
        {
            var vm = new UsuariosViewModel
            {
                Lista = await _servicio.ListarAsync(),
                Roles = await _servicio.RolesAsync()
            };
            return View(vm);
        }
        [HttpGet("usuarios/datos")]
        public async Task<IActionResult> Datos()
        {
            var lista = await _servicio.ListarAsync();
            return Json(lista.Select(u => new
            {
                u.Id,
                u.Nombre,
                u.Correo,
                Rol = u.Rol?.Nombre,
                u.Activo
            }));
        }
        [HttpPost("usuarios/crear")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(string? nombre, string? correo, int rolId, string? clave)
        {
            var error = await _servicio.CrearAsync(nombre, correo, rolId, clave, UsuarioActualId());
            Resultado(error, "El usuario se creó correctamente.");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost("usuarios/editar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, string? nombre, int rolId)
        {
            var error = await _servicio.EditarAsync(id, nombre, rolId, UsuarioActualId());
            Resultado(error, "Los datos del usuario se actualizaron.");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost("usuarios/estado")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Estado(int id, bool activo)
        {
            var error = await _servicio.CambiarEstadoAsync(id, activo, UsuarioActualId());
            Resultado(error, activo ? "El usuario se activó." : "El usuario se desactivó.");
            return RedirectToAction(nameof(Index));
        }
        [HttpPost("usuarios/clave")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clave(int id, string? clave)
        {
            var error = await _servicio.RestablecerClaveAsync(id, clave);
            Resultado(error, "La contraseña se restableció.");
            return RedirectToAction(nameof(Index));
        }
    }
}
