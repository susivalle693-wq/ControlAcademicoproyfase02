/* =====================================================================
   Control Académico - Fase 2
   02_datos_prueba.sql : 3 docentes, 10 estudiantes, 5 cursos,
   15 matrículas, 12 calificaciones y 5 actividades.

   (Ojito, solo ejecutar una vez ;-;)
   ===================================================================== */
USE ControlAcademico;
GO

-- Docentes -------------------------------------------------------------
INSERT INTO Docentes (CodigoEmpleado, Nombre, Apellido, Correo, Telefono, FechaNacimiento, Especialidad) VALUES
('DOC001', N'Marta',   N'Rivas',   'marta.rivas@ejemplo.com',   '7100-0001', '1980-03-14', N'Programación'),
('DOC002', N'Ricardo', N'Mejía',   'ricardo.mejia@ejemplo.com', '7100-0002', '1975-09-02', N'Bases de datos'),
('DOC003', N'Patricia',N'Gómez',   'patricia.gomez@ejemplo.com','7100-0003', '1985-11-21', N'Ingeniería de software');

-- Estudiantes ----------------------------------------------------------
INSERT INTO Estudiantes (Carnet, Nombre, Apellido, Correo, Telefono, FechaNacimiento, FechaIngreso, Carrera, Facultad, CicloActual, CUM, EstadoAcademico, CreditosAprobados) VALUES
('AB253001', N'Carlos',  N'Hernández', 'carlos.hernandez@ejemplo.com', '7200-0001', '2004-01-15', '2025-02-10', N'Ingeniería en Sistemas', N'Ingeniería y Arquitectura', 3, 8.20, N'Activo',   48),
('AB253002', N'Lucía',   N'Martínez',  'lucia.martinez@ejemplo.com',   '7200-0002', '2004-05-30', '2025-02-10', N'Ingeniería en Sistemas', N'Ingeniería y Arquitectura', 3, 7.60, N'Activo',   44),
('AB253003', N'Diego',   N'Flores',    'diego.flores@ejemplo.com',     '7200-0003', '2003-11-08', '2025-02-10', N'Ingeniería en Sistemas', N'Ingeniería y Arquitectura', 4, 6.80, N'Activo',   60),
('AB253004', N'Sofía',   N'Ramírez',   'sofia.ramirez@ejemplo.com',    '7200-0004', '2004-07-19', '2025-02-10', N'Ingeniería en Sistemas', N'Ingeniería y Arquitectura', 2, 9.10, N'Activo',   32),
('AB253005', N'Andrés',  N'Castillo',  'andres.castillo@ejemplo.com',  '7200-0005', '2003-03-25', '2024-02-12', N'Ingeniería en Sistemas', N'Ingeniería y Arquitectura', 5, 7.40, N'Activo',   80),
('AB253006', N'Valeria', N'Ortiz',     'valeria.ortiz@ejemplo.com',    '7200-0006', '2004-09-12', '2025-02-10', N'Ingeniería Industrial',  N'Ingeniería y Arquitectura', 3, 8.60, N'Activo',   46),
('AB253007', N'Mateo',   N'Alvarado',  'mateo.alvarado@ejemplo.com',   '7200-0007', '2002-12-01', '2024-02-12', N'Ingeniería en Sistemas', N'Ingeniería y Arquitectura', 6, 5.90, N'Activo',   92),
('AB253008', N'Camila',  N'Pineda',    'camila.pineda@ejemplo.com',    '7200-0008', '2004-04-04', '2025-02-10', N'Ingeniería Industrial',  N'Ingeniería y Arquitectura', 2, 7.90, N'Activo',   30),
('AB253009', N'Daniel',  N'Cruz',      'daniel.cruz@ejemplo.com',      '7200-0009', '2001-06-17', '2023-02-13', N'Ingeniería en Sistemas', N'Ingeniería y Arquitectura', 9, 7.10, N'Inactivo', 150),
('AB253010', N'Isabella',N'Navarro',   'isabella.navarro@ejemplo.com', '7200-0010', '2003-08-28', '2024-02-12', N'Ingeniería Industrial',  N'Ingeniería y Arquitectura', 4, 8.00, N'Activo',   62);

-- Cursos ---------------------------------------------------------------
INSERT INTO Cursos (Codigo, Nombre, Descripcion, Creditos, CupoMaximo, DocenteId)
SELECT v.Codigo, v.Nombre, v.Descripcion, v.Creditos, v.CupoMaximo, d.Id
FROM (VALUES
    ('POO101', N'Programación Orientada a Objetos', N'Clases, herencia, polimorfismo e interfaces en C#.',        4, 30, 'DOC001'),
    ('BDD101', N'Bases de Datos I',                 N'Modelo relacional, SQL y diseño de bases de datos.',        4, 30, 'DOC002'),
    ('DAW201', N'Desarrollo de Aplicaciones Web',   N'ASP.NET Core MVC, formularios, validaciones y servicios.',  4, 25, 'DOC001'),
    ('EDD201', N'Estructuras de Datos',             N'Listas, pilas, colas, árboles y análisis de algoritmos.',   3, 30, 'DOC001'),
    ('ISW301', N'Ingeniería de Software',           N'Ciclo de vida, requisitos, diseño y pruebas de software.',  4, 20, 'DOC003')
) AS v (Codigo, Nombre, Descripcion, Creditos, CupoMaximo, CodigoDocente)
JOIN Docentes d ON d.CodigoEmpleado = v.CodigoDocente;
GO

-- Matrículas (15) ------------------------------------------------------
INSERT INTO Matriculas (CodigoMatricula, EstudianteId, CursoId, FechaMatricula)
SELECT v.CodigoMatricula, e.Id, c.Id, v.Fecha
FROM (VALUES
    ('MAT253-001', 'AB253001', 'POO101', '2026-08-03'),
    ('MAT253-002', 'AB253002', 'POO101', '2026-08-03'),
    ('MAT253-003', 'AB253003', 'POO101', '2026-08-04'),
    ('MAT253-004', 'AB253004', 'POO101', '2026-08-04'),
    ('MAT253-005', 'AB253001', 'BDD101', '2026-08-05'),
    ('MAT253-006', 'AB253005', 'BDD101', '2026-08-05'),
    ('MAT253-007', 'AB253006', 'BDD101', '2026-08-06'),
    ('MAT253-008', 'AB253002', 'DAW201', '2026-08-06'),
    ('MAT253-009', 'AB253007', 'DAW201', '2026-08-07'),
    ('MAT253-010', 'AB253008', 'DAW201', '2026-08-07'),
    ('MAT253-011', 'AB253003', 'EDD201', '2026-08-10'),
    ('MAT253-012', 'AB253009', 'EDD201', '2026-08-10'),
    ('MAT253-013', 'AB253010', 'ISW301', '2026-08-11'),
    ('MAT253-014', 'AB253004', 'ISW301', '2026-08-11'),
    ('MAT253-015', 'AB253005', 'ISW301', '2026-08-12')
) AS v (CodigoMatricula, Carnet, CodigoCurso, Fecha)
JOIN Estudiantes e ON e.Carnet = v.Carnet
JOIN Cursos c      ON c.Codigo = v.CodigoCurso;
GO

-- Calificaciones (12; las matrículas 013 a 015 quedan sin nota) -------
INSERT INTO Calificaciones (MatriculaId, Nota, Estado, FechaRegistro)
SELECT m.Id, v.Nota,
       CASE WHEN v.Nota >= 6.0 THEN N'Aprobado' ELSE N'Reprobado' END,
       v.Fecha
FROM (VALUES
    ('MAT253-001', 8.5, '2026-09-20'),
    ('MAT253-002', 7.0, '2026-09-20'),
    ('MAT253-003', 5.4, '2026-09-21'),
    ('MAT253-004', 9.2, '2026-09-21'),
    ('MAT253-005', 6.0, '2026-09-22'),
    ('MAT253-006', 7.8, '2026-09-22'),
    ('MAT253-007', 4.9, '2026-09-23'),
    ('MAT253-008', 8.0, '2026-09-23'),
    ('MAT253-009', 9.5, '2026-09-24'),
    ('MAT253-010', 6.5, '2026-09-24'),
    ('MAT253-011', 5.8, '2026-09-25'),
    ('MAT253-012', 7.2, '2026-09-25')
) AS v (CodigoMatricula, Nota, Fecha)
JOIN Matriculas m ON m.CodigoMatricula = v.CodigoMatricula;
GO

-- Actividad reciente ---------------------------------------------------
INSERT INTO Actividad (Descripcion, Fecha) VALUES
(N'Se registró el curso Ingeniería de Software',                  '2026-10-12 09:15'),
(N'Se matriculó al estudiante AB253010 en ISW301',                '2026-10-13 10:40'),
(N'Se registró la nota 7.2 al estudiante AB253009 en EDD201',     '2026-10-14 08:05'),
(N'Se actualizó el estudiante AB253007',                          '2026-10-15 14:30'),
(N'Se registró al docente Patricia Gómez',                        '2026-10-16 11:20');
GO
