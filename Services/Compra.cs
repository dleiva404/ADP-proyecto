namespace SmartProcessAI.Services;

// datos de una compra para revisar (no va a la base de datos)
public class Compra
{
    public string Descripcion { get; set; } = string.Empty;
    public decimal Monto { get; set; }

    // Inventario, Gasto o ActivoFijo
    public string ClasificacionRegistrada { get; set; } = string.Empty;
}