namespace SmartProcessAI.Models;

public static class RolesSistema
{
    public const string Auditor = "Auditor";
    public const string Jefe = "Jefe de Auditoria Operativa";
    public const string Gerente = "Gerente de Auditoria Interna";
    public const string AdminTI = "Administrador de TI";
}
public static class EstadosHallazgo
{
    public const string Pendiente = "Pendiente";
    public const string EnRevisionGerente = "En revision del Gerente";
    public const string Aprobado = "Aprobado";
    public const string RechazadoPorJefe = "Rechazado por Jefe";
    public const string RechazadoPorGerente = "Rechazado por Gerente";
}
public static class EstadosAuditoria
{
    public const string Abierta = "Abierta";
    public const string Cerrada = "Cerrada";
    public const string EnModificacion = "En modificacion";
}
public static class NivelesDecision
{
    public const string Jefe = "Jefe";
    public const string Gerente = "Gerente";
}
public static class AccionesDecision
{
    public const string Aprobar = "Aprobar";
    public const string Rechazar = "Rechazar";
    public const string Reactivar = "Reactivar";
}
public static class EstadosCorreccion
{
    public const string PendienteAprobacion = "Pendiente de aprobacion";
    public const string Aprobada = "Aprobada";
    public const string Rechazada = "Rechazada";
}