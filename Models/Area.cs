namespace SmartProcessAI.Models
{
    public class Area
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public ICollection<Proceso> Procesos { get; set; } = new List<Proceso>();
    }
}