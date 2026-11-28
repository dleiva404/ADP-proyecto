namespace SmartProcessAI.Services;

public record PlantillaRecomendacion(string Hallazgo, string Recomendacion, double PorcentajeAhorro);

public static class CatalogoRecomendaciones
{
    private static readonly Dictionary<(string Area, string Tipo), PlantillaRecomendacion> PorAreaYTipo = new()
    {
        [("Efectivo", "Captura manual")] = new PlantillaRecomendacion(
            "Los ingresos de caja se capturan a mano, lo que expone a errores de digitación y a registros fuera de tiempo.",
            "Registrar depósitos y recibos desde una plantilla validada o con carga automática desde el reporte del banco, validando montos y fechas al ingresar.",
            0.80),
        [("Efectivo", "Conciliacion manual")] = new PlantillaRecomendacion(
            "La conciliación bancaria se hace a mano entre el estado de cuenta y el registro contable, con riesgo de partidas sin identificar.",
            "Automatizar el cruce de movimientos bancarios contra contabilidad por monto y fecha, y listar solo las partidas sin pareja para su revisión.",
            0.60),
        [("Efectivo", "Control sin evidencia")] = new PlantillaRecomendacion(
            "Los arqueos y las salidas de efectivo no dejan evidencia firmada del conteo ni de la autorización.",
            "Estandarizar un formato de arqueo con conteo por denominación, firma de quien cuenta y de quien revisa, y archivarlo digitalmente en cada cierre.",
            0.30),
        [("Efectivo", "Reporte manual")] = new PlantillaRecomendacion(
            "El reporte de flujo de efectivo se arma manualmente, lo que retrasa su entrega y puede generar diferencias entre versiones.",
            "Generar el reporte de flujo desde una única fuente de datos con una plantilla fija, de modo que se actualice sin reconstruirlo cada vez.",
            0.75),
        [("Efectivo", "Seguimiento manual")] = new PlantillaRecomendacion(
            "El seguimiento de cheques y remesas pendientes depende de revisiones manuales, y algunos pendientes pueden quedar sin atender.",
            "Mantener un tablero de pendientes con fecha límite y alertas automáticas por antigüedad para que ningún cheque o remesa quede sin cierre.",
            0.65),
        [("Inventario", "Captura manual")] = new PlantillaRecomendacion(
            "Las entradas y salidas de bodega se digitan manualmente, con riesgo de diferencias entre el movimiento físico y el sistema.",
            "Registrar los movimientos con lectura de códigos de barras o carga desde el documento de entrega, validando las cantidades contra la orden.",
            0.80),
        [("Inventario", "Conciliacion manual")] = new PlantillaRecomendacion(
            "El conteo físico se compara a mano contra el sistema, lo que alarga el cierre y deja diferencias sin explicar.",
            "Cargar el conteo en un archivo estándar y cruzarlo automáticamente con el kárdex, destacando solo los artículos con diferencia.",
            0.60),
        [("Inventario", "Control sin evidencia")] = new PlantillaRecomendacion(
            "Los ajustes de inventario y las mermas se aplican sin respaldo firmado ni autorización documentada.",
            "Exigir un formato de ajuste con motivo, evidencia y firma de autorización antes de afectar el inventario, y conservar la copia digital.",
            0.30),
        [("Inventario", "Reporte manual")] = new PlantillaRecomendacion(
            "El reporte de existencias y rotación se prepara manualmente en cada periodo.",
            "Generar el reporte de existencias, rotación y productos sin movimiento desde el sistema con una plantilla fija, y programar su emisión periódica.",
            0.75),
        [("Inventario", "Seguimiento manual")] = new PlantillaRecomendacion(
            "La mercadería en tránsito y los pedidos pendientes se controlan con listas manuales.",
            "Centralizar los pedidos en un registro con estado y fecha esperada de llegada, con alertas por retrasos.",
            0.65),
        [("Administrativa", "Captura manual")] = new PlantillaRecomendacion(
            "Las facturas de proveedores se digitan a mano, con riesgo de errores de monto, de proveedor y de duplicados.",
            "Capturar las facturas leyendo los datos del documento electrónico y validar automáticamente duplicados y proveedor registrado.",
            0.80),
        [("Administrativa", "Conciliacion manual")] = new PlantillaRecomendacion(
            "Las cuentas por pagar se concilian a mano contra los estados de cuenta de los proveedores.",
            "Cruzar automáticamente facturas, órdenes de compra y estados de proveedores, y revisar solo las diferencias.",
            0.60),
        [("Administrativa", "Control sin evidencia")] = new PlantillaRecomendacion(
            "Las autorizaciones de gasto y de pago no siempre quedan respaldadas con firma o aprobación registrada.",
            "Implementar un flujo de aprobación por niveles de monto, con registro de quién aprobó y cuándo, antes de liberar el pago.",
            0.30),
        [("Administrativa", "Reporte manual")] = new PlantillaRecomendacion(
            "El reporte mensual de gastos se arma manualmente y su preparación consume tiempo que podría dedicarse al análisis.",
            "Automatizar la consolidación de gastos por categoría y centro de costo desde el sistema, y compararla contra el presupuesto.",
            0.75),
        [("Administrativa", "Seguimiento manual")] = new PlantillaRecomendacion(
            "Los pagos pendientes y las facturas por vencer se vigilan con listas manuales.",
            "Programar alertas por vencimiento de facturas y un calendario de pagos que priorice lo próximo a vencer.",
            0.65),
        [("Infraestructura", "Captura manual")] = new PlantillaRecomendacion(
            "Las órdenes de trabajo y la bitácora de mantenimiento se registran manualmente, con información incompleta o tardía.",
            "Registrar las órdenes de trabajo en un formulario con campos obligatorios y fecha automática, que alimente la bitácora.",
            0.80),
        [("Infraestructura", "Conciliacion manual")] = new PlantillaRecomendacion(
            "El inventario de equipos e instalaciones se concilia a mano contra el registro de activos.",
            "Cruzar el inventario físico con el registro de activos mediante una carga estándar y reportar los equipos sin registro o sin ubicación.",
            0.60),
        [("Infraestructura", "Control sin evidencia")] = new PlantillaRecomendacion(
            "Las inspecciones y recepciones de mantenimiento no dejan evidencia fotográfica ni acta firmada.",
            "Estandarizar un acta de inspección con lista de verificación, fotografías y firma del responsable, archivada por equipo.",
            0.30),
        [("Infraestructura", "Reporte manual")] = new PlantillaRecomendacion(
            "El informe de estado de equipos y de costos de mantenimiento se prepara de forma manual.",
            "Generar el informe desde las órdenes de trabajo registradas, con costos y tiempos de atención por equipo.",
            0.75),
        [("Infraestructura", "Seguimiento manual")] = new PlantillaRecomendacion(
            "Las órdenes abiertas y los mantenimientos preventivos se siguen con controles manuales, lo que puede dejar atrasos sin detectar.",
            "Programar los mantenimientos preventivos con recordatorios y un tablero de órdenes abiertas por antigüedad.",
            0.65),
        [("Legal", "Captura manual")] = new PlantillaRecomendacion(
            "Los contratos, poderes y vigencias se registran a mano en hojas de cálculo.",
            "Mantener un registro central de contratos con campos de vigencia, partes y responsable, validando las fechas al ingresar.",
            0.80),
        [("Legal", "Conciliacion manual")] = new PlantillaRecomendacion(
            "Los documentos legales se cotejan manualmente contra el expediente y contra el registro de operaciones.",
            "Cruzar automáticamente la lista de contratos y permisos con las operaciones vigentes para detectar documentos faltantes.",
            0.60),
        [("Legal", "Control sin evidencia")] = new PlantillaRecomendacion(
            "La vigencia de permisos y el cumplimiento normativo se verifican sin dejar copia o evidencia archivada.",
            "Archivar cada permiso o contrato firmado en el expediente digital, con fecha de revisión y responsable, y verificar la vigencia con una lista de control.",
            0.30),
        [("Legal", "Reporte manual")] = new PlantillaRecomendacion(
            "El reporte de vencimientos y trámites se arma manualmente.",
            "Generar de forma automática el reporte de vencimientos próximos y trámites en curso desde el registro central.",
            0.75),
        [("Legal", "Seguimiento manual")] = new PlantillaRecomendacion(
            "Los trámites y renovaciones se vigilan con controles manuales, con riesgo de vencimientos no atendidos a tiempo.",
            "Programar alertas escalonadas antes de cada vencimiento y asignar un responsable visible por trámite.",
            0.65),
        [("Prevencion de lavado de dinero y activos", "Captura manual")] = new PlantillaRecomendacion(
            "Los datos de los clientes para el expediente se capturan manualmente, con riesgo de campos incompletos.",
            "Usar un formulario único de conocimiento del cliente con campos obligatorios y validación de documentos antes de guardarlo.",
            0.80),
        [("Prevencion de lavado de dinero y activos", "Conciliacion manual")] = new PlantillaRecomendacion(
            "Las transacciones se comparan manualmente contra el perfil del cliente y contra las listas internas.",
            "Automatizar la comparación de transacciones contra el perfil y los umbrales definidos, y revisar solo las desviaciones.",
            0.60),
        [("Prevencion de lavado de dinero y activos", "Control sin evidencia")] = new PlantillaRecomendacion(
            "La consulta de listas restrictivas y la validación de clientes no siempre deja registro de la consulta.",
            "Registrar automáticamente cada consulta de listas con fecha, usuario y resultado, y conservarla en el expediente del cliente.",
            0.30),
        [("Prevencion de lavado de dinero y activos", "Reporte manual")] = new PlantillaRecomendacion(
            "El reporte de operaciones inusuales se elabora de forma manual.",
            "Generar el reporte de operaciones inusuales desde reglas de detección, con la información requerida precargada para el oficial de cumplimiento.",
            0.75),
        [("Prevencion de lavado de dinero y activos", "Seguimiento manual")] = new PlantillaRecomendacion(
            "Las alertas y los expedientes por actualizar se siguen con listas manuales.",
            "Gestionar las alertas en un tablero con estado, responsable y plazo, y recordar la actualización periódica de los expedientes.",
            0.65),
        [("Cuentas por cobrar", "Captura manual")] = new PlantillaRecomendacion(
            "Los cobros y abonos se registran manualmente, con riesgo de aplicarlos a la factura o al cliente equivocado.",
            "Aplicar los cobros con sugerencia automática de la factura correspondiente y validación de montos.",
            0.80),
        [("Cuentas por cobrar", "Conciliacion manual")] = new PlantillaRecomendacion(
            "Los saldos con clientes se concilian a mano contra el auxiliar y el mayor.",
            "Cruzar automáticamente el auxiliar de cuentas por cobrar con el mayor y con los cobros aplicados, y listar las diferencias.",
            0.60),
        [("Cuentas por cobrar", "Control sin evidencia")] = new PlantillaRecomendacion(
            "Los documentos de crédito y las notas de crédito se autorizan sin respaldo o firma documentada.",
            "Exigir autorización registrada por nivel para notas de crédito y ampliaciones de crédito, con respaldo digital.",
            0.30),
        [("Cuentas por cobrar", "Reporte manual")] = new PlantillaRecomendacion(
            "El reporte de antigüedad de saldos y de cartera vencida se prepara a mano.",
            "Generar el reporte de antigüedad de saldos desde el sistema con una plantilla fija y emisión periódica.",
            0.75),
        [("Cuentas por cobrar", "Seguimiento manual")] = new PlantillaRecomendacion(
            "Los cobros vencidos y las promesas de pago se gestionan con controles manuales.",
            "Automatizar recordatorios de cobro por antigüedad y registrar cada gestión y promesa de pago con fecha y responsable.",
            0.65),
        [("Flota vehicular", "Captura manual")] = new PlantillaRecomendacion(
            "El combustible y las bitácoras de viaje se registran manualmente, con datos incompletos o inconsistentes.",
            "Registrar combustible y viajes en un formulario con campos obligatorios por unidad, o cargar directamente los datos de la tarjeta de combustible.",
            0.80),
        [("Flota vehicular", "Conciliacion manual")] = new PlantillaRecomendacion(
            "El kilometraje se concilia a mano contra el consumo de combustible y las facturas.",
            "Cruzar automáticamente kilometraje, litros y facturas por unidad, y resaltar los rendimientos fuera de rango.",
            0.60),
        [("Flota vehicular", "Control sin evidencia")] = new PlantillaRecomendacion(
            "Los documentos de los vehículos, las rutas y los siniestros no siempre tienen copia o acta archivada.",
            "Mantener un expediente digital por vehículo con los documentos vigentes, y exigir acta firmada para cada siniestro o desvío de ruta.",
            0.30),
        [("Flota vehicular", "Reporte manual")] = new PlantillaRecomendacion(
            "El informe de rendimiento y de costos de flota se arma manualmente.",
            "Generar el informe de rendimiento, kilometraje y costo por unidad desde los registros, con una plantilla fija.",
            0.75),
        [("Flota vehicular", "Seguimiento manual")] = new PlantillaRecomendacion(
            "Los mantenimientos programados y los vencimientos de seguros y tarjetas de circulación se vigilan manualmente.",
            "Programar alertas por kilometraje y por fecha para mantenimientos y vencimientos, con un tablero por unidad.",
            0.65),
    };

    private static readonly Dictionary<string, PlantillaRecomendacion> PorTipo = new()
    {
        ["Captura manual"] = new PlantillaRecomendacion(
            "Esta tarea captura datos manualmente, con riesgo de errores de digitación y de registros fuera de tiempo.",
            "Automatizar la captura con una plantilla validada o carga desde el documento de origen, con validación de datos al ingresar.",
            0.80),
        ["Conciliacion manual"] = new PlantillaRecomendacion(
            "Esta tarea concilia información entre dos fuentes de forma manual, con riesgo de diferencias sin explicar.",
            "Automatizar el cruce entre ambas fuentes y revisar solo las diferencias.",
            0.60),
        ["Control sin evidencia"] = new PlantillaRecomendacion(
            "Este control se ejecuta sin dejar evidencia o respaldo firmado.",
            "Estandarizar un formato con firma de quien ejecuta y de quien revisa, y archivarlo digitalmente.",
            0.30),
        ["Reporte manual"] = new PlantillaRecomendacion(
            "Este reporte se elabora manualmente, lo que consume tiempo y puede generar diferencias entre versiones.",
            "Generar el reporte desde una única fuente de datos con una plantilla fija.",
            0.75),
        ["Seguimiento manual"] = new PlantillaRecomendacion(
            "Este seguimiento depende de revisiones manuales, y algunos pendientes pueden quedar sin atender.",
            "Mantener un tablero de pendientes con fecha límite y alertas automáticas.",
            0.65),
    };

    private static readonly PlantillaRecomendacion General = new(
        "Esta tarea manual puede optimizarse.",
        "Estandarizar y simplificar los pasos de la tarea.",
        0.30);

    public static PlantillaRecomendacion Obtener(string? area, string? tipo)
    {
        if (area != null && tipo != null && PorAreaYTipo.TryGetValue((area, tipo), out var plantilla))
            return plantilla;
        if (tipo != null && PorTipo.TryGetValue(tipo, out var generica))
            return generica;
        return General;
    }
}