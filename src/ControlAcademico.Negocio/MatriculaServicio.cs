using ControlAcademico.Datos;
using ControlAcademico.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ControlAcademico.Negocio
{
    // Contrato de la capa de Negocio para matrículas: la capa Web solo conoce esta interfaz, nunca el DbContext.
    public interface IMatriculaServicio
    {
        Task<List<Matricula>> ListarAsync(string? buscar = null);
        Task<Matricula?> ObtenerAsync(int id);
        Task<List<Matricula>> ObtenerPorCursoAsync(int cursoId);
        Task<List<Matricula>> ObtenerPorEstudianteAsync(int estudianteId);
        Task<Resultado> CrearAsync(Matricula matricula);
        Task<Resultado> ActualizarAsync(Matricula matricula);
        Task<Resultado> EliminarAsync(int id);
    }

    // Reglas de matrícula: estudiante y curso existentes, sin duplicados, con cupo disponible y código autogenerado.
    public class MatriculaServicio : IMatriculaServicio
    {
        private readonly ControlAcademicoContext _contexto;

        public MatriculaServicio(ControlAcademicoContext contexto)
        {
            _contexto = contexto;
        }

        // Prefijo del código autogenerado: MAT253-001, MAT253-002...
        private const string PrefijoCodigo = "MAT253-";

        public async Task<List<Matricula>> ListarAsync(string? buscar = null)
        {
            IQueryable<Matricula> consulta = _contexto.Matriculas.AsNoTracking()
                .Include(m => m.Estudiante)
                .Include(m => m.Curso);

            // Cada palabra del buscador debe coincidir con el código, el carnet, el estudiante o el curso.
            foreach (var termino in Validador.Terminos(buscar))
            {
                var t = termino;
                consulta = consulta.Where(m =>
                    EF.Functions.Collate(m.CodigoMatricula!, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(m.Estudiante!.Carnet, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(m.Estudiante!.Nombre, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(m.Estudiante!.Apellido, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(m.Curso!.Codigo, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(m.Curso!.Nombre, Validador.ColacionSinAcentos).Contains(t));
            }

            return await consulta
                .OrderByDescending(m => m.FechaMatricula)
                .ThenByDescending(m => m.Id)
                .ToListAsync();
        }

        public async Task<List<Matricula>> ObtenerPorCursoAsync(int cursoId)
        {
            return await _contexto.Matriculas.AsNoTracking()
                .Include(m => m.Estudiante)
                .Include(m => m.Curso)
                .Where(m => m.CursoId == cursoId)
                .OrderBy(m => m.Estudiante!.Apellido).ThenBy(m => m.Estudiante!.Nombre)
                .ToListAsync();
        }

        public async Task<List<Matricula>> ObtenerPorEstudianteAsync(int estudianteId)
        {
            return await _contexto.Matriculas.AsNoTracking()
                .Include(m => m.Estudiante)
                .Include(m => m.Curso)
                .Where(m => m.EstudianteId == estudianteId)
                .OrderBy(m => m.Curso!.Nombre)
                .ToListAsync();
        }

        // Devuelve una matrícula con su estudiante y curso (null si no existe).
        public async Task<Matricula?> ObtenerAsync(int id)
        {
            return await _contexto.Matriculas.AsNoTracking()
                .Include(m => m.Estudiante)
                .Include(m => m.Curso)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // Crea una matrícula: valida las reglas, genera el código MAT253-### y guarda controlando errores de la BD.
        public async Task<Resultado> CrearAsync(Matricula matricula)
        {
            // Se ignoran el Id y los objetos relacionados que vengan del formulario: solo importan los ids del estudiante y del curso.
            matricula.Id = 0;
            matricula.Estudiante = null;
            matricula.Curso = null;
            matricula.Calificaciones = new List<Calificacion>();

            var resultado = await ValidarAsync(matricula);
            if (!resultado.Exitoso) return resultado;

            // El código lo genera Negocio, nunca el usuario.
            matricula.CodigoMatricula = await GenerarCodigoAsync();

            _contexto.Matriculas.Add(matricula);
            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar la matrícula. Verifique que el estudiante no esté ya matriculado en el curso.");
        }

        // Modifica estudiante, curso y fecha; el código de matrícula nunca cambia.
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

        // Elimina una matrícula; se bloquea si ya tiene calificaciones (no hay borrado en cascada).
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

        // Reúne todas las reglas de negocio; devuelve un Resultado con los errores en español para mostrarlos en la vista.
        private async Task<Resultado> ValidarAsync(Matricula m)
        {
            var resultado = new Resultado();

            // 1) El estudiante debe estar seleccionado y existir en la base de datos.
            if (m.EstudianteId <= 0)
            {
                resultado.Agregar(nameof(Matricula.EstudianteId), "Debe seleccionar un estudiante.");
            }
            else if (!await _contexto.Estudiantes.AnyAsync(e => e.Id == m.EstudianteId))
            {
                resultado.Agregar(nameof(Matricula.EstudianteId), "El estudiante seleccionado no existe.");
            }

            // 2) El curso debe estar seleccionado y existir.
            if (m.CursoId <= 0)
            {
                resultado.Agregar(nameof(Matricula.CursoId), "Debe seleccionar un curso.");
            }
            else if (!await _contexto.Cursos.AnyAsync(c => c.Id == m.CursoId))
            {
                resultado.Agregar(nameof(Matricula.CursoId), "El curso seleccionado no existe.");
            }

            // 3) La fecha es obligatoria.
            if (m.FechaMatricula == default)
            {
                resultado.Agregar(nameof(Matricula.FechaMatricula), "El campo Fecha de matrícula es obligatorio.");
            }

            // 4) Un estudiante no puede matricularse dos veces en el mismo curso.
            if (resultado.Exitoso
                && await _contexto.Matriculas.AnyAsync(x => x.Id != m.Id
                    && x.EstudianteId == m.EstudianteId && x.CursoId == m.CursoId))
            {
                resultado.Agregar(string.Empty, "El estudiante ya está matriculado en este curso.");
            }

            // Regla de cupo (se excluye la propia matrícula para que editar no cuente doble): no se puede matricular en un curso que ya llegó a su cupo máximo.
            if (resultado.Exitoso)
            {
                var cupoMaximo = await _contexto.Cursos
                    .Where(c => c.Id == m.CursoId)
                    .Select(c => c.CupoMaximo)
                    .FirstAsync();
                // Cuántos estudiantes hay ya inscritos en el curso (sin contar esta matrícula).
                var inscritos = await _contexto.Matriculas.CountAsync(x => x.Id != m.Id && x.CursoId == m.CursoId);
                if (inscritos >= cupoMaximo)
                {
                    resultado.Agregar(nameof(Matricula.CursoId),
                        $"El curso ya no tiene cupo disponible (cupo máximo: {cupoMaximo}).");
                }
            }

            return resultado;
        }

        // Siguiente código libre: busca el número más alto existente y suma uno.
        private async Task<string> GenerarCodigoAsync()
        {
            var codigos = await _contexto.Matriculas.AsNoTracking()
                .Where(m => m.CodigoMatricula != null && m.CodigoMatricula.StartsWith(PrefijoCodigo))
                .Select(m => m.CodigoMatricula!)
                .ToListAsync();

            var mayor = 0;
            foreach (var codigo in codigos)
            {
                if (int.TryParse(codigo.Substring(PrefijoCodigo.Length), out var numero) && numero > mayor)
                    mayor = numero;
            }

            return $"{PrefijoCodigo}{mayor + 1:000}";
        }
    }
}