using SmartProcessAI.Models;

namespace SmartProcessAI.Services;

// arma el hallazgo y la recomendación de una tarea usando ML.NET
public class AnalizadorTareaService
{
    private readonly AnalisisMLService _ml;

    public AnalizadorTareaService(AnalisisMLService ml)
    {
        _ml = ml;
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

        double porcentajeAhorro = prediccion.EsAutomatizable ? 0.8 : 0.3;
        int ahorroMin = (int)(minutosMes * porcentajeAhorro);

        string texto = prediccion.EsAutomatizable
            ? $"Automatizar esta tarea manual. La IA estima {prediccion.Probabilidad:P0} de probabilidad de que sea automatizable."
            : "Estandarizar y simplificar los pasos de esta tarea manual. La IA no la considera fácil de automatizar por completo.";

        if (atipica)
            texto += " Además, su tiempo de ejecución es atípico frente a tareas similares.";

        var hallazgo = new Hallazgo
        {
            Descripcion = tarea.Descripcion,
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
