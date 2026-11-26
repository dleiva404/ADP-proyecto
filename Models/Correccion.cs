namespace SmartProcessAI.Models
{
    public class Correccion
    {
        public int Id { get; set; }
        public int HallazgoId { get; set; }
        public Hallazgo? Hallazgo { get; set; }
        public int DocumentoId { get; set; }
        public Documento? Documento { get; set; }
        public int? VersionDocumentoId { get; set; }
        public VersionDocumento? VersionDocumento { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = EstadosCorreccion.PendienteAprobacion;
        public int CreadaPorUsuarioId { get; set; }
        public Usuario? CreadaPor { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public int? AprobadaPorUsuarioId { get; set; }
        public Usuario? AprobadaPor { get; set; }
        public DateTime? FechaAprobacion { get; set; }
    }
}

