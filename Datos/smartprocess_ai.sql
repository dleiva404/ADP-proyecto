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
    EmpresaId INT NOT NULL REFERENCES Empresa(Id)
);

-- 4. Proceso
CREATE TABLE Proceso (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(150) NOT NULL,
    Area NVARCHAR(100) NOT NULL,
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
    ProcesoId INT NOT NULL REFERENCES Proceso(Id)
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
    Estado NVARCHAR(20) NOT NULL,
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
    Estado NVARCHAR(20) NOT NULL,
    AuditoriaId INT NOT NULL REFERENCES Auditoria(Id),
    TareaId INT NULL REFERENCES Tarea(Id),
    ReglaId INT NULL REFERENCES Regla(Id),
    UsuarioId INT NOT NULL REFERENCES Usuario(Id)
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
GO

-- Datos base
INSERT INTO Rol (Nombre) VALUES
    ('Auditor'),
    ('Jefatura'),
    ('Gerencia'),
    ('Area auditada'),
    ('Administrador');

INSERT INTO Empresa (Nombre, Giro) VALUES
    ('Inversiones Calma S.A. de C.V.', 'Grupo empresarial'),
    ('DIDELCO S.A. de C.V.', 'Tiendas'),
    ('Steel International Corporation S.A. de C.V.', 'Consultoria'),
    ('COPRO S.A. de C.V.', 'Constructora El Progreso'),
    ('Propiedades Ultramar S.A. de C.V.', 'Bienes y raices'),
    ('Depositos de Centro America S.A. de C.V.', 'DCA Bodegas'),
    ('ELF S.A. de C.V.', 'Division de transporte y logistica');

INSERT INTO Usuario (Nombre, Correo, PasswordHash, RolId, EmpresaId) VALUES
    ('Carlos Alberto Cornejo Calderon', 'ccornejo@inversionescalma.com', 'AQAAAAEAAYagAAAAEKGyw9Tl9gcYKTpLXG1+j5CZEO297zXlvzcWsVzklHLNso4iaR/4aEtFC2kNHZmrdg==', 5, 1),
    ('David Antonio Leiva Martinez', 'dleiva@inversionescalma.com', 'AQAAAAEAAYagAAAAEKGyw9Tl9gcYKTpLXG1+j5CZEO297zXlvzcWsVzklHLNso4iaR/4aEtFC2kNHZmrdg==', 2, 1),
    ('Moises David Garcia Casco', 'mgarcia@inversionescalma.com', 'AQAAAAEAAYagAAAAEKGyw9Tl9gcYKTpLXG1+j5CZEO297zXlvzcWsVzklHLNso4iaR/4aEtFC2kNHZmrdg==', 4, 1),
    ('Jose David Carabantes Hernandez', 'jcarabantes@inversionescalma.com', 'AQAAAAEAAYagAAAAEKGyw9Tl9gcYKTpLXG1+j5CZEO297zXlvzcWsVzklHLNso4iaR/4aEtFC2kNHZmrdg==', 1, 1),
    ('Fernando Antonio Lopez Paz', 'flopez@inversionescalma.com', 'AQAAAAEAAYagAAAAEKGyw9Tl9gcYKTpLXG1+j5CZEO297zXlvzcWsVzklHLNso4iaR/4aEtFC2kNHZmrdg==', 3, 1);

INSERT INTO Proceso (Nombre, Area, Tipo, EmpresaId) VALUES
    ('Cuentas por pagar', 'Contabilidad', 'Administrativo', 1),
    ('Ventas', 'Comercial', 'Operativo', 1),
    ('Tesoreria', 'Finanzas', 'Administrativo', 1),
    ('Bodega', 'Inventarios', 'Operativo', 1);

INSERT INTO Regla (Nombre, Condicion, Mensaje, Referencia, Activa) VALUES
    ('Tipo de documento segun cliente', 'Contribuyente de IVA con Credito Fiscal, consumidor final con Factura', 'El documento no corresponde al tipo de cliente.', 'R-01 / R-02', 1),
    ('Clasificacion de compra', 'Compra igual o mayor al umbral de activo fijo no debe registrarse como gasto', 'La compra supera el umbral de activo fijo y esta registrada como gasto.', 'R-03', 1);

INSERT INTO Auditoria (Estado, Fecha, EmpresaId, ProcesoId, AuditorId) VALUES
    ('Abierta', SYSDATETIME(), 1, 1, 4);
GO