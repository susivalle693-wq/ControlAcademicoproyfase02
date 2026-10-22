using ControlAcademico.Datos;
using ControlAcademico.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ControlAcademico.Negocio
{
    // Contrato de Negocio para calificaciones (la capa Web solo usa esta interfaz).
    public interface ICalificacionServicio
    {
        Task<List<Calificacion>> ListarAsync(string? buscar = null, int? cursoId = null);
        Task<Calificacion?> ObtenerAsync(int id);
        Task<List<Calificacion>> ObtenerPorEstudianteAsync(int estudianteId);
        Task<double?> CalcularPromedioAsync(int estudianteId);
        Task<Resultado> CrearAsync(Calificacion calificacion);
        Task<Resultado> ActualizarAsync(Calificacion calificacion);
        Task<Resultado> EliminarAsync(int id);
    }

    // Reglas de calificaciones: nota entre 0 y 10, una nota por matrícula y estado calculado con la nota mínima.
    public class CalificacionServicio : ICalificacionServicio
    {
        private readonly ControlAcademicoContext _contexto;

        public CalificacionServicio(ControlAcademicoContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Calificacion>> ListarAsync(string? buscar = null, int? cursoId = null)
        {
            IQueryable<Calificacion> consulta = _contexto.Calificaciones.AsNoTracking()
                .Include(c => c.Matricula).ThenInclude(m => m!.Estudiante)
                .Include(c => c.Matricula).ThenInclude(m => m!.Curso);

            // Filtro por curso (lista desplegable del wireframe); si no se elige curso se muestran todas.
            if (cursoId.HasValue && cursoId.Value > 0)
            {
                var id = cursoId.Value;
                consulta = consulta.Where(c => c.Matricula!.CursoId == id);
            }

            // Buscador por estudiante (carnet, nombre o apellido) o por curso.
            foreach (var termino in Validador.Terminos(buscar))
            {
                var t = termino;
                consulta = consulta.Where(c =>
                    EF.Functions.Collate(c.Matricula!.Estudiante!.Carnet, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(c.Matricula!.Estudiante!.Nombre, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(c.Matricula!.Estudiante!.Apellido, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(c.Matricula!.Curso!.Nombre, Validador.ColacionSinAcentos).Contains(t));
            }

            return await consulta
                .OrderBy(c => c.Matricula!.Estudiante!.Apellido)
                .ThenBy(c => c.Matricula!.Estudiante!.Nombre)
                .ThenBy(c => c.Id)
                .ToListAsync();
        }

        // Notas de un estudiante, ordenadas por curso.
        public async Task<List<Calificacion>> ObtenerPorEstudianteAsync(int estudianteId)
        {
            return await _contexto.Calificaciones.AsNoTracking()
                .Include(c => c.Matricula).ThenInclude(m => m!.Curso)
                .Where(c => c.Matricula!.EstudianteId == estudianteId)
                .OrderBy(c => c.Matricula!.Curso!.Nombre)
                .ToListAsync();
        }

        // Promedio de todas las notas del estudiante (null si todavía no tiene notas).
        public async Task<double?> CalcularPromedioAsync(int estudianteId)
        {
            var notas = await _contexto.Calificaciones.AsNoTracking()
                .Where(c => c.Matricula!.EstudianteId == estudianteId)
                .Select(c => c.Nota)
                .ToListAsync();
            return ReglasAcademicas.CalcularPromedio(notas);
        }

        // Devuelve una calificación con su matrícula, estudiante y curso (null si no existe).
        public async Task<Calificacion?> ObtenerAsync(int id)
        {
            return await _contexto.Calificaciones.AsNoTracking()
                .Include(c => c.Matricula).ThenInclude(m => m!.Estudiante)
                .Include(c => c.Matricula).ThenInclude(m => m!.Curso)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // Registra una nota nueva y calcula su estado (Aprobado/Reprobado).
        public async Task<Resultado> CrearAsync(Calificacion calificacion)
        {
            // Se ignoran el Id y la matrícula enviados por el formulario.
            calificacion.Id = 0;
            calificacion.Matricula = null;

            var resultado = await ValidarAsync(calificacion);
            if (!resultado.Exitoso) return resultado;

            // El estado (Aprobado/Reprobado) lo calcula Negocio con la nota mínima.
            calificacion.Estado = ReglasAcademicas.ObtenerEstado(calificacion.Nota);

            _contexto.Calificaciones.Add(calificacion);
            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar la calificación.");
        }

        // Modifica la nota y recalcula el estado.
        public async Task<Resultado> ActualizarAsync(Calificacion calificacion)
        {
            var actual = await _contexto.Calificaciones.FindAsync(calificacion.Id);
            if (actual == null) return Resultado.NoExiste("La calificación que intenta modificar no existe.");

            var resultado = await ValidarAsync(calificacion);
            if (!resultado.Exitoso) return resultado;

            actual.Nota = calificacion.Nota;
            actual.MatriculaId = calificacion.MatriculaId;
            actual.Estado = ReglasAcademicas.ObtenerEstado(calificacion.Nota);

            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar la calificación.");
        }

        // Elimina una calificación.
        public async Task<Resultado> EliminarAsync(int id)
        {
            var calificacion = await _contexto.Calificaciones.FindAsync(id);
            if (calificacion == null) return Resultado.NoExiste("La calificación que intenta eliminar no existe.");

            _contexto.Calificaciones.Remove(calificacion);
            return await Validador.GuardarAsync(_contexto, new Resultado(),
                "No se pudo eliminar la calificación.");
        }

        // Validaciones de negocio: rango de la nota, matrícula existente y una sola nota por matrícula.
        private async Task<Resultado> ValidarAsync(Calificacion c)
        {
            var resultado = new Resultado();

            // 1) La nota debe ser un número entre 0 y 10.
            if (double.IsNaN(c.Nota) || double.IsInfinity(c.Nota) || c.Nota < 0 || c.Nota > 10)
            {
                resultado.Agregar(nameof(Calificacion.Nota), "La nota debe estar entre 0 y 10.");
            }

            // 2) La matrícula debe estar seleccionada, existir y no tener ya una nota.
            if (c.MatriculaId <= 0)
            {
                resultado.Agregar(nameof(Calificacion.MatriculaId), "Debe seleccionar una matrícula.");
            }
            else if (!await _contexto.Matriculas.AnyAsync(m => m.Id == c.MatriculaId))
            {
                resultado.Agregar(nameof(Calificacion.MatriculaId), "La matrícula seleccionada no existe.");
            }
            else if (await _contexto.Calificaciones.AnyAsync(x => x.Id != c.Id && x.MatriculaId == c.MatriculaId))
            {
                resultado.Agregar(nameof(Calificacion.MatriculaId), "Esta matrícula ya tiene una calificación registrada.");
            }

            return resultado;
        }
    }
}