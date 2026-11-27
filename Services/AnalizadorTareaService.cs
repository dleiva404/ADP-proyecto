using SmartProcessAI.Datos;
using SmartProcessAI.Models;

namespace SmartProcessAI.Services;

// arma el hallazgo y la recomendación de una tarea usando ML.NET
public class AnalizadorTareaService
{
    private readonly AnalisisMLService _ml;
    private readonly AppDbContext _db;

    public AnalizadorTareaService(AnalisisMLService ml, AppDbContext db)
    {
        _ml = ml;
        _db = db;
    }

    private string? NombreArea(Tarea tarea)
    {
        if (tarea.Area != null)
            return tarea.Area.Nombre;
        if (tarea.Proceso?.Area != null)
            return tarea.Proceso.Area.Nombre;
        return _db.Procesos
            .Where(p => p.Id == tarea.ProcesoId)
            .Select(p => p.Area!.Nombre)
            .FirstOrDefault();
    }

    // devuelve el hallazgo con su recomendación, o null si la tarea no es manual
    public Hallazgo? AnalizarTarea(Tarea tarea, Auditoria auditoria, int usuarioId)
    {
        if (!tarea.EsManual)
            return null;

        var prediccion = _ml.PredecirAutomatizable(tarea);
        bool atipica = _ml.DetectarTiemposAtipicos(new List<Tarea> { tarea })[0].EsAtipica;

        int minutosMes = tarea.TiempoMinutos * tarea.FrecuenciaMensual;
        double horasMes = minutosMes / 60.0;

        string riesgo = horasMes >= 40 ? "Critico"
                      : horasMes >= 20 ? "Alto"
                      : horasMes >= 8 ? "Medio"
                      : "Bajo";

        // un tiempo atípico sube el riesgo mínimo a Medio
        if (atipica && riesgo == "Bajo")
            riesgo = "Medio";

        string? area = NombreArea(tarea);
        var (tipo, confianza) = _ml.PredecirTipoProblema(tarea);
        var plantilla = CatalogoRecomendaciones.Obtener(area, tipo);

        double porcentajeAhorro = prediccion.EsAutomatizable
            ? plantilla.PorcentajeAhorro
            : Math.Min(plantilla.PorcentajeAhorro, 0.3);
        int ahorroMin = (int)(minutosMes * porcentajeAhorro);

        string texto = plantilla.Recomendacion;
        texto += prediccion.EsAutomatizable
            ? $" La IA estima {prediccion.Probabilidad:P0} de probabilidad de que sea automatizable."
            : " La IA no la considera fácil de automatizar por completo, por lo que conviene estandarizar primero.";
        texto += $" Tipo de problema detectado: {tipo} ({confianza:P0} de confianza).";

        double? promedio = _ml.PromedioTiempo(area, tipo);
        if (promedio is > 0)
        {
            double diferencia = (tarea.TiempoMinutos - promedio.Value) / promedio.Value;
            string sentido = diferencia >= 0 ? "por encima" : "por debajo";
            texto += $" Su tiempo ({tarea.TiempoMinutos} min) está {Math.Abs(diferencia):P0} {sentido} del promedio de tareas similares del área ({promedio.Value:F0} min).";
        }

        if (atipica)
            texto += " Además, su tiempo de ejecución es atípico frente a tareas similares.";

        var hallazgo = new Hallazgo
        {
            Descripcion = tarea.Descripcion.TrimEnd('.', ' ') + ". " + plantilla.Hallazgo,
            Riesgo = riesgo,
            Fecha = DateTime.Now,
            Estado = "Pendiente",
            Auditoria = auditoria,
            Tarea = tarea,
            UsuarioId = usuarioId
        };

        hallazgo.Recomendaciones.Add(new Recomendacion
        {
            Descripcion = texto,
            Prioridad = riesgo,
            GeneradaPorIA = true,
            TiempoAhorradoMin = ahorroMin,
            EstadoRevision = "Pendiente",
            Comentario = string.Empty
        });

        return hallazgo;
    }
}
