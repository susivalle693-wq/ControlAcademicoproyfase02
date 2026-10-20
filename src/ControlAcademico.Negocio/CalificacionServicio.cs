using ControlAcademico.Datos;
using ControlAcademico.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ControlAcademico.Negocio
{
    public interface ICalificacionServicio
    {
        Task<List<Calificacion>> ListarAsync();
        Task<Calificacion?> ObtenerAsync(int id);
        Task<Resultado> CrearAsync(Calificacion calificacion);
        Task<Resultado> ActualizarAsync(Calificacion calificacion);
        Task<Resultado> EliminarAsync(int id);
    }

    public class CalificacionServicio : ICalificacionServicio
    {
        private readonly ControlAcademicoContext _contexto;

        public CalificacionServicio(ControlAcademicoContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Calificacion>> ListarAsync()
        {
            return await _contexto.Calificaciones.AsNoTracking()
                .Include(c => c.Matricula).ThenInclude(m => m!.Estudiante)
                .Include(c => c.Matricula).ThenInclude(m => m!.Curso)
                .OrderBy(c => c.Id)
                .ToListAsync();
        }

        public async Task<Calificacion?> ObtenerAsync(int id)
        {
            return await _contexto.Calificaciones.AsNoTracking()
                .Include(c => c.Matricula).ThenInclude(m => m!.Estudiante)
                .Include(c => c.Matricula).ThenInclude(m => m!.Curso)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Resultado> CrearAsync(Calificacion calificacion)
        {
            calificacion.Id = 0;
            calificacion.Matricula = null;

            var resultado = await ValidarAsync(calificacion);
            if (!resultado.Exitoso) return resultado;

            _contexto.Calificaciones.Add(calificacion);
            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar la calificación.");
        }

        public async Task<Resultado> ActualizarAsync(Calificacion calificacion)
        {
            var actual = await _contexto.Calificaciones.FindAsync(calificacion.Id);
            if (actual == null) return Resultado.NoExiste("La calificación que intenta modificar no existe.");

            var resultado = await ValidarAsync(calificacion);
            if (!resultado.Exitoso) return resultado;

            actual.Nota = calificacion.Nota;
            actual.MatriculaId = calificacion.MatriculaId;

            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar la calificación.");
        }

        public async Task<Resultado> EliminarAsync(int id)
        {
            var calificacion = await _contexto.Calificaciones.FindAsync(id);
            if (calificacion == null) return Resultado.NoExiste("La calificación que intenta eliminar no existe.");

            _contexto.Calificaciones.Remove(calificacion);
            return await Validador.GuardarAsync(_contexto, new Resultado(),
                "No se pudo eliminar la calificación.");
        }

        private async Task<Resultado> ValidarAsync(Calificacion c)
        {
            var resultado = new Resultado();

            if (double.IsNaN(c.Nota) || double.IsInfinity(c.Nota) || c.Nota < 0 || c.Nota > 10)
            {
                resultado.Agregar(nameof(Calificacion.Nota), "La nota debe estar entre 0 y 10.");
            }

            if (c.MatriculaId <= 0)
            {
                resultado.Agregar(nameof(Calificacion.MatriculaId), "Debe seleccionar una matrícula.");
            }
            else if (!await _contexto.Matriculas.AnyAsync(m => m.Id == c.MatriculaId))
            {
                resultado.Agregar(nameof(Calificacion.MatriculaId), "La matrícula seleccionada no existe.");
            }

            return resultado;
        }
    }
}
