using ControlAcademico.Datos;
using ControlAcademico.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ControlAcademico.Negocio
{
    public interface ICursoServicio
    {
        Task<List<Curso>> ListarAsync(string? buscar = null);
        Task<Curso?> ObtenerAsync(int id);
        Task<CursoDetalle?> ObtenerDetalleAsync(int id);
        Task<Resultado> CrearAsync(Curso curso);
        Task<Resultado> ActualizarAsync(Curso curso);
        Task<Resultado> EliminarAsync(int id);
    }

    public class CursoServicio : ICursoServicio
    {
        private readonly ControlAcademicoContext _contexto;

        public CursoServicio(ControlAcademicoContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Curso>> ListarAsync(string? buscar = null)
        {
            IQueryable<Curso> consulta = _contexto.Cursos.AsNoTracking().Include(c => c.Docente);

            foreach (var termino in Validador.Terminos(buscar))
            {
                var t = termino;
                consulta = consulta.Where(c => EF.Functions.Collate(c.Codigo, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(c.Nombre, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(c.Descripcion, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(c.Docente!.Nombre, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(c.Docente!.Apellido, Validador.ColacionSinAcentos).Contains(t));
            }

            return await consulta
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        }

        public async Task<Curso?> ObtenerAsync(int id)
        {
            return await _contexto.Cursos.AsNoTracking()
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CursoDetalle?> ObtenerDetalleAsync(int id)
        {
            var curso = await _contexto.Cursos.AsNoTracking()
                .Include(c => c.Docente)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (curso == null) return null;

            var matriculas = await _contexto.Matriculas.AsNoTracking()
                .Where(m => m.CursoId == id)
                .Include(m => m.Estudiante)
                .Include(m => m.Calificaciones)
                .ToListAsync();

            var estudiantes = matriculas
                .OrderBy(m => m.Estudiante!.Apellido).ThenBy(m => m.Estudiante!.Nombre)
                .Select(m => new EstudianteMatriculado
                {
                    MatriculaId = m.Id,
                    EstudianteId = m.EstudianteId,
                    Carnet = m.Estudiante!.Carnet,
                    NombreCompleto = m.Estudiante.NombreCompleto,
                    Correo = m.Estudiante.Correo,
                    FechaMatricula = m.FechaMatricula,
                    TotalNotas = m.Calificaciones.Count,
                    Promedio = m.Calificaciones.Count > 0
                        ? Math.Round(m.Calificaciones.Average(c => c.Nota), 2)
                        : null
                })
                .ToList();

            var todasLasNotas = matriculas.SelectMany(m => m.Calificaciones).ToList();

            return new CursoDetalle
            {
                Id = curso.Id,
                Codigo = curso.Codigo,
                Nombre = curso.Nombre,
                Creditos = curso.Creditos,
                CupoMaximo = curso.CupoMaximo,
                CuposDisponibles = Math.Max(0, curso.CupoMaximo - estudiantes.Count),
                Descripcion = curso.Descripcion,
                DocenteId = curso.DocenteId,
                DocenteNombre = curso.Docente?.NombreCompleto ?? string.Empty,
                DocenteCorreo = curso.Docente?.Correo ?? string.Empty,
                DocenteEspecialidad = curso.Docente?.Especialidad ?? string.Empty,
                TotalEstudiantes = estudiantes.Count,
                PromedioGeneral = todasLasNotas.Count > 0
                    ? Math.Round(todasLasNotas.Average(c => c.Nota), 2)
                    : null,
                Estudiantes = estudiantes
            };
        }

        public async Task<Resultado> CrearAsync(Curso curso)
        {
            curso.Id = 0;
            curso.Docente = null;
            curso.Matriculas = new List<Matricula>();

            var resultado = await ValidarAsync(curso);
            if (!resultado.Exitoso) return resultado;

            _contexto.Cursos.Add(curso);
            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar el curso. Verifique que el código y el nombre no estén repetidos.");
        }

        public async Task<Resultado> ActualizarAsync(Curso curso)
        {
            var actual = await _contexto.Cursos.FindAsync(curso.Id);
            if (actual == null) return Resultado.NoExiste("El curso que intenta modificar no existe.");

            var resultado = await ValidarAsync(curso);
            if (!resultado.Exitoso) return resultado;

            actual.Codigo = curso.Codigo;
            actual.Nombre = curso.Nombre;
            actual.Creditos = curso.Creditos;
            actual.CupoMaximo = curso.CupoMaximo;
            actual.Descripcion = curso.Descripcion;
            actual.DocenteId = curso.DocenteId;

            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar el curso. Verifique que el código y el nombre no estén repetidos.");
        }

        public async Task<Resultado> EliminarAsync(int id)
        {
            var curso = await _contexto.Cursos.FindAsync(id);
            if (curso == null) return Resultado.NoExiste("El curso que intenta eliminar no existe.");

            if (await _contexto.Matriculas.AnyAsync(m => m.CursoId == id))
            {
                return new Resultado().Agregar(string.Empty,
                    "No se puede eliminar el curso porque tiene estudiantes matriculados.");
            }

            _contexto.Cursos.Remove(curso);
            return await Validador.GuardarAsync(_contexto, new Resultado(),
                "No se pudo eliminar el curso.");
        }

        private async Task<Resultado> ValidarAsync(Curso c)
        {
            var resultado = new Resultado();

            c.Codigo = Validador.Limpiar(c.Codigo).ToUpperInvariant();
            c.Nombre = Validador.Limpiar(c.Nombre);
            c.Descripcion = Validador.Limpiar(c.Descripcion);

            Validador.Texto(resultado, nameof(Curso.Codigo), c.Codigo, "Código", 20);
            Validador.Texto(resultado, nameof(Curso.Nombre), c.Nombre, "Nombre", 150);

            if (c.Creditos < 1 || c.Creditos > 6)
            {
                resultado.Agregar(nameof(Curso.Creditos), "Los créditos deben estar entre 1 y 6.");
            }

            if (c.CupoMaximo < 1 || c.CupoMaximo > 500)
            {
                resultado.Agregar(nameof(Curso.CupoMaximo), "El cupo máximo debe ser mayor que 0 (máximo 500).");
            }
            else if (c.Id > 0)
            {
                var inscritos = await _contexto.Matriculas.CountAsync(m => m.CursoId == c.Id);
                if (c.CupoMaximo < inscritos)
                {
                    resultado.Agregar(nameof(Curso.CupoMaximo),
                        $"El cupo máximo no puede ser menor que los {inscritos} estudiantes ya matriculados.");
                }
            }
            Validador.Texto(resultado, nameof(Curso.Descripcion), c.Descripcion, "Descripción", 1000);

            if (c.DocenteId <= 0)
            {
                resultado.Agregar(nameof(Curso.DocenteId), "Debe seleccionar un docente.");
            }
            else if (!await _contexto.Docentes.AnyAsync(d => d.Id == c.DocenteId))
            {
                resultado.Agregar(nameof(Curso.DocenteId), "El docente seleccionado no existe.");
            }

            if (resultado.Errores.All(x => x.Campo != nameof(Curso.Codigo))
                && await _contexto.Cursos.AnyAsync(x => x.Id != c.Id && x.Codigo == c.Codigo))
            {
                resultado.Agregar(nameof(Curso.Codigo), $"El código '{c.Codigo}' ya está registrado en otro curso.");
            }

            if (resultado.Errores.All(x => x.Campo != nameof(Curso.Nombre))
                && await _contexto.Cursos.AnyAsync(x => x.Id != c.Id && x.Nombre == c.Nombre))
            {
                resultado.Agregar(nameof(Curso.Nombre), $"Ya existe un curso con el nombre '{c.Nombre}'.");
            }

            return resultado;
        }
    }
}
