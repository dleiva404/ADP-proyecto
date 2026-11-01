namespace SmartProcessAI.Models;

public class Hallazgo
{
    public int Id { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string Riesgo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Estado { get; set; } = "Pendiente";

    public int AuditoriaId { get; set; }
    public Auditoria? Auditoria { get; set; }

    public int? TareaId { get; set; }
    public Tarea? Tarea { get; set; }

    public int? ReglaId { get; set; }
    public Regla? Regla { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public ICollection<Recomendacion> Recomendaciones { get; set; } = new List<Recomendacion>();
    public ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();
}
