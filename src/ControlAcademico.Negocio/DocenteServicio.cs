using ControlAcademico.Datos;
using ControlAcademico.Entidades;
using Microsoft.EntityFrameworkCore;

namespace ControlAcademico.Negocio
{
    public interface IDocenteServicio
    {
        Task<List<Docente>> ListarAsync(string? buscar = null);
        Task<Docente?> ObtenerAsync(int id);
        Task<Resultado> CrearAsync(Docente docente);
        Task<Resultado> ActualizarAsync(Docente docente);
        Task<Resultado> EliminarAsync(int id);
    }

    public class DocenteServicio : IDocenteServicio
    {
        private readonly ControlAcademicoContext _contexto;

        public DocenteServicio(ControlAcademicoContext contexto)
        {
            _contexto = contexto;
        }

        public async Task<List<Docente>> ListarAsync(string? buscar = null)
        {
            IQueryable<Docente> consulta = _contexto.Docentes.AsNoTracking();

            foreach (var termino in Validador.Terminos(buscar))
            {
                var t = termino;
                consulta = consulta.Where(d => EF.Functions.Collate(d.CodigoEmpleado, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(d.Nombre, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(d.Apellido, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(d.Correo, Validador.ColacionSinAcentos).Contains(t)
                    || EF.Functions.Collate(d.Especialidad, Validador.ColacionSinAcentos).Contains(t));
            }

            return await consulta
                .OrderBy(d => d.Apellido).ThenBy(d => d.Nombre)
                .ToListAsync();
        }

        public async Task<Docente?> ObtenerAsync(int id)
        {
            return await _contexto.Docentes.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Resultado> CrearAsync(Docente docente)
        {
            docente.Id = 0;
            var resultado = await ValidarAsync(docente);
            if (!resultado.Exitoso) return resultado;

            _contexto.Docentes.Add(docente);
            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar el docente. Verifique que el correo y el código de empleado no estén repetidos.");
        }

        public async Task<Resultado> ActualizarAsync(Docente docente)
        {
            var actual = await _contexto.Docentes.FindAsync(docente.Id);
            if (actual == null) return Resultado.NoExiste("El docente que intenta modificar no existe.");

            var resultado = await ValidarAsync(docente);
            if (!resultado.Exitoso) return resultado;

            actual.Nombre = docente.Nombre;
            actual.Apellido = docente.Apellido;
            actual.Correo = docente.Correo;
            actual.Especialidad = docente.Especialidad;
            actual.CodigoEmpleado = docente.CodigoEmpleado;
            actual.Telefono = docente.Telefono;
            actual.FechaNacimiento = docente.FechaNacimiento;

            return await Validador.GuardarAsync(_contexto, resultado,
                "No se pudo guardar el docente. Verifique que el correo y el código de empleado no estén repetidos.");
        }

        public async Task<Resultado> EliminarAsync(int id)
        {
            var docente = await _contexto.Docentes.FindAsync(id);
            if (docente == null) return Resultado.NoExiste("El docente que intenta eliminar no existe.");

            if (await _contexto.Cursos.AnyAsync(c => c.DocenteId == id))
            {
                return new Resultado().Agregar(string.Empty,
                    "No se puede eliminar el docente porque tiene cursos asignados.");
            }

            _contexto.Docentes.Remove(docente);
            return await Validador.GuardarAsync(_contexto, new Resultado(),
                "No se pudo eliminar el docente.");
        }

        private async Task<Resultado> ValidarAsync(Docente d)
        {
            var resultado = new Resultado();

            d.Nombre = Validador.Limpiar(d.Nombre);
            d.Apellido = Validador.Limpiar(d.Apellido);
            d.Especialidad = Validador.Limpiar(d.Especialidad);
            d.Correo = Validador.Limpiar(d.Correo).ToLowerInvariant();
            d.CodigoEmpleado = Validador.Limpiar(d.CodigoEmpleado).ToUpperInvariant();
            d.Telefono = string.IsNullOrWhiteSpace(d.Telefono) ? null : Validador.Limpiar(d.Telefono);

            Validador.Texto(resultado, nameof(Docente.Nombre), d.Nombre, "Nombre", 100);
            Validador.Texto(resultado, nameof(Docente.Apellido), d.Apellido, "Apellido", 100);
            Validador.Texto(resultado, nameof(Docente.Especialidad), d.Especialidad, "Especialidad", 100);
            Validador.Correo(resultado, nameof(Docente.Correo), d.Correo);
            Validador.Texto(resultado, nameof(Docente.CodigoEmpleado), d.CodigoEmpleado, "Código de empleado", 20);
            Validador.Telefono(resultado, nameof(Docente.Telefono), d.Telefono);
            Validador.FechaNacimiento(resultado, nameof(Docente.FechaNacimiento), d.FechaNacimiento);

            if (resultado.Errores.All(x => x.Campo != nameof(Docente.CodigoEmpleado))
                && await _contexto.Docentes.AnyAsync(x => x.Id != d.Id && x.CodigoEmpleado == d.CodigoEmpleado))
            {
                resultado.Agregar(nameof(Docente.CodigoEmpleado), $"El código de empleado '{d.CodigoEmpleado}' ya está registrado.");
            }

            if (resultado.Errores.All(x => x.Campo != nameof(Docente.Correo))
                && await _contexto.Docentes.AnyAsync(x => x.Id != d.Id && x.Correo == d.Correo))
            {
                resultado.Agregar(nameof(Docente.Correo), $"El correo '{d.Correo}' ya está registrado.");
            }

            return resultado;
        }
    }
}
