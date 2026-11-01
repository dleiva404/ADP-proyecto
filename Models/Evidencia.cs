namespace SmartProcessAI.Models;

public class Evidencia
{
    public int Id { get; set; }
    public string NombreArchivo { get; set; } = string.Empty;
    public string Ruta { get; set; } = string.Empty;
    public DateTime FechaCarga { get; set; } = DateTime.Now;

    public int HallazgoId { get; set; }
    public Hallazgo? Hallazgo { get; set; }

    public int? TareaId { get; set; }
    public Tarea? Tarea { get; set; }

    public int UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
}
