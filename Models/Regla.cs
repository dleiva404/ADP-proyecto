namespace SmartProcessAI.Models;

public class Regla
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Condicion { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Referencia { get; set; } = string.Empty;
    public bool Activa { get; set; } = true;

    public ICollection<Hallazgo> Hallazgos { get; set; } = new List<Hallazgo>();
}
