namespace SmartProcessAI.ViewModels;

// funciones chicas para mostrar datos en las vistas
public static class Formato
{
    public static string ClaseRiesgo(string? riesgo)
    {
        return (riesgo ?? "").ToLower() switch
        {
            "critico" or "crítico" => "critico",
            "alto" => "alto",
            "medio" => "medio",
            _ => "bajo"
        };
    }

    public static string TextoRiesgo(string? riesgo)
    {
        return ClaseRiesgo(riesgo) switch
        {
            "critico" => "Crítico",
            "alto" => "Alto",
            "medio" => "Medio",
            _ => "Bajo"
        };
    }

    public static string ClaseEstado(string? estado)
    {
        return estado switch
        {
            "Aprobado" => "aprobado",
            "Descartado" => "descartado",
            _ => "pendiente"
        };
    }

    public static string Corto(string? texto, int max = 60)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return "";
        texto = texto.Trim();
        return texto.Length <= max ? texto : texto.Substring(0, max).TrimEnd() + "...";
    }

    // primer nombre y primer apellido (con 4 palabras: la 1 y la 3)
    public static string NombreCorto(string? nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            return "";
        var partes = nombre.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (partes.Length >= 4)
            return partes[0] + " " + partes[2];
        if (partes.Length >= 2)
            return partes[0] + " " + partes[1];
        return partes[0];
    }

    public static string Iniciales(string? nombre)
    {
        var corto = NombreCorto(nombre);
        if (corto == "")
            return "?";
        var partes = corto.Split(' ');
        string a = partes[0].Substring(0, 1);
        string b = partes.Length > 1 ? partes[1].Substring(0, 1) : "";
        return (a + b).ToUpper();
    }
}
