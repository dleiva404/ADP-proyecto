namespace SmartProcessAI.Services;

// resumen que devuelve el análisis de una auditoría
public class ResultadoAnalisis
{
    public int AuditoriaId { get; set; }
    public int TareasRevisadas { get; set; }
    public int HallazgosPorReglas { get; set; }
    public int TareasAutomatizables { get; set; }
    public int TareasAtipicas { get; set; }
    public int RecomendacionesGeneradas { get; set; }
}