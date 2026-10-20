using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;

namespace ControlAcademico.Controllers.Api
{
    [ApiController]
    [Route("api/cursos")]
    [Produces("application/json")]
    public class CursosApiController : ApiBase
    {
        private readonly ICursoServicio _servicio;

        public CursosApiController(ICursoServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Curso>>> Listar()
        {
            return Ok(await _servicio.ListarAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Curso>> Obtener(int id)
        {
            var item = await _servicio.ObtenerAsync(id);
            if (item == null) return NoExiste("El curso solicitado no existe.");
            return Ok(item);
        }

        [HttpGet("{id:int}/detalle")]
        public async Task<ActionResult<CursoDetalle>> Detalle(int id)
        {
            var detalle = await _servicio.ObtenerDetalleAsync(id);
            if (detalle == null) return NoExiste("El curso solicitado no existe.");
            return Ok(detalle);
        }

        [HttpPost]
        public async Task<ActionResult<Curso>> Crear(Curso curso)
        {
            var resultado = await _servicio.CrearAsync(curso);
            if (!resultado.Exitoso) return Fallo(resultado);
            return CreatedAtAction(nameof(Obtener), new { id = curso.Id }, curso);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, Curso curso)
        {
            if (id != curso.Id) return IdNoCoincide();
            var resultado = await _servicio.ActualizarAsync(curso);
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
