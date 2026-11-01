using SmartProcessAI.Models;

namespace SmartProcessAI.Services;

// validaciones V01 a V04 de una tarea
public static class ValidadorTarea
{
    public static List<string> Validar(Tarea tarea)
    {
        var errores = new List<string>();

        if (string.IsNullOrWhiteSpace(tarea.Puesto))
            errores.Add("El puesto es obligatorio.");

        if (string.IsNullOrWhiteSpace(tarea.Descripcion))
            errores.Add("La descripción es obligatoria.");

        if (tarea.ProcesoId <= 0)
            errores.Add("Debe seleccionar un proceso.");

        if (tarea.TiempoMinutos <= 0)
            errores.Add("El tiempo debe ser mayor que cero.");

        if (tarea.FrecuenciaMensual < 1)
            errores.Add("La frecuencia mensual debe ser de al menos 1.");

        if (tarea.NumeroPasos < 1)
            errores.Add("El número de pasos debe ser de al menos 1.");

        return errores;
    }
}