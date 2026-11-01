namespace SmartProcessAI.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public int RolId { get; set; }
    public Rol? Rol { get; set; }

    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }

    public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
    public ICollection<Hallazgo> Hallazgos { get; set; } = new List<Hallazgo>();
    public ICollection<Evidencia> Evidencias { get; set; } = new List<Evidencia>();
    public ICollection<Recomendacion> RecomendacionesRevisadas { get; set; } = new List<Recomendacion>();
}
