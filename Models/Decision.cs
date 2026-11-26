namespace SmartProcessAI.Models
{
    public class Decision
    {
        public int Id { get; set; }
        public int HallazgoId { get; set; }
        public Hallazgo? Hallazgo { get; set; }
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public string Accion { get; set; } = string.Empty;
        public string Comentario { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
