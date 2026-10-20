using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;

namespace ControlAcademico.Controllers.Api
{
    [ApiController]
    [Route("api/calificaciones")]
    [Produces("application/json")]
    public class CalificacionesApiController : ApiBase
    {
        private readonly ICalificacionServicio _servicio;

        public CalificacionesApiController(ICalificacionServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Calificacion>>> Listar()
        {
            return Ok(await _servicio.ListarAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Calificacion>> Obtener(int id)
        {
            var item = await _servicio.ObtenerAsync(id);
            if (item == null) return NoExiste("La calificación solicitada no existe.");
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Calificacion>> Crear(Calificacion calificacion)
        {
            var resultado = await _servicio.CrearAsync(calificacion);
            if (!resultado.Exitoso) return Fallo(resultado);
            return CreatedAtAction(nameof(Obtener), new { id = calificacion.Id }, calificacion);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, Calificacion calificacion)
        {
            if (id != calificacion.Id) return IdNoCoincide();
            var resultado = await _servicio.ActualizarAsync(calificacion);
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
