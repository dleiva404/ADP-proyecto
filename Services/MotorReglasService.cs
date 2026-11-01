namespace SmartProcessAI.Services;

// reglas programadas, estas no usan ML.NET
public class MotorReglasService
{
    private readonly decimal _umbralActivoFijo;

    // 1000 es un ejemplo, el umbral real lo define el auditor
    public MotorReglasService(decimal umbralActivoFijo = 1000m)
    {
        _umbralActivoFijo = umbralActivoFijo;
    }

    // R-01 y R-02: tipo de documento según el cliente
    public ResultadoRegla ValidarTipoDocumento(string tipoCliente, string tipoDocumento)
    {
        var cmp = StringComparison.OrdinalIgnoreCase;
        bool esContribuyente = tipoCliente.Equals("ContribuyenteIVA", cmp);
        bool esConsumidorFinal = tipoCliente.Equals("ConsumidorFinal", cmp);

        if (!esContribuyente && !esConsumidorFinal)
            return ResultadoRegla.Falla("Tipo de cliente no reconocido.", "R-00");

        if (esContribuyente && !tipoDocumento.Equals("CreditoFiscal", cmp))
            return ResultadoRegla.Falla(
                "A un contribuyente de IVA le corresponde un Comprobante de Crédito Fiscal.",
                "R-01");

        if (esConsumidorFinal && !tipoDocumento.Equals("Factura", cmp))
            return ResultadoRegla.Falla(
                "A un consumidor final le corresponde una Factura.",
                "R-02");

        return ResultadoRegla.Ok();
    }

    // R-03: compra de monto alto registrada como gasto
    public ResultadoRegla ValidarClasificacionCompra(Compra compra)
    {
        bool esGasto = compra.ClasificacionRegistrada
            .Equals("Gasto", StringComparison.OrdinalIgnoreCase);

        if (esGasto && compra.Monto >= _umbralActivoFijo)
            return ResultadoRegla.Falla(
                $"La compra '{compra.Descripcion}' (${compra.Monto:N2}) supera el umbral " +
                "de activo fijo y está registrada como gasto.",
                "R-03");

        return ResultadoRegla.Ok();
    }
}