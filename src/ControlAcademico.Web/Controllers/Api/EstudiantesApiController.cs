using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;

namespace ControlAcademico.Controllers.Api
{
    [ApiController]
    [Route("api/estudiantes")]
    [Produces("application/json")]
    public class EstudiantesApiController : ApiBase
    {
        private readonly IEstudianteServicio _servicio;
        private readonly IConsultaEstudianteServicio _consulta;

        public EstudiantesApiController(IEstudianteServicio servicio, IConsultaEstudianteServicio consulta)
        {
            _servicio = servicio;
            _consulta = consulta;
        }

        /// <summary>Consulta las notas de un estudiante por carnet: cursos con nota y estado, y promedio.</summary>
        /// <param name="carnet">Carnet del estudiante, por ejemplo AB253001.</param>
        [HttpGet("{carnet}/notas")]
        [ProducesResponseType(typeof(NotasEstudianteDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<ActionResult<NotasEstudianteDto>> ObtenerNotas(string carnet)
        {
            var resultado = await _consulta.ObtenerNotasAsync(carnet);
            if (resultado == null) return NoExiste($"No existe un estudiante con el carnet '{carnet}'.");
            return Ok(resultado);
        }

        /// <summary>Consulta el estado de matrícula de un estudiante por carnet: si está matriculado y en qué cursos.</summary>
        /// <param name="carnet">Carnet del estudiante, por ejemplo AB253001.</param>
        [HttpGet("{carnet}/matricula")]
        [ProducesResponseType(typeof(MatriculaEstudianteDto), 200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<ActionResult<MatriculaEstudianteDto>> ObtenerMatricula(string carnet)
        {
            var resultado = await _consulta.ObtenerMatriculaAsync(carnet);
            if (resultado == null) return NoExiste($"No existe un estudiante con el carnet '{carnet}'.");
            return Ok(resultado);
        }

        [HttpGet]
        public async Task<ActionResult<List<Estudiante>>> Listar()
        {
            return Ok(await _servicio.ListarAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Estudiante>> Obtener(int id)
        {
            var item = await _servicio.ObtenerAsync(id);
            if (item == null) return NoExiste("El estudiante solicitado no existe.");
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Estudiante>> Crear(Estudiante estudiante)
        {
            var resultado = await _servicio.CrearAsync(estudiante);
            if (!resultado.Exitoso) return Fallo(resultado);
            return CreatedAtAction(nameof(Obtener), new { id = estudiante.Id }, estudiante);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, Estudiante estudiante)
        {
            if (id != estudiante.Id) return IdNoCoincide();
            var resultado = await _servicio.ActualizarAsync(estudiante);
            if (!resultado.Exitoso) return Fallo(resultado);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _servicio.EliminarAsync(id);
            if (!resultado.Exitoso) return Fallo(resultado);
            return NoContent();
        }
    }
}
