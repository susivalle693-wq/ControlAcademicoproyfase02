using ControlAcademico.Datos;
using Microsoft.EntityFrameworkCore;

namespace ControlAcademico.Negocio
{
    // ---- Respuestas del servicio web (lo que se convierte en JSON) ----

    public class CursoNotaDto
    {
        public string CodigoCurso { get; set; } = string.Empty;
        public string NombreCurso { get; set; } = string.Empty;
        public int Creditos { get; set; }
        public DateTime FechaMatricula { get; set; }
        public double? Nota { get; set; }
        public string Estado { get; set; } = string.Empty;
    }

    public class NotasEstudianteDto
    {
        public int EstudianteId { get; set; }
        public string Carnet { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Carrera { get; set; } = string.Empty;
        public string EstadoAcademico { get; set; } = string.Empty;
        public double? Promedio { get; set; }
        public int Aprobados { get; set; }
        public int Reprobados { get; set; }
        public int Pendientes { get; set; }
        public List<CursoNotaDto> Cursos { get; set; } = new();
    }

    public class CursoMatriculadoDto
    {
        public string? CodigoMatricula { get; set; }
        public string CodigoCurso { get; set; } = string.Empty;
        public string NombreCurso { get; set; } = string.Empty;
        public string Docente { get; set; } = string.Empty;
        public DateTime FechaMatricula { get; set; }
    }

    public class MatriculaEstudianteDto
    {
        public int EstudianteId { get; set; }
        public string Carnet { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string EstadoAcademico { get; set; } = string.Empty;
        public bool Matriculado { get; set; }
        public int TotalCursos { get; set; }
        public List<CursoMatriculadoDto> Cursos { get; set; } = new();
    }

    // ---- Servicio de Negocio que usa el controlador de la Web API ----

    public interface IConsultaEstudianteServicio
    {
        Task<NotasEstudianteDto?> ObtenerNotasAsync(string carnet);
        Task<MatriculaEstudianteDto?> ObtenerMatriculaAsync(string carnet);
    }

    public class ConsultaEstudianteServicio : IConsultaEstudianteServicio
    {
        private readonly ControlAcademicoContext _contexto;

        public ConsultaEstudianteServicio(ControlAcademicoContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<NotasEstudianteDto?> ObtenerNotasAsync(string carnet)
        {
            var estudiante = await BuscarAsync(carnet);
            if (estudiante == null) return null;

            var matriculas = await _contexto.Matriculas.AsNoTracking()
                .Where(m => m.EstudianteId == estudiante.Id)
                .Include(m => m.Curso)
                .Include(m => m.Calificaciones)
                .ToListAsync();

            var cursos = matriculas
                .OrderBy(m => m.Curso!.Nombre)
                .Select(m =>
                {
                    // Si una matrícula tuviera varias notas, se toma la más reciente.
                    var calificacion = m.Calificaciones.OrderByDescending(c => c.Id).FirstOrDefault();
                    double? nota = calificacion?.Nota;
                    return new CursoNotaDto
                    {
                        CodigoCurso = m.Curso!.Codigo,
                        NombreCurso = m.Curso.Nombre,
                        Creditos = m.Curso.Creditos,
                        FechaMatricula = m.FechaMatricula,
                        Nota = nota,
                        Estado = ReglasAcademicas.ObtenerEstado(nota)
                    };
                })
                .ToList();

            return new NotasEstudianteDto
            {
                EstudianteId = estudiante.Id,
                Carnet = estudiante.Carnet,
                NombreCompleto = estudiante.NombreCompleto,
                Correo = estudiante.Correo,
                Carrera = estudiante.Carrera,
                EstadoAcademico = estudiante.EstadoAcademico,
                Promedio = ReglasAcademicas.CalcularPromedio(
                    cursos.Where(c => c.Nota.HasValue).Select(c => c.Nota!.Value)),
                Aprobados = cursos.Count(c => c.Estado == ReglasAcademicas.Aprobado),
                Reprobados = cursos.Count(c => c.Estado == ReglasAcademicas.Reprobado),
                Pendientes = cursos.Count(c => c.Estado == ReglasAcademicas.Pendiente),
                Cursos = cursos
            };
        }

        public async Task<MatriculaEstudianteDto?> ObtenerMatriculaAsync(string carnet)
        {
            var estudiante = await BuscarAsync(carnet);
            if (estudiante == null) return null;

            var matriculas = await _contexto.Matriculas.AsNoTracking()
                .Where(m => m.EstudianteId == estudiante.Id)
                .Include(m => m.Curso).ThenInclude(c => c!.Docente)
                .ToListAsync();

            var cursos = matriculas
                .OrderBy(m => m.Curso!.Nombre)
                .Select(m => new CursoMatriculadoDto
                {
                    CodigoMatricula = m.CodigoMatricula,
                    CodigoCurso = m.Curso!.Codigo,
                    NombreCurso = m.Curso.Nombre,
                    Docente = m.Curso.Docente?.NombreCompleto ?? string.Empty,
                    FechaMatricula = m.FechaMatricula
                })
                .ToList();

            return new MatriculaEstudianteDto
            {
                EstudianteId = estudiante.Id,
                Carnet = estudiante.Carnet,
                NombreCompleto = estudiante.NombreCompleto,
                EstadoAcademico = estudiante.EstadoAcademico,
                Matriculado = cursos.Count > 0,
                TotalCursos = cursos.Count,
                Cursos = cursos
            };
        }

        private async Task<ControlAcademico.Entidades.Estudiante?> BuscarAsync(string? carnet)
        {
            var normalizado = Validador.Limpiar(carnet).ToUpperInvariant();
            if (normalizado.Length == 0) return null;

            return await _contexto.Estudiantes.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Carnet == normalizado);
        }
    }
}
