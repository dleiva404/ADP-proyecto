namespace SmartProcessAI.Models
{
    public class VersionDocumento
    {
        public int Id { get; set; }
        public int DocumentoId { get; set; }
        public Documento? Documento { get; set; }
        public int Numero { get; set; }
        public string Ruta { get; set; } = string.Empty;
        public int? HallazgoId { get; set; }
        public Hallazgo? Hallazgo { get; set; }
        public int CreadaPorUsuarioId { get; set; }
        public Usuario? CreadaPor { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
