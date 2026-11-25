USE master;
GO

IF DB_ID('smartprocess_ai') IS NOT NULL
BEGIN
    ALTER DATABASE smartprocess_ai SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE smartprocess_ai;
END
GO

CREATE DATABASE smartprocess_ai;
GO

USE smartprocess_ai;
GO

-- 1. Empresa
CREATE TABLE Empresa (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Giro NVARCHAR(150) NOT NULL
);

-- 1.1 Area
CREATE TABLE Area (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL UNIQUE
);

-- 2. Rol
CREATE TABLE Rol (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL
);

-- 3. Usuario
CREATE TABLE Usuario (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Correo NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RolId INT NOT NULL REFERENCES Rol(Id),
    EmpresaId INT NOT NULL REFERENCES Empresa(Id),
    Activo BIT NOT NULL DEFAULT 1
);

-- 4. Proceso
CREATE TABLE Proceso (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    AreaId INT NOT NULL REFERENCES Area(Id),
    Tipo NVARCHAR(30) NOT NULL,
    EmpresaId INT NOT NULL REFERENCES Empresa(Id)
);

-- 5. Tarea
CREATE TABLE Tarea (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Puesto NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(MAX) NOT NULL,
    TiempoMinutos INT NOT NULL,
    FrecuenciaMensual INT NOT NULL,
    NumeroPasos INT NOT NULL,
    Herramientas NVARCHAR(255) NOT NULL,
    EsManual BIT NOT NULL,
    ProcesoId INT NOT NULL REFERENCES Proceso(Id),
    AreaId INT NULL REFERENCES Area(Id)
);

-- 6. Regla
CREATE TABLE Regla (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Condicion NVARCHAR(MAX) NOT NULL,
    Mensaje NVARCHAR(MAX) NOT NULL,
    Referencia NVARCHAR(255) NOT NULL,
    Activa BIT NOT NULL
);

-- 7. Auditoria
CREATE TABLE Auditoria (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Estado NVARCHAR(20) NOT NULL CHECK (Estado IN
        ('Abierta','Cerrada','En modificacion')),
    Fecha DATETIME2 NOT NULL,
    EmpresaId INT NOT NULL REFERENCES Empresa(Id),
    ProcesoId INT NOT NULL REFERENCES Proceso(Id),
    AuditorId INT NOT NULL REFERENCES Usuario(Id)
);

-- 8. Hallazgo
CREATE TABLE Hallazgo (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion NVARCHAR(MAX) NOT NULL,
    Riesgo NVARCHAR(10) NOT NULL CHECK (Riesgo IN ('Bajo', 'Medio', 'Alto', 'Critico')),
    Fecha DATETIME2 NOT NULL,
    Estado NVARCHAR(30) NOT NULL CHECK (Estado IN
        ('Pendiente','En revision del Gerente','Aprobado',
         'Rechazado por Jefe','Rechazado por Gerente')),
    AuditoriaId INT NOT NULL REFERENCES Auditoria(Id),
    TareaId INT NULL REFERENCES Tarea(Id),
    ReglaId INT NULL REFERENCES Regla(Id),
    UsuarioId INT NOT NULL REFERENCES Usuario(Id),
    AreaId INT NULL REFERENCES Area(Id)
);

-- 9. Recomendacion
CREATE TABLE Recomendacion (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Descripcion NVARCHAR(MAX) NOT NULL,
    Prioridad NVARCHAR(10) NOT NULL,
    GeneradaPorIA BIT NOT NULL,
    TiempoAhorradoMin INT NOT NULL,
    EstadoRevision NVARCHAR(20) NOT NULL,
    FechaRevision DATETIME2 NULL,
    Comentario NVARCHAR(MAX) NOT NULL,
    HallazgoId INT NOT NULL REFERENCES Hallazgo(Id),
    RevisadoPorUsuarioId INT NULL REFERENCES Usuario(Id)
);

-- 10. Evidencia
CREATE TABLE Evidencia (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    NombreArchivo NVARCHAR(255) NOT NULL,
    Ruta NVARCHAR(500) NOT NULL,
    FechaCarga DATETIME2 NOT NULL,
    HallazgoId INT NOT NULL REFERENCES Hallazgo(Id),
    TareaId INT NULL REFERENCES Tarea(Id),
    UsuarioId INT NOT NULL REFERENCES Usuario(Id)
);

-- 11. Decision
CREATE TABLE Decision (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    HallazgoId INT NOT NULL REFERENCES Hallazgo(Id),
    UsuarioId INT NOT NULL REFERENCES Usuario(Id),
    Nivel NVARCHAR(20) NOT NULL CHECK (Nivel IN ('Jefe','Gerente')),
    Accion NVARCHAR(20) NOT NULL CHECK (Accion IN
        ('Aprobar','Rechazar','Reactivar')),
    Comentario NVARCHAR(MAX) NOT NULL,
    Fecha DATETIME2 NOT NULL
);

-- 12. Documento
CREATE TABLE Documento (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(255) NOT NULL,
    Tipo NVARCHAR(20) NOT NULL,
    AreaId INT NULL REFERENCES Area(Id),
    AuditoriaId INT NULL REFERENCES Auditoria(Id),
    SubidoPorUsuarioId INT NOT NULL REFERENCES Usuario(Id),
    FechaCarga DATETIME2 NOT NULL
);

-- 13. VersionDocumento
CREATE TABLE VersionDocumento (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DocumentoId INT NOT NULL REFERENCES Documento(Id),
    Numero INT NOT NULL,
    Ruta NVARCHAR(500) NOT NULL,
    HallazgoId INT NULL REFERENCES Hallazgo(Id),
    CreadaPorUsuarioId INT NOT NULL REFERENCES Usuario(Id),
    FechaCreacion DATETIME2 NOT NULL
);

-- 14. Correccion
CREATE TABLE Correccion (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    HallazgoId INT NOT NULL REFERENCES Hallazgo(Id),
    DocumentoId INT NOT NULL REFERENCES Documento(Id),
    VersionDocumentoId INT NULL REFERENCES VersionDocumento(Id),
    Descripcion NVARCHAR(MAX) NOT NULL,
    Estado NVARCHAR(30) NOT NULL CHECK (Estado IN
        ('Pendiente de aprobacion','Aprobada','Rechazada')),
    CreadaPorUsuarioId INT NOT NULL REFERENCES Usuario(Id),
    FechaCreacion DATETIME2 NOT NULL,
    AprobadaPorUsuarioId INT NULL REFERENCES Usuario(Id),
    FechaAprobacion DATETIME2 NULL
);
GO

-- Datos base
INSERT INTO Rol (Nombre) VALUES
    ('Auditor'),
    ('Jefe de Auditoria Operativa'),
    ('Gerente de Auditoria Interna'),
    ('Administrador de TI');

INSERT INTO Area (Nombre) VALUES
    ('Efectivo'),
    ('Inventario'),
    ('Administrativa'),
    ('Infraestructura'),
    ('Legal'),
    ('Prevencion de lavado de dinero y activos'),
    ('Cuentas por cobrar'),
    ('Flota vehicular');

INSERT INTO Empresa (Nombre, Giro) VALUES
    ('Inversiones Calma S.A. de C.V.', 'Grupo empresarial'),
    ('DIDELCO S.A. de C.V.', 'Tiendas'),
    ('Steel International Corporation S.A. de C.V.', 'Consultoria'),
    ('COPRO S.A. de C.V.', 'Constructora El Progreso'),
    ('Propiedades Ultramar S.A. de C.V.', 'Bienes y raices'),
    ('Depositos de Centro America S.A. de C.V.', 'DCA Bodegas'),
    ('ELF S.A. de C.V.', 'Division de transporte y logistica');

INSERT INTO Usuario (Nombre, Correo, PasswordHash, RolId, EmpresaId) VALUES
    ('Carlos Alberto Cornejo Calderon', 'ccornejo@inversionescalma.com', 'AQAAAAEAAYagAAAAEKGyw9Tl9gcYKTpLXG1+j5CZEO297zXlvzcWsVzklHLNso4iaR/4aEtFC2kNHZmrdg==', 4, 1),
    ('David Antonio Leiva Martinez', 'dleiva@inversionescalma.com', 'AQAAAAEAAYagAAAAEKGyw9Tl9gcYKTpLXG1+j5CZEO297zXlvzcWsVzklHLNso4iaR/4aEtFC2kNHZmrdg==', 2, 1),
    ('Moises David Garcia Casco', 'mgarcia@inversionescalma.com', 'AQAAAAEAAYagAAAAEKGyw9Tl9gcYKTpLXG1+j5CZEO297zXlvzcWsVzklHLNso4iaR/4aEtFC2kNHZmrdg==', 1, 1),
    ('Jose David Carabantes Hernandez', 'jcarabantes@inversionescalma.com', 'AQAAAAEAAYagAAAAEKGyw9Tl9gcYKTpLXG1+j5CZEO297zXlvzcWsVzklHLNso4iaR/4aEtFC2kNHZmrdg==', 1, 1),
    ('Fernando Antonio Lopez Paz', 'flopez@inversionescalma.com', 'AQAAAAEAAYagAAAAEKGyw9Tl9gcYKTpLXG1+j5CZEO297zXlvzcWsVzklHLNso4iaR/4aEtFC2kNHZmrdg==', 3, 1);

INSERT INTO Proceso (Nombre, AreaId, Tipo, EmpresaId) VALUES
    ('Cuentas por pagar', 3, 'Administrativo', 1),
    ('Ventas', 7, 'Operativo', 1),
    ('Tesoreria', 1, 'Administrativo', 1),
    ('Bodega', 2, 'Operativo', 1);

INSERT INTO Regla (Nombre, Condicion, Mensaje, Referencia, Activa) VALUES
    ('Tipo de documento segun cliente', 'Contribuyente de IVA con Credito Fiscal, consumidor final con Factura', 'El documento no corresponde al tipo de cliente.', 'R-01 / R-02', 1),
    ('Clasificacion de compra', 'Compra igual o mayor al umbral de activo fijo no debe registrarse como gasto', 'La compra supera el umbral de activo fijo y esta registrada como gasto.', 'R-03', 1);

INSERT INTO Auditoria (Estado, Fecha, EmpresaId, ProcesoId, AuditorId) VALUES
    ('Abierta', SYSDATETIME(), 1, 1, 4);
GO