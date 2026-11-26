namespace SmartProcessAI.Models
{
    public class Documento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int? AreaId { get; set; }
        public Area? Area { get; set; }
        public int? AuditoriaId { get; set; }
        public Auditoria? Auditoria { get; set; }
        public int SubidoPorUsuarioId { get; set; }
        public Usuario? SubidoPor { get; set; }
        public DateTime FechaCarga { get; set; } = DateTime.Now;
        public ICollection<VersionDocumento> Versiones { get; set; } = new List<VersionDocumento>();
    }
}