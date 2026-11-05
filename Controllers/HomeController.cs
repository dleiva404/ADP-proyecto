using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartProcessAI.Datos;
using SmartProcessAI.Models;
using SmartProcessAI.Services;
using SmartProcessAI.ViewModels;

namespace SmartProcessAI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _db;
        private readonly AnalizadorTareaService _analizador;

        public HomeController(ILogger<HomeController> logger, AppDbContext db, AnalizadorTareaService analizador)
        {
            _logger = logger;
            _db = db;
            _analizador = analizador;
        }

        private int UsuarioActualId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [Route("panel")]
        public async Task<IActionResult> Index()
        {
            var vm = new PanelViewModel
            {
                Auditorias = await _db.Auditorias.CountAsync(),
                Abiertos = await _db.Hallazgos.CountAsync(h => h.Estado == "Pendiente"),
                Criticos = await _db.Hallazgos.CountAsync(h => h.Riesgo == "Critico"),
                PorRevisar = await _db.Recomendaciones.CountAsync(r => r.EstadoRevision == "Pendiente"),
                Bajo = await _db.Hallazgos.CountAsync(h => h.Riesgo == "Bajo"),
                Medio = await _db.Hallazgos.CountAsync(h => h.Riesgo == "Medio"),
                Alto = await _db.Hallazgos.CountAsync(h => h.Riesgo == "Alto"),
                Critico = await _db.Hallazgos.CountAsync(h => h.Riesgo == "Critico"),
                Recientes = await _db.Hallazgos
                    .Include(h => h.Tarea).ThenInclude(t => t!.Proceso)
                    .OrderByDescending(h => h.Fecha)
                    .Take(5)
                    .ToListAsync()
            };
            return View(vm);
        }

        private async Task<TareaFormViewModel> ConProcesos(TareaFormViewModel vm)
        {
            vm.Procesos = await _db.Procesos.OrderBy(p => p.Nombre).ToListAsync();
            return vm;
        }

        [HttpGet("tareas/registrar")]
        [Authorize(Roles = "Auditor")]
        public async Task<IActionResult> CargarDatos()
        {
            return View(await ConProcesos(new TareaFormViewModel()));
        }

        [HttpPost("tareas/registrar")]
        [Authorize(Roles = "Auditor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CargarDatos(TareaFormViewModel form)
        {
            var tarea = new Tarea
            {
                Puesto = form.Puesto ?? string.Empty,
                Descripcion = form.Descripcion ?? string.Empty,
                TiempoMinutos = form.TiempoMinutos,
                FrecuenciaMensual = form.FrecuenciaMensual,
                NumeroPasos = form.NumeroPasos,
                Herramientas = form.Herramientas ?? string.Empty,
                EsManual = form.EsManual,
                ProcesoId = form.ProcesoId
            };

            var errores = ValidadorTarea.Validar(tarea);
            var proceso = tarea.ProcesoId > 0 ? await _db.Procesos.FindAsync(tarea.ProcesoId) : null;
            if (tarea.ProcesoId > 0 && proceso == null)
                errores.Add("El proceso seleccionado no existe.");

            if (errores.Count > 0 || proceso == null)
            {
                form.Errores = errores;
                return View(await ConProcesos(form));
            }

            int usuarioId = UsuarioActualId();

            // se usa la auditoría abierta del proceso, o se crea una nueva
            var auditoria = await _db.Auditorias
                .FirstOrDefaultAsync(a => a.ProcesoId == proceso.Id && a.Estado == "Abierta");
            if (auditoria == null)
            {
                auditoria = new Auditoria
                {
                    Estado = "Abierta",
                    Fecha = DateTime.Now,
                    EmpresaId = proceso.EmpresaId,
                    ProcesoId = proceso.Id,
                    AuditorId = usuarioId
                };
                _db.Auditorias.Add(auditoria);
            }

            _db.Tareas.Add(tarea);

            var hallazgo = _analizador.AnalizarTarea(tarea, auditoria, usuarioId);
            if (hallazgo != null)
                _db.Hallazgos.Add(hallazgo);

            // un solo SaveChanges: se guarda todo o no se guarda nada
            await _db.SaveChangesAsync();

            if (hallazgo == null)
            {
                TempData["Aviso"] = "La tarea se guardó. Como no es manual, no genera hallazgos.";
                return RedirectToAction(nameof(Hallazgos));
            }

            return RedirectToAction(nameof(Hallazgos), new { id = hallazgo.Id });
        }

        [Route("hallazgos")]
        public async Task<IActionResult> Hallazgos(int? id)
        {
            var lista = await _db.Hallazgos
                .Include(h => h.Tarea).ThenInclude(t => t!.Proceso)
                .Include(h => h.Recomendaciones)
                .OrderByDescending(h => h.Fecha)
                .ToListAsync();

            var seleccionado = lista.FirstOrDefault(h => h.Id == id) ?? lista.FirstOrDefault();
            return View(new HallazgosViewModel { Lista = lista, Seleccionado = seleccionado });
        }

        [HttpPost("hallazgos/decidir")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Jefatura")]
        public async Task<IActionResult> Decidir(int id, string decision, string? comentario)
        {
            var hallazgo = await _db.Hallazgos
                .Include(h => h.Recomendaciones)
                .FirstOrDefaultAsync(h => h.Id == id);
            if (hallazgo == null)
                return NotFound();

            bool descartar = decision == "descartar";
            if (descartar && string.IsNullOrWhiteSpace(comentario))
            {
                TempData["Error"] = "El comentario es obligatorio para descartar.";
                return RedirectToAction(nameof(Hallazgos), new { id });
            }

            foreach (var r in hallazgo.Recomendaciones)
            {
                r.EstadoRevision = descartar ? "Descartada" : "Aprobada";
                r.FechaRevision = DateTime.Now;
                r.Comentario = comentario?.Trim() ?? string.Empty;
                r.RevisadoPorUsuarioId = UsuarioActualId();
            }
            hallazgo.Estado = descartar ? "Descartado" : "Aprobado";

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Historial));
        }

        [Route("historial")]
        public async Task<IActionResult> Historial()
        {
            var decisiones = await _db.Recomendaciones
                .Include(r => r.Hallazgo)
                .Include(r => r.RevisadoPor).ThenInclude(u => u!.Rol)
                .Where(r => r.EstadoRevision != "Pendiente")
                .OrderByDescending(r => r.FechaRevision)
                .ToListAsync();
            return View(new HistorialViewModel { Decisiones = decisiones });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
