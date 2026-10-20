using ControlAcademico.Datos;
using ControlAcademico.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ControlAcademico.Negocio
{
    public interface IMatriculaServicio
    {
        Task<List<Matricula>> ListarAsync();
        Task<Matricula?> ObtenerAsync(int id);
        Task<Resultado> CrearAsync(Matricula matricula);
        Task<Resultado> ActualizarAsync(Matricula matricula);
        Task<Resultado> EliminarAsync(int id);
    }

    public class MatriculaServicio : IMatriculaServicio
    {
        private readonly ControlAcademicoContext _contexto;

        public MatriculaServicio(ControlAcademicoContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Matricula>> ListarAsync()
        {
            return await _contexto.Matriculas.AsNoTracking()
                .Include(m => m.Estudiante)
                .Include(m => m.Curso)
                .OrderByDescending(m => m.FechaMatricula)
                .ToListAsync();
        }

        public async Task<Matricula?> ObtenerAsync(int id)
        {
            return await _contexto.Matriculas.AsNoTracking()
                .Include(m => m.Estudiante)
                .Include(m => m.Curso)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<Resultado> CrearAsync(Matricula matricula)
        {
            matricula.Id = 0;
            matricula.Estudiante = null;
            matricula.Curso = null;
            matricula.Calificaciones = new List<Calificacion>();

            var resultado = await ValidarAsync(matricula);
            if (!resultado.Exitoso) return resultado;

            _contexto.Matriculas.Add(matricula);
            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar la matrícula. Verifique que el estudiante no esté ya matriculado en el curso.");
        }

        public async Task<Resultado> ActualizarAsync(Matricula matricula)
        {
            var actual = await _contexto.Matriculas.FindAsync(matricula.Id);
            if (actual == null) return Resultado.NoExiste("La matrícula que intenta modificar no existe.");

            var resultado = await ValidarAsync(matricula);
            if (!resultado.Exitoso) return resultado;

            actual.EstudianteId = matricula.EstudianteId;
            actual.CursoId = matricula.CursoId;
            actual.FechaMatricula = matricula.FechaMatricula;

            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar la matrícula. Verifique que el estudiante no esté ya matriculado en el curso.");
        }

        public async Task<Resultado> EliminarAsync(int id)
        {
            var matricula = await _contexto.Matriculas.FindAsync(id);
            if (matricula == null) return Resultado.NoExiste("La matrícula que intenta eliminar no existe.");

            if (await _contexto.Calificaciones.AnyAsync(c => c.MatriculaId == id))
            {
                return new Resultado().Agregar(string.Empty,
                    "No se puede eliminar la matrícula porque tiene calificaciones registradas.");
            }

            _contexto.Matriculas.Remove(matricula);
            return await Validador.GuardarAsync(_contexto, new Resultado(),
                "No se pudo eliminar la matrícula.");
        }

        private async Task<Resultado> ValidarAsync(Matricula m)
        {
            var resultado = new Resultado();

            if (m.EstudianteId <= 0)
            {
                resultado.Agregar(nameof(Matricula.EstudianteId), "Debe seleccionar un estudiante.");
            }
            else if (!await _contexto.Estudiantes.AnyAsync(e => e.Id == m.EstudianteId))
            {
                resultado.Agregar(nameof(Matricula.EstudianteId), "El estudiante seleccionado no existe.");
            }

            if (m.CursoId <= 0)
            {
                resultado.Agregar(nameof(Matricula.CursoId), "Debe seleccionar un curso.");
            }
            else if (!await _contexto.Cursos.AnyAsync(c => c.Id == m.CursoId))
            {
                resultado.Agregar(nameof(Matricula.CursoId), "El curso seleccionado no existe.");
            }

            if (m.FechaMatricula == default)
            {
                resultado.Agregar(nameof(Matricula.FechaMatricula), "El campo Fecha de matrícula es obligatorio.");
            }

            if (resultado.Exitoso
                && await _contexto.Matriculas.AnyAsync(x => x.Id != m.Id
                    && x.EstudianteId == m.EstudianteId && x.CursoId == m.CursoId))
            {
                resultado.Agregar(string.Empty, "El estudiante ya está matriculado en este curso.");
            }

            return resultado;
        }
    }
}
