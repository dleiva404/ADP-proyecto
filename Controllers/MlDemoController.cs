using Microsoft.AspNetCore.Mvc;
using SmartProcessAI.Models;
using SmartProcessAI.Services;

namespace SmartProcessAI.Controllers;

// controlador de prueba para ver ML.NET sin pantallas
public class MlDemoController : Controller
{
    private readonly AnalisisMLService _ml;

    public MlDemoController(AnalisisMLService ml)
    {
        _ml = ml;
    }

    public IActionResult Index()
    {
        var tarea = new Tarea
        {
            Id = 1,
            Puesto = "Auxiliar de inventario",
            Descripcion = "Conciliación de inventario físico contra el sistema",
            TiempoMinutos = 240,
            FrecuenciaMensual = 4,
            NumeroPasos = 9,
            Herramientas = "Excel, Sistema contable",
            EsManual = true
        };

        var prediccion = _ml.PredecirAutomatizable(tarea);
        var atipicas = _ml.DetectarTiemposAtipicos(new List<Tarea> { tarea });

        return Json(new
        {
            entrenamiento = _ml.ResumenEntrenamiento,
            esAutomatizable = prediccion.EsAutomatizable,
            probabilidad = prediccion.Probabilidad,
            esTiempoAtipico = atipicas[0].EsAtipica
        });
    }
}