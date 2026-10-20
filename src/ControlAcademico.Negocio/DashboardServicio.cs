using ControlAcademico.Datos;
using Microsoft.EntityFrameworkCore;

namespace ControlAcademico.Negocio
{
    public class InscritosPorCurso
    {
        public string Curso { get; set; } = string.Empty;
        public int Inscritos { get; set; }
    }

    public class ActividadReciente
    {
        public string Descripcion { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }

    public class DashboardResumen
    {
        public int Estudiantes { get; set; }
        public int Docentes { get; set; }
        public int Cursos { get; set; }
        public int Matriculas { get; set; }
        public List<InscritosPorCurso> InscritosPorCurso { get; set; } = new();
        public List<ActividadReciente> ActividadReciente { get; set; } = new();

        public int MayorInscripcion => InscritosPorCurso.Count > 0 ? Math.Max(1, InscritosPorCurso.Max(c => c.Inscritos)) : 1;
    }

    public interface IDashboardServicio
    {
        Task<DashboardResumen> ObtenerResumenAsync();
    }

    public class DashboardServicio : IDashboardServicio
    {
        private readonly ControlAcademicoContext _contexto;

        public DashboardServicio(ControlAcademicoContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<DashboardResumen> ObtenerResumenAsync()
        {
            var resumen = new DashboardResumen
            {
                Estudiantes = await _contexto.Estudiantes.CountAsync(),
                Docentes = await _contexto.Docentes.CountAsync(),
                Cursos = await _contexto.Cursos.CountAsync(),
                Matriculas = await _contexto.Matriculas.CountAsync()
            };

            resumen.InscritosPorCurso = await _contexto.Cursos.AsNoTracking()
                .Select(c => new InscritosPorCurso { Curso = c.Nombre, Inscritos = c.Matriculas.Count })
                .OrderByDescending(c => c.Inscritos)
                .ThenBy(c => c.Curso)
                .ToListAsync();

            resumen.ActividadReciente = await _contexto.Actividad.AsNoTracking()
                .OrderByDescending(a => a.Fecha)
                .ThenByDescending(a => a.Id)
                .Take(8)
                .Select(a => new ActividadReciente { Descripcion = a.Descripcion, Fecha = a.Fecha })
                .ToListAsync();

            return resumen;
        }
    }
}
