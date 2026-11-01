namespace SmartProcessAI.Models;

public class Tarea
{
    public int Id { get; set; }
    public string Puesto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int TiempoMinutos { get; set; }
    public int FrecuenciaMensual { get; set; }
    public int NumeroPasos { get; set; }
    public string Herramientas { get; set; } = string.Empty;
    public bool EsManual { get; set; }

    public int ProcesoId { get; set; }
    public Proceso? Proceso { get; set; }

    public ICollection<Hallazgo> Hallazgos { get; set; } = new List<Hallazgo>();
    public ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();
}
