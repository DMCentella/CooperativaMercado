CREATE DATABASE CooperativaBD
go
USE CooperativaBD
GO
CREATE TABLE Socio (
    IdSocio INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    DNI NVARCHAR(20) NULL,
    Telefono NVARCHAR(20) NULL,
    Activo BIT NOT NULL DEFAULT 1,
    CONSTRAINT UQ_Socio_DNI UNIQUE (DNI)
);
GO

-- =========================
-- USUARIO
-- =========================
CREATE TABLE Usuario (
    IdUsuario INT IDENTITY PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Password NVARCHAR(100) NOT NULL,
    Rol NVARCHAR(20) NOT NULL CHECK (Rol IN ('Admin','Socio')),
    IdSocio INT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (IdSocio) REFERENCES Socio(IdSocio)
);
GO

-- =========================
-- PUESTO
-- =========================
CREATE TABLE Puesto (
    IdPuesto INT IDENTITY PRIMARY KEY,
    Numero NVARCHAR(10) NOT NULL UNIQUE,
    Metraje DECIMAL(5,2) NULL CHECK (Metraje >= 0),
    Ubicacion NVARCHAR(100) NULL,
    Giro NVARCHAR(50) NULL,
    MontoAlquiler DECIMAL(10,2) NOT NULL CHECK (MontoAlquiler >= 0),
    IdSocio INT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (IdSocio) REFERENCES Socio(IdSocio)
);
GO

-- =========================
-- TIPO DE DEUDA
-- =========================
CREATE TABLE TipoDeuda (
    IdTipoDeuda INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL UNIQUE,
    MontoBase DECIMAL(10,2) NULL CHECK (MontoBase >= 0),
    Activo BIT NOT NULL DEFAULT 1
);
GO

-- =========================
-- DEUDA
-- =========================
CREATE TABLE Deuda (
    IdDeuda INT IDENTITY PRIMARY KEY,
    IdPuesto INT NOT NULL,
    IdTipoDeuda INT NOT NULL,
    Descripcion NVARCHAR(100) NULL,
    Monto DECIMAL(10,2) NOT NULL CHECK (Monto > 0),
    Mes INT NOT NULL CHECK (Mes BETWEEN 1 AND 12),
    Anio INT NOT NULL CHECK (Anio >= 2020),
    FechaVencimiento DATE NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente' CHECK (Estado IN ('Pendiente','Pagado')),
    FOREIGN KEY (IdPuesto) REFERENCES Puesto(IdPuesto),
    FOREIGN KEY (IdTipoDeuda) REFERENCES TipoDeuda(IdTipoDeuda),
    CONSTRAINT UQ_Deuda UNIQUE (IdPuesto, IdTipoDeuda, Mes, Anio)
);
GO

-- =========================
-- PAGO
-- =========================
CREATE TABLE Pago (
    IdPago INT IDENTITY PRIMARY KEY,
    IdDeuda INT NOT NULL,
    Monto DECIMAL(10,2) NOT NULL CHECK (Monto > 0),
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    NumeroRecibo NVARCHAR(30) NOT NULL UNIQUE,
    MetodoPago NVARCHAR(20) NULL,
    FOREIGN KEY (IdDeuda) REFERENCES Deuda(IdDeuda)
);
GO

-- =========================
-- INGRESO DIARIO
-- =========================
CREATE TABLE IngresoDiario (
    IdIngreso INT IDENTITY PRIMARY KEY,
    IdPuesto INT NOT NULL,
    Fecha DATE NOT NULL,
    Monto DECIMAL(10,2) NOT NULL CHECK (Monto > 0),
    FechaRegistro DATETIME NOT NULL DEFAULT GETDATE(),
    IdUsuario INT NOT NULL,
    FOREIGN KEY (IdPuesto) REFERENCES Puesto(IdPuesto),
    FOREIGN KEY (IdUsuario) REFERENCES Usuario(IdUsuario),
    CONSTRAINT UQ_Ingreso UNIQUE (IdPuesto, Fecha)
);
GO
/* =========================================================
   AJUSTE POR NO USAR LOGIN
   ========================================================= */
ALTER TABLE IngresoDiario
ALTER COLUMN IdUsuario INT NULL;
GO

/* =========================================================
   SOCIO
   ========================================================= */

CREATE OR ALTER PROCEDURE sp_RegistrarSocio
    @Nombre NVARCHAR(100),
    @DNI NVARCHAR(20) = NULL,
    @Telefono NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = ''
    BEGIN
        RAISERROR('El nombre es obligatorio.', 16, 1);
        RETURN;
    END

    IF @DNI IS NOT NULL AND EXISTS (SELECT 1 FROM Socio WHERE DNI = @DNI)
    BEGIN
        RAISERROR('Ya existe un socio con ese DNI.', 16, 1);
        RETURN;
    END

    INSERT INTO Socio (Nombre, DNI, Telefono, Activo)
    VALUES (@Nombre, @DNI, @Telefono, 1);
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarSocio
    @IdSocio INT,
    @Nombre NVARCHAR(100),
    @DNI NVARCHAR(20) = NULL,
    @Telefono NVARCHAR(20) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Socio WHERE IdSocio = @IdSocio)
    BEGIN
        RAISERROR('El socio no existe.', 16, 1);
        RETURN;
    END

    IF @Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = ''
    BEGIN
        RAISERROR('El nombre es obligatorio.', 16, 1);
        RETURN;
    END

    IF @DNI IS NOT NULL AND EXISTS (
        SELECT 1 FROM Socio
        WHERE DNI = @DNI AND IdSocio <> @IdSocio
    )
    BEGIN
        RAISERROR('Ya existe otro socio con ese DNI.', 16, 1);
        RETURN;
    END

    UPDATE Socio
    SET Nombre = @Nombre,
        DNI = @DNI,
        Telefono = @Telefono
    WHERE IdSocio = @IdSocio;
END
GO

CREATE OR ALTER PROCEDURE sp_DesactivarSocio
    @IdSocio INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Socio WHERE IdSocio = @IdSocio)
    BEGIN
        RAISERROR('El socio no existe.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Puesto WHERE IdSocio = @IdSocio)
    BEGIN
        RAISERROR('No se puede desactivar porque tiene puestos asignados.', 16, 1);
        RETURN;
    END

    UPDATE Socio
    SET Activo = 0
    WHERE IdSocio = @IdSocio;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarSocios
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Socio;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerSocioPorId
    @IdSocio INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Socio WHERE IdSocio = @IdSocio;
END
GO

/* =========================================================
   PUESTO
   ========================================================= */

CREATE OR ALTER PROCEDURE sp_RegistrarPuesto
    @Numero NVARCHAR(10),
    @Metraje DECIMAL(5,2) = NULL,
    @Ubicacion NVARCHAR(100) = NULL,
    @Giro NVARCHAR(50) = NULL,
    @MontoAlquiler DECIMAL(10,2),
    @IdSocio INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Numero IS NULL OR LTRIM(RTRIM(@Numero)) = ''
    BEGIN
        RAISERROR('El número de puesto es obligatorio.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Puesto WHERE Numero = @Numero)
    BEGIN
        RAISERROR('Ya existe un puesto con ese número.', 16, 1);
        RETURN;
    END

    IF @MontoAlquiler < 0
    BEGIN
        RAISERROR('El monto de alquiler no puede ser negativo.', 16, 1);
        RETURN;
    END

    IF @IdSocio IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Socio WHERE IdSocio = @IdSocio AND Activo = 1)
    BEGIN
        RAISERROR('El socio asignado no existe o está inactivo.', 16, 1);
        RETURN;
    END

    INSERT INTO Puesto (Numero, Metraje, Ubicacion, Giro, MontoAlquiler, IdSocio, Activo)
    VALUES (@Numero, @Metraje, @Ubicacion, @Giro, @MontoAlquiler, @IdSocio, 1);
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarPuesto
    @IdPuesto INT,
    @Numero NVARCHAR(10),
    @Metraje DECIMAL(5,2) = NULL,
    @Ubicacion NVARCHAR(100) = NULL,
    @Giro NVARCHAR(50) = NULL,
    @MontoAlquiler DECIMAL(10,2),
    @IdSocio INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Puesto WHERE IdPuesto = @IdPuesto)
    BEGIN
        RAISERROR('El puesto no existe.', 16, 1);
        RETURN;
    END

    IF @Numero IS NULL OR LTRIM(RTRIM(@Numero)) = ''
    BEGIN
        RAISERROR('El número de puesto es obligatorio.', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1 FROM Puesto
        WHERE Numero = @Numero AND IdPuesto <> @IdPuesto
    )
    BEGIN
        RAISERROR('Ya existe otro puesto con ese número.', 16, 1);
        RETURN;
    END

    IF @MontoAlquiler < 0
    BEGIN
        RAISERROR('El monto de alquiler no puede ser negativo.', 16, 1);
        RETURN;
    END

    IF @IdSocio IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Socio WHERE IdSocio = @IdSocio AND Activo = 1)
    BEGIN
        RAISERROR('El socio asignado no existe o está inactivo.', 16, 1);
        RETURN;
    END

    UPDATE Puesto
    SET Numero = @Numero,
        Metraje = @Metraje,
        Ubicacion = @Ubicacion,
        Giro = @Giro,
        MontoAlquiler = @MontoAlquiler,
        IdSocio = @IdSocio
    WHERE IdPuesto = @IdPuesto;
END
GO

CREATE OR ALTER PROCEDURE sp_AsociarPuesto
    @IdPuesto INT,
    @IdSocio INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Puesto WHERE IdPuesto = @IdPuesto)
    BEGIN
        RAISERROR('El puesto no existe.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM Socio WHERE IdSocio = @IdSocio AND Activo = 1)
    BEGIN
        RAISERROR('El socio no existe o está inactivo.', 16, 1);
        RETURN;
    END

    UPDATE Puesto
    SET IdSocio = @IdSocio
    WHERE IdPuesto = @IdPuesto;
END
GO

CREATE OR ALTER PROCEDURE sp_DesasociarPuesto
    @IdPuesto INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Puesto WHERE IdPuesto = @IdPuesto)
    BEGIN
        RAISERROR('El puesto no existe.', 16, 1);
        RETURN;
    END

    UPDATE Puesto
    SET IdSocio = NULL
    WHERE IdPuesto = @IdPuesto;
END
GO

CREATE OR ALTER PROCEDURE sp_DesactivarPuesto
    @IdPuesto INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Puesto WHERE IdPuesto = @IdPuesto)
    BEGIN
        RAISERROR('El puesto no existe.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Deuda WHERE IdPuesto = @IdPuesto)
    BEGIN
        RAISERROR('No se puede desactivar porque tiene deudas registradas.', 16, 1);
        RETURN;
    END

    UPDATE Puesto
    SET Activo = 0
    WHERE IdPuesto = @IdPuesto;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarPuestos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Puesto;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerPuestoPorId
    @IdPuesto INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Puesto WHERE IdPuesto = @IdPuesto;
END
GO

/* =========================================================
   TIPO DE DEUDA
   ========================================================= */

CREATE OR ALTER PROCEDURE sp_RegistrarTipoDeuda
    @Nombre NVARCHAR(50),
    @MontoBase DECIMAL(10,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = ''
    BEGIN
        RAISERROR('El nombre del tipo de deuda es obligatorio.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM TipoDeuda WHERE Nombre = @Nombre)
    BEGIN
        RAISERROR('Ya existe ese tipo de deuda.', 16, 1);
        RETURN;
    END

    INSERT INTO TipoDeuda (Nombre, MontoBase, Activo)
    VALUES (@Nombre, @MontoBase, 1);
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarTipoDeuda
    @IdTipoDeuda INT,
    @Nombre NVARCHAR(50),
    @MontoBase DECIMAL(10,2) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM TipoDeuda WHERE IdTipoDeuda = @IdTipoDeuda)
    BEGIN
        RAISERROR('El tipo de deuda no existe.', 16, 1);
        RETURN;
    END

    IF @Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = ''
    BEGIN
        RAISERROR('El nombre del tipo de deuda es obligatorio.', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1 FROM TipoDeuda
        WHERE Nombre = @Nombre AND IdTipoDeuda <> @IdTipoDeuda
    )
    BEGIN
        RAISERROR('Ya existe otro tipo de deuda con ese nombre.', 16, 1);
        RETURN;
    END

    UPDATE TipoDeuda
    SET Nombre = @Nombre,
        MontoBase = @MontoBase
    WHERE IdTipoDeuda = @IdTipoDeuda;
END
GO

CREATE OR ALTER PROCEDURE sp_DesactivarTipoDeuda
    @IdTipoDeuda INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM TipoDeuda WHERE IdTipoDeuda = @IdTipoDeuda)
    BEGIN
        RAISERROR('El tipo de deuda no existe.', 16, 1);
        RETURN;
    END

    IF EXISTS (SELECT 1 FROM Deuda WHERE IdTipoDeuda = @IdTipoDeuda)
    BEGIN
        RAISERROR('No se puede desactivar porque ya tiene deudas asociadas.', 16, 1);
        RETURN;
    END

    UPDATE TipoDeuda
    SET Activo = 0
    WHERE IdTipoDeuda = @IdTipoDeuda;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarTipoDeuda
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM TipoDeuda;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerTipoDeuda
    @IdTipoDeuda INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM TipoDeuda WHERE IdTipoDeuda = @IdTipoDeuda;
END
GO

/* =========================================================
   DEUDA
   ========================================================= */

CREATE OR ALTER PROCEDURE sp_CrearDeuda
    @IdPuesto INT,
    @IdTipoDeuda INT,
    @Descripcion NVARCHAR(100) = NULL,
    @Monto DECIMAL(10,2),
    @Mes INT,
    @Anio INT,
    @FechaVencimiento DATE = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Puesto WHERE IdPuesto = @IdPuesto AND Activo = 1)
    BEGIN
        RAISERROR('El puesto no existe o está inactivo.', 16, 1);
        RETURN;
    END

    IF NOT EXISTS (SELECT 1 FROM TipoDeuda WHERE IdTipoDeuda = @IdTipoDeuda AND Activo = 1)
    BEGIN
        RAISERROR('El tipo de deuda no existe o está inactivo.', 16, 1);
        RETURN;
    END

    IF @Monto <= 0
    BEGIN
        RAISERROR('El monto debe ser mayor a cero.', 16, 1);
        RETURN;
    END

    IF @Mes NOT BETWEEN 1 AND 12
    BEGIN
        RAISERROR('El mes debe estar entre 1 y 12.', 16, 1);
        RETURN;
    END

    IF @Anio < 2020
    BEGIN
        RAISERROR('El año no es válido.', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM Deuda
        WHERE IdPuesto = @IdPuesto
          AND IdTipoDeuda = @IdTipoDeuda
          AND Mes = @Mes
          AND Anio = @Anio
    )
    BEGIN
        RAISERROR('Ya existe esta deuda para ese periodo.', 16, 1);
        RETURN;
    END

    INSERT INTO Deuda (IdPuesto, IdTipoDeuda, Descripcion, Monto, Mes, Anio, FechaVencimiento, Estado)
    VALUES (@IdPuesto, @IdTipoDeuda, @Descripcion, @Monto, @Mes, @Anio, @FechaVencimiento, 'Pendiente');
END
GO

CREATE OR ALTER PROCEDURE sp_ActualizarDeuda
    @IdDeuda INT,
    @Descripcion NVARCHAR(100) = NULL,
    @Monto DECIMAL(10,2),
    @FechaVencimiento DATE = NULL,
    @Estado NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM Deuda WHERE IdDeuda = @IdDeuda)
    BEGIN
        RAISERROR('La deuda no existe.', 16, 1);
        RETURN;
    END

    IF @Monto <= 0
    BEGIN
        RAISERROR('El monto debe ser mayor a cero.', 16, 1);
        RETURN;
    END

    IF @Estado NOT IN ('Pendiente','Pagado')
    BEGIN
        RAISERROR('Estado no válido.', 16, 1);
        RETURN;
    END

    UPDATE Deuda
    SET Descripcion = @Descripcion,
        Monto = @Monto,
        FechaVencimiento = @FechaVencimiento,
        Estado = @Estado
    WHERE IdDeuda = @IdDeuda;
END
GO

CREATE OR ALTER PROCEDURE sp_GenerarAlquilerMensual
    @Mes INT,
    @Anio INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TipoAlquiler INT;

    SELECT @TipoAlquiler = IdTipoDeuda
    FROM TipoDeuda
    WHERE Nombre = 'Alquiler' AND Activo = 1;

    IF @TipoAlquiler IS NULL
    BEGIN
        RAISERROR('No existe el tipo de deuda Alquiler.', 16, 1);
        RETURN;
    END

    INSERT INTO Deuda (IdPuesto, IdTipoDeuda, Descripcion, Monto, Mes, Anio, Estado)
    SELECT 
        p.IdPuesto,
        @TipoAlquiler,
        'Alquiler mensual',
        p.MontoAlquiler,
        @Mes,
        @Anio,
        'Pendiente'
    FROM Puesto p
    WHERE p.Activo = 1
      AND p.IdSocio IS NOT NULL
      AND NOT EXISTS (
            SELECT 1
            FROM Deuda d
            WHERE d.IdPuesto = p.IdPuesto
              AND d.IdTipoDeuda = @TipoAlquiler
              AND d.Mes = @Mes
              AND d.Anio = @Anio
      );
END
GO

CREATE OR ALTER PROCEDURE sp_ListarDeudas
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Deuda;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerDeudaPorId
    @IdDeuda INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Deuda WHERE IdDeuda = @IdDeuda;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerDeudasPorPuesto
    @IdPuesto INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Deuda WHERE IdPuesto = @IdPuesto;
END
GO

CREATE OR ALTER PROCEDURE sp_DeudasPendientes
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Deuda WHERE Estado = 'Pendiente';
END
GO

/* =========================================================
   PAGO
   ========================================================= */

CREATE OR ALTER PROCEDURE sp_RegistrarPago
    @IdDeuda INT,
    @Monto DECIMAL(10,2),
    @MetodoPago NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @MontoDeuda DECIMAL(10,2);
    DECLARE @Estado NVARCHAR(20);
    DECLARE @NumeroRecibo NVARCHAR(20);

    SELECT 
        @MontoDeuda = Monto,
        @Estado = Estado
    FROM Deuda
    WHERE IdDeuda = @IdDeuda;

    IF @MontoDeuda IS NULL
    BEGIN
        RAISERROR('La deuda no existe.', 16, 1);
        RETURN;
    END

    IF @Estado = 'Pagado'
    BEGIN
        RAISERROR('La deuda ya está pagada.', 16, 1);
        RETURN;
    END

    IF @Monto <> @MontoDeuda
    BEGIN
        RAISERROR('El monto debe ser exacto.', 16, 1);
        RETURN;
    END

    SET @NumeroRecibo = 'REC-' + CAST(@IdDeuda AS NVARCHAR);

    INSERT INTO Pago (IdDeuda, Monto, Fecha, NumeroRecibo, MetodoPago)
    VALUES (@IdDeuda, @Monto, GETDATE(), @NumeroRecibo, @MetodoPago);

    UPDATE Deuda
    SET Estado = 'Pagado'
    WHERE IdDeuda = @IdDeuda;
END
GO

CREATE OR ALTER PROCEDURE sp_ListarPagos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Pago;
END
GO

CREATE OR ALTER PROCEDURE sp_ObtenerPagosPorDeuda
    @IdDeuda INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM Pago WHERE IdDeuda = @IdDeuda;
END
GO

/* =========================================================
   INGRESO DIARIO
   ========================================================= */

CREATE OR ALTER PROCEDURE sp_RegistrarIngreso
    @IdPuesto INT,
    @Fecha DATE,
    @Monto DECIMAL(10,2),
    @IdUsuario INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM Puesto
        WHERE IdPuesto = @IdPuesto
          AND Activo = 1
    )
    BEGIN
        RAISERROR('El puesto no existe o está inactivo.', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM IngresoDiario
        WHERE IdPuesto = @IdPuesto
          AND Fecha = @Fecha
    )
    BEGIN
        RAISERROR('Ya existe un ingreso registrado para este puesto en esa fecha.', 16, 1);
        RETURN;
    END

    INSERT INTO IngresoDiario (IdPuesto, Fecha, Monto, IdUsuario)
    VALUES (@IdPuesto, @Fecha, @Monto, @IdUsuario);
END
GO

CREATE OR ALTER PROCEDURE sp_ListarIngresos
AS
BEGIN
    SET NOCOUNT ON;
    SELECT IdIngreso, IdPuesto, Fecha, Monto, IdUsuario
    FROM IngresoDiario;
END
GO

/* =========================================================
   REPORTES
   ========================================================= */

CREATE OR ALTER PROCEDURE sp_ReporteDeudasPendientes
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        d.IdDeuda,
        p.Numero AS Puesto,
        td.Nombre AS TipoDeuda,
        d.Descripcion,
        d.Monto,
        d.Mes,
        d.Anio,
        d.FechaVencimiento,
        d.Estado
    FROM Deuda d
    INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto
    INNER JOIN TipoDeuda td ON d.IdTipoDeuda = td.IdTipoDeuda
    WHERE d.Estado = 'Pendiente';
END
GO

CREATE OR ALTER PROCEDURE sp_ReporteDeudasPagadas
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        d.IdDeuda,
        p.Numero AS Puesto,
        s.Nombre AS Socio,
        d.Monto,
        d.Mes,
        d.Anio,
        d.Estado
    FROM Deuda d
    INNER JOIN Puesto p ON d.IdPuesto = p.IdPuesto
    LEFT JOIN Socio s ON p.IdSocio = s.IdSocio
    WHERE d.Estado = 'Pagado';
END
GO

CREATE OR ALTER PROCEDURE sp_ReporteIngresosDiarios
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        i.IdIngreso,
        p.Numero AS Puesto,
        i.Fecha,
        i.Monto,
        i.FechaRegistro,
        u.Username
    FROM IngresoDiario i
    INNER JOIN Puesto p ON i.IdPuesto = p.IdPuesto
    LEFT JOIN Usuario u ON i.IdUsuario = u.IdUsuario
    ORDER BY i.Fecha DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_ReporteIngresosPorFecha
    @Fecha DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.Numero AS Puesto,
        i.Monto,
        i.Fecha
    FROM IngresoDiario i
    INNER JOIN Puesto p ON i.IdPuesto = p.IdPuesto
    WHERE i.Fecha = @Fecha;
END
GO

CREATE OR ALTER PROCEDURE sp_ReporteRecaudadoPorRango
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ISNULL(SUM(Monto), 0) AS TotalRecaudado
    FROM Pago
    WHERE Fecha >= @FechaInicio
      AND Fecha < DATEADD(DAY, 1, @FechaFin);
END
GO

CREATE OR ALTER PROCEDURE sp_ReporteIngresosPorRango
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        ISNULL(SUM(i.Monto), 0) AS TotalIngresos
    FROM IngresoDiario i
    WHERE i.Fecha >= @FechaInicio
      AND i.Fecha < DATEADD(DAY, 1, @FechaFin);
END
GO

CREATE OR ALTER PROCEDURE sp_ReporteIngresosPorPuesto
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        p.Numero AS Puesto,
        ISNULL(SUM(i.Monto), 0) AS TotalIngresos
    FROM Puesto p
    LEFT JOIN IngresoDiario i 
        ON p.IdPuesto = i.IdPuesto
        AND i.Fecha >= @FechaInicio
        AND i.Fecha < DATEADD(DAY, 1, @FechaFin)
    GROUP BY p.Numero
    ORDER BY TotalIngresos DESC;
END
GO

CREATE OR ALTER PROCEDURE sp_ReporteMensual
    @Mes INT,
    @Anio INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        td.Nombre AS Concepto,
        COUNT(*) AS CantidadDeudas,
        ISNULL(SUM(d.Monto), 0) AS Total
    FROM Deuda d
    INNER JOIN TipoDeuda td ON d.IdTipoDeuda = td.IdTipoDeuda
    WHERE d.Mes = @Mes
      AND d.Anio = @Anio
    GROUP BY td.Nombre;
END
GO

CREATE OR ALTER PROCEDURE sp_ReporteIngresos
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        COUNT(*) AS CantidadPagos,
        ISNULL(SUM(Monto), 0) AS TotalRecaudado
    FROM Pago
    WHERE Fecha >= @FechaInicio
      AND Fecha < DATEADD(DAY, 1, @FechaFin);
END
GO

/* =========================================================
   DATOS INICIALES
   ========================================================= */

INSERT INTO TipoDeuda (Nombre, MontoBase, Activo)
VALUES 
('Alquiler', NULL, 1),
('Luz', 50, 1),
('Agua', 30, 1),
('Vigilancia', 20, 1),
('Mora', NULL, 1);
GO

INSERT INTO Socio (Nombre, DNI, Telefono) VALUES
('Thiago Quispe','70123456','912345678'),
('Valeria Huamán','70234567','923456789'),
('Bruno Tacuri','70345678','934567890'),
('Camila Chura','70456789','945678901'),
('Mateo Mamani','70567890','956789012'),
('Alondra Choque','70678901','967890123'),
('Sebastián Pari','70789012','978901234'),
('Luciana Condori','70890123','989012345'),
('Gael Huanca','70901234','990123456'),
('Antonella Yupanqui','71012345','901234567'),
('Ian Apaza','71123456','912345679'),
('Danna Vilca','71234567','923456780'),
('Axel Cusi','71345678','934567891'),
('Mía Calla','71456789','945678902'),
('Emiliano Suca','71567890','956789013'),
('Renata Tito','71678901','967890124'),
('Santiago Quenta','71789012','978901235'),
('Valentina Lazo','71890123','989012346'),
('Nicolás Poma','71901234','990123457'),
('Fernanda Saire','72012345','901234568');

GO
INSERT INTO Puesto (Numero, Metraje, Ubicacion, Giro, MontoAlquiler, IdSocio) VALUES
('P1',10,'Zona Norte','Comida',250,1),
('P2',12,'Zona Norte','Ropa',260,2),
('P3',8,'Zona Este','Tecnología',300,3),
('P4',15,'Zona Sur','Carnes',320,4),
('P5',9,'Zona Oeste','Frutas',200,5),
('P6',11,'Centro','Verduras',210,6),
('P7',14,'Centro','Lácteos',280,7),
('P8',10,'Centro','Bebidas',190,8),
('P9',13,'Centro','Panadería',230,9),
('P10',7,'Centro','Juguetes',180,10),
('P11',10,'Zona Norte','Zapatos',240,11),
('P12',12,'Zona Norte','Accesorios',250,12),
('P13',8,'Zona Este','Celulares',310,13),
('P14',15,'Zona Sur','Electrónica',350,14),
('P15',9,'Zona Oeste','Hierbas',190,15),
('P16',11,'Centro','Pescados',270,16),
('P17',14,'Centro','Abarrotes',260,17),
('P18',10,'Centro','Snacks',200,18),
('P19',13,'Centro','Helados',220,19),
('P20',7,'Centro','Dulces',180,20);
GO

INSERT INTO Deuda (IdPuesto, IdTipoDeuda, Descripcion, Monto, Mes, Anio, Estado) VALUES
(1,1,'Alquiler Abril',250,4,2026,'Pendiente'),
(2,1,'Alquiler Abril',260,4,2026,'Pendiente'),
(3,1,'Alquiler Abril',300,4,2026,'Pagado'),
(4,1,'Alquiler Abril',320,4,2026,'Pendiente'),
(5,1,'Alquiler Abril',200,4,2026,'Pagado'),
(6,2,'Luz Abril',80,4,2026,'Pendiente'),
(7,2,'Luz Abril',90,4,2026,'Pagado'),
(8,3,'Agua Abril',60,4,2026,'Pendiente'),
(9,3,'Agua Abril',70,4,2026,'Pagado'),
(10,1,'Alquiler Abril',180,4,2026,'Pendiente'),
(11,1,'Alquiler Abril',240,4,2026,'Pagado'),
(12,2,'Luz Abril',85,4,2026,'Pendiente'),
(13,3,'Agua Abril',65,4,2026,'Pendiente'),
(14,1,'Alquiler Abril',350,4,2026,'Pendiente'),
(15,1,'Alquiler Abril',190,4,2026,'Pagado'),
(16,2,'Luz Abril',95,4,2026,'Pendiente'),
(17,3,'Agua Abril',75,4,2026,'Pagado'),
(18,1,'Alquiler Abril',200,4,2026,'Pendiente'),
(19,1,'Alquiler Abril',220,4,2026,'Pagado'),
(20,1,'Alquiler Abril',180,4,2026,'Pendiente');

INSERT INTO Pago (IdDeuda, Monto, NumeroRecibo, MetodoPago) VALUES
(3,300,'REC-3','Efectivo'),
(5,200,'REC-5','Yape'),
(7,90,'REC-7','Efectivo'),
(9,70,'REC-9','Transferencia'),
(11,240,'REC-11','Efectivo'),
(15,190,'REC-15','Yape'),
(17,75,'REC-17','Transferencia'),
(19,220,'REC-19','Efectivo');

INSERT INTO IngresoDiario (IdPuesto, Fecha, Monto) VALUES
(1,'2026-04-01',500),
(2,'2026-04-01',450),
(3,'2026-04-01',600),
(4,'2026-04-01',700),
(5,'2026-04-01',300),
(6,'2026-04-02',350),
(7,'2026-04-02',800),
(8,'2026-04-02',420),
(9,'2026-04-02',390),
(10,'2026-04-02',310),
(11,'2026-04-03',520),
(12,'2026-04-03',480),
(13,'2026-04-03',610),
(14,'2026-04-03',900),
(15,'2026-04-03',330),
(16,'2026-04-04',760),
(17,'2026-04-04',680),
(18,'2026-04-04',410),
(19,'2026-04-04',370),
(20,'2026-04-04',290);