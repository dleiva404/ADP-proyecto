namespace SmartProcessAI.Models;

public class Auditoria
{
    public int Id { get; set; }
    public string Estado { get; set; } = "Abierta";
    public DateTime Fecha { get; set; } = DateTime.Now;

    public int EmpresaId { get; set; }
    public Empresa? Empresa { get; set; }

    public int ProcesoId { get; set; }
    public Proceso? Proceso { get; set; }

    public int AuditorId { get; set; }
    public Usuario? Auditor { get; set; }

    public ICollection<Hallazgo> Hallazgos { get; set; } = new List<Hallazgo>();
}
