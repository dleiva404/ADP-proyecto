using SmartProcessAI.Models;

namespace SmartProcessAI.ViewModels;

public class PanelViewModel
{
    public int Auditorias { get; set; }
    public int Abiertos { get; set; }
    public int Criticos { get; set; }
    public int PorRevisar { get; set; }
    public int Bajo { get; set; }
    public int Medio { get; set; }
    public int Alto { get; set; }
    public int Critico { get; set; }
    public List<Hallazgo> Recientes { get; set; } = new();
}

public class TareaFormViewModel
{
    public string? Puesto { get; set; }
    public int ProcesoId { get; set; }
    public string? Descripcion { get; set; }
    public int TiempoMinutos { get; set; }
    public int FrecuenciaMensual { get; set; }
    public int NumeroPasos { get; set; }
    public string? Herramientas { get; set; }
    public bool EsManual { get; set; } = true;
    public List<Proceso> Procesos { get; set; } = new();
    public List<string> Errores { get; set; } = new();
}

public class HallazgosViewModel
{
    public List<Hallazgo> Lista { get; set; } = new();
    public Hallazgo? Seleccionado { get; set; }
}

public class HistorialViewModel
{
    public List<Recomendacion> Decisiones { get; set; } = new();
}
