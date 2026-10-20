using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;

namespace ControlAcademico.Controllers.Api
{
    [ApiController]
    [Route("api/docentes")]
    [Produces("application/json")]
    public class DocentesApiController : ApiBase
    {
        private readonly IDocenteServicio _servicio;

        public DocentesApiController(IDocenteServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Docente>>> Listar()
        {
            return Ok(await _servicio.ListarAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Docente>> Obtener(int id)
        {
            var item = await _servicio.ObtenerAsync(id);
            if (item == null) return NoExiste("El docente solicitado no existe.");
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Docente>> Crear(Docente docente)
        {
            var resultado = await _servicio.CrearAsync(docente);
            if (!resultado.Exitoso) return Fallo(resultado);
            return CreatedAtAction(nameof(Obtener), new { id = docente.Id }, docente);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, Docente docente)
        {
            if (id != docente.Id) return IdNoCoincide();
            var resultado = await _servicio.ActualizarAsync(docente);
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
