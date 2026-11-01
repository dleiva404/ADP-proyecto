namespace SmartProcessAI.Models;

public class Proceso
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Area { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;

    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }

    public ICollection<Tarea> Tareas { get; set; } = new List<Tarea>();
    public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
}
