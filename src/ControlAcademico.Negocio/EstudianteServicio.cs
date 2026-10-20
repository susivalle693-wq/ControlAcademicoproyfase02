using ControlAcademico.Datos;
using ControlAcademico.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ControlAcademico.Negocio
{
    public interface IEstudianteServicio
    {
        Task<List<Estudiante>> ListarAsync(string? buscar = null);
        Task<Estudiante?> ObtenerAsync(int id);
        Task<Estudiante?> BuscarPorCarnetAsync(string carnet);
        Task<Resultado> CrearAsync(Estudiante estudiante);
        Task<Resultado> ActualizarAsync(Estudiante estudiante);
        Task<Resultado> EliminarAsync(int id);
    }

    public class EstudianteServicio : IEstudianteServicio
    {
        private readonly ControlAcademicoContext _contexto;

        public EstudianteServicio(ControlAcademicoContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Estudiante>> ListarAsync(string? buscar = null)
        {
            IQueryable<Estudiante> consulta = _contexto.Estudiantes.AsNoTracking();

            foreach (var termino in Validador.Terminos(buscar))
            {
                var t = termino;
                consulta = consulta.Where(e => EF.Functions.Collate(e.Carnet, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(e.Nombre, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(e.Apellido, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(e.Correo, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(e.Carrera, Validador.ColacionSinAcentos).Contains(t));
            }

            return await consulta
                .OrderBy(e => e.Apellido).ThenBy(e => e.Nombre)
                .ToListAsync();
        }

        public async Task<Estudiante?> ObtenerAsync(int id)
        {
            return await _contexto.Estudiantes.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Estudiante?> BuscarPorCarnetAsync(string carnet)
        {
            var normalizado = Validador.Limpiar(carnet).ToUpperInvariant();
            return await _contexto.Estudiantes.AsNoTracking().FirstOrDefaultAsync(e => e.Carnet == normalizado);
        }

        public async Task<Resultado> CrearAsync(Estudiante estudiante)
        {
            estudiante.Id = 0;
            var resultado = await ValidarAsync(estudiante);
            if (!resultado.Exitoso) return resultado;

            _contexto.Estudiantes.Add(estudiante);
            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar el estudiante. Verifique que el carnet y el correo no estén repetidos.");
        }

        public async Task<Resultado> ActualizarAsync(Estudiante estudiante)
        {
            var actual = await _contexto.Estudiantes.FindAsync(estudiante.Id);
            if (actual == null) return Resultado.NoExiste("El estudiante que intenta modificar no existe.");

            var resultado = await ValidarAsync(estudiante);
            if (!resultado.Exitoso) return resultado;

            actual.Carnet = estudiante.Carnet;
            actual.Nombre = estudiante.Nombre;
            actual.Apellido = estudiante.Apellido;
            actual.Correo = estudiante.Correo;
            actual.FechaIngreso = estudiante.FechaIngreso;
            actual.Telefono = estudiante.Telefono;
            actual.FechaNacimiento = estudiante.FechaNacimiento;
            actual.Carrera = estudiante.Carrera;
            actual.Facultad = estudiante.Facultad;
            actual.CicloActual = estudiante.CicloActual;
            actual.CUM = estudiante.CUM;
            actual.EstadoAcademico = estudiante.EstadoAcademico;
            actual.CreditosAprobados = estudiante.CreditosAprobados;

            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar el estudiante. Verifique que el carnet y el correo no estén repetidos.");
        }

        public async Task<Resultado> EliminarAsync(int id)
        {
            var estudiante = await _contexto.Estudiantes.FindAsync(id);
            if (estudiante == null) return Resultado.NoExiste("El estudiante que intenta eliminar no existe.");

            if (await _contexto.Matriculas.AnyAsync(m => m.EstudianteId == id))
            {
                return new Resultado().Agregar(string.Empty,
                    "No se puede eliminar el estudiante porque tiene matrículas registradas.");
            }

            _contexto.Estudiantes.Remove(estudiante);
            return await Validador.GuardarAsync(_contexto, new Resultado(),
                "No se pudo eliminar el estudiante.");
        }

        private async Task<Resultado> ValidarAsync(Estudiante e)
        {
            var resultado = new Resultado();

            e.Nombre = Validador.Limpiar(e.Nombre);
            e.Apellido = Validador.Limpiar(e.Apellido);
            e.Carnet = Validador.Limpiar(e.Carnet);
            e.Correo = Validador.Limpiar(e.Correo).ToLowerInvariant();
            e.Carnet = e.Carnet.ToUpperInvariant();
            e.Carrera = Validador.Limpiar(e.Carrera);
            e.Facultad = Validador.Limpiar(e.Facultad);
            e.EstadoAcademico = Validador.Limpiar(e.EstadoAcademico);
            e.Telefono = string.IsNullOrWhiteSpace(e.Telefono) ? null : Validador.Limpiar(e.Telefono);

            Validador.Texto(resultado, nameof(Estudiante.Nombre), e.Nombre, "Nombre", 100);
            Validador.Texto(resultado, nameof(Estudiante.Apellido), e.Apellido, "Apellido", 100);
            Validador.Texto(resultado, nameof(Estudiante.Carnet), e.Carnet, "Carnet", 20);
            Validador.Correo(resultado, nameof(Estudiante.Correo), e.Correo);
            Validador.Texto(resultado, nameof(Estudiante.Carrera), e.Carrera, "Carrera", 100);
            Validador.Texto(resultado, nameof(Estudiante.Facultad), e.Facultad, "Facultad", 100);
            Validador.Telefono(resultado, nameof(Estudiante.Telefono), e.Telefono);
            Validador.FechaNacimiento(resultado, nameof(Estudiante.FechaNacimiento), e.FechaNacimiento);

            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Carnet, @"^[A-Z]{2}\d{6}$"))
            {
                resultado.Agregar(nameof(Estudiante.Carnet), "El carnet debe tener 2 letras mayúsculas y 6 números (ejemplo: AB253001).");
            }

            if (e.CicloActual < 1 || e.CicloActual > 12)
            {
                resultado.Agregar(nameof(Estudiante.CicloActual), "El ciclo actual debe estar entre 1 y 12.");
            }

            if (e.CUM < 0 || e.CUM > 10)
            {
                resultado.Agregar(nameof(Estudiante.CUM), "El CUM debe estar entre 0 y 10.");
            }

            if (e.CreditosAprobados < 0 || e.CreditosAprobados > 500)
            {
                resultado.Agregar(nameof(Estudiante.CreditosAprobados), "Los créditos aprobados deben estar entre 0 y 500.");
            }

            if (!Estudiante.EstadosAcademicos.Contains(e.EstadoAcademico))
            {
                resultado.Agregar(nameof(Estudiante.EstadoAcademico),
                    $"El estado académico debe ser uno de: {string.Join(", ", Estudiante.EstadosAcademicos)}.");
            }

            if (e.FechaIngreso == default)
            {
                resultado.Agregar(nameof(Estudiante.FechaIngreso), "El campo Fecha de ingreso es obligatorio.");
            }
            else if (e.FechaIngreso.Date > DateTime.Today)
            {
                resultado.Agregar(nameof(Estudiante.FechaIngreso), "La fecha de ingreso no puede ser futura.");
            }

            if (resultado.Errores.All(x => x.Campo != nameof(Estudiante.Carnet))
                && await _contexto.Estudiantes.AnyAsync(x => x.Id != e.Id && x.Carnet == e.Carnet))
            {
                resultado.Agregar(nameof(Estudiante.Carnet), $"El carnet '{e.Carnet}' ya está registrado.");
            }

            if (resultado.Errores.All(x => x.Campo != nameof(Estudiante.Correo))
                && await _contexto.Estudiantes.AnyAsync(x => x.Id != e.Id && x.Correo == e.Correo))
            {
                resultado.Agregar(nameof(Estudiante.Correo), $"El correo '{e.Correo}' ya está registrado.");
            }

            return resultado;
        }
    }
}
