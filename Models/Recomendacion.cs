namespace SmartProcessAI.Models;

public class Recomendacion
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Prioridad { get; set; } = string.Empty;
    public bool GeneradaPorIA { get; set; }
    public int TiempoAhorradoMin { get; set; }
    public string EstadoRevision { get; set; } = "Pendiente";
    public DateTime? FechaRevision { get; set; }
    public string Comentario { get; set; } = string.Empty;

    public int HallazgoId { get; set; }
    public Hallazgo? Hallazgo { get; set; }

    public int? RevisadoPorUsuarioId { get; set; }
    public Usuario? RevisadoPor { get; set; }
}
