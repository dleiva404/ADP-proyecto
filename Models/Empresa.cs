namespace SmartProcessAI.Models;

public class Empresa
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Giro { get; set; } = string.Empty;

    public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    public ICollection<Proceso> Procesos { get; set; } = new List<Proceso>();
    public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
}
