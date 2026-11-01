namespace SmartProcessAI.Services;

// resultado de revisar una regla
public class ResultadoRegla
{
    public bool Cumple { get; set; }
    public string Mensaje { get; set; } = string.Empty;
    public string Referencia { get; set; } = string.Empty;

    public static ResultadoRegla Ok() => new() { Cumple = true };

    public static ResultadoRegla Falla(string mensaje, string referencia = "") =>
        new() { Cumple = false, Mensaje = mensaje, Referencia = referencia };
}