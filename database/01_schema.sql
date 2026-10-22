IF DB_ID(N'ControlAcademico') IS NULL
    CREATE DATABASE ControlAcademico;
GO
USE ControlAcademico;
GO

CREATE TABLE Docentes (
    Id              INT IDENTITY(1,1) NOT NULL,
    Nombre          NVARCHAR(100)  NOT NULL,
    Apellido        NVARCHAR(100)  NOT NULL,
    Correo          NVARCHAR(150)  NOT NULL,
    Telefono        NVARCHAR(9)    NULL,
    FechaNacimiento DATETIME2      NOT NULL,
    CodigoEmpleado  NVARCHAR(20)   NOT NULL,
    Especialidad    NVARCHAR(100)  NOT NULL,
    CONSTRAINT PK_Docentes PRIMARY KEY (Id)
);
CREATE UNIQUE INDEX IX_Docentes_Correo         ON Docentes (Correo);
CREATE UNIQUE INDEX IX_Docentes_CodigoEmpleado ON Docentes (CodigoEmpleado);

CREATE TABLE Estudiantes (
    Id                INT IDENTITY(1,1) NOT NULL,
    Nombre            NVARCHAR(100)  NOT NULL,
    Apellido          NVARCHAR(100)  NOT NULL,
    Correo            NVARCHAR(150)  NOT NULL,
    Telefono          NVARCHAR(9)    NULL,
    FechaNacimiento   DATETIME2      NOT NULL,
    Carnet            NVARCHAR(20)   NOT NULL,
    FechaIngreso      DATETIME2      NOT NULL,
    Carrera           NVARCHAR(100)  NOT NULL,
    Facultad          NVARCHAR(100)  NOT NULL,
    CicloActual       INT            NOT NULL,
    CUM               DECIMAL(4,2)   NOT NULL,
    EstadoAcademico   NVARCHAR(30)   NOT NULL,
    CreditosAprobados INT            NOT NULL,
    CONSTRAINT PK_Estudiantes PRIMARY KEY (Id),
    CONSTRAINT CK_Estudiantes_CUM CHECK (CUM BETWEEN 0 AND 10)
);
CREATE UNIQUE INDEX IX_Estudiantes_Carnet ON Estudiantes (Carnet);
CREATE UNIQUE INDEX IX_Estudiantes_Correo ON Estudiantes (Correo);

CREATE TABLE Cursos (
    Id          INT IDENTITY(1,1) NOT NULL,
    Codigo      NVARCHAR(20)   NOT NULL,
    Nombre      NVARCHAR(150)  NOT NULL,
    Descripcion NVARCHAR(1000) NOT NULL,
    Creditos    INT            NOT NULL,
    CupoMaximo  INT            NOT NULL,
    DocenteId   INT            NOT NULL,
    CONSTRAINT PK_Cursos PRIMARY KEY (Id),
    CONSTRAINT FK_Cursos_Docentes_DocenteId FOREIGN KEY (DocenteId) REFERENCES Docentes (Id),
    CONSTRAINT CK_Cursos_Creditos   CHECK (Creditos BETWEEN 1 AND 6),
    CONSTRAINT CK_Cursos_CupoMaximo CHECK (CupoMaximo > 0)
);
CREATE UNIQUE INDEX IX_Cursos_Codigo    ON Cursos (Codigo);
CREATE UNIQUE INDEX IX_Cursos_Nombre    ON Cursos (Nombre);
CREATE        INDEX IX_Cursos_DocenteId ON Cursos (DocenteId);

CREATE TABLE Matriculas (
    Id              INT IDENTITY(1,1) NOT NULL,
    CodigoMatricula NVARCHAR(20) NULL,
    EstudianteId    INT          NOT NULL,
    CursoId         INT          NOT NULL,
    FechaMatricula  DATETIME2    NOT NULL,
    CONSTRAINT PK_Matriculas PRIMARY KEY (Id),
    CONSTRAINT FK_Matriculas_Estudiantes_EstudianteId FOREIGN KEY (EstudianteId) REFERENCES Estudiantes (Id),
    CONSTRAINT FK_Matriculas_Cursos_CursoId           FOREIGN KEY (CursoId)      REFERENCES Cursos (Id)
);
CREATE UNIQUE INDEX IX_Matriculas_EstudianteId_CursoId ON Matriculas (EstudianteId, CursoId);
CREATE UNIQUE INDEX IX_Matriculas_CodigoMatricula      ON Matriculas (CodigoMatricula) WHERE CodigoMatricula IS NOT NULL;
CREATE        INDEX IX_Matriculas_CursoId              ON Matriculas (CursoId);

CREATE TABLE Calificaciones (
    Id            INT IDENTITY(1,1) NOT NULL,
    Nota          FLOAT       NOT NULL,
    MatriculaId   INT         NOT NULL,
    Estado        NVARCHAR(20) NULL,
    FechaRegistro DATETIME2   NOT NULL CONSTRAINT DF_Calificaciones_FechaRegistro DEFAULT (GETDATE()),
    CONSTRAINT PK_Calificaciones PRIMARY KEY (Id),
    CONSTRAINT FK_Calificaciones_Matriculas_MatriculaId FOREIGN KEY (MatriculaId) REFERENCES Matriculas (Id),
    CONSTRAINT CK_Calificaciones_Nota CHECK (Nota BETWEEN 0 AND 10)
);
CREATE INDEX IX_Calificaciones_MatriculaId ON Calificaciones (MatriculaId);

CREATE TABLE Actividad (
    Id          INT IDENTITY(1,1) NOT NULL,
    Descripcion NVARCHAR(250) NOT NULL,
    Fecha       DATETIME2     NOT NULL CONSTRAINT DF_Actividad_Fecha DEFAULT (GETDATE()),
    CONSTRAINT PK_Actividad PRIMARY KEY (Id)
);
GO
