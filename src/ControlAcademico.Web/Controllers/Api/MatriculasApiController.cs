using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;

namespace ControlAcademico.Controllers.Api
{
    [ApiController]
    [Route("api/matriculas")]
    [Produces("application/json")]
    public class MatriculasApiController : ApiBase
    {
        private readonly IMatriculaServicio _servicio;

        public MatriculasApiController(IMatriculaServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        public async Task<ActionResult<List<Matricula>>> Listar()
        {
            return Ok(await _servicio.ListarAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Matricula>> Obtener(int id)
        {
            var item = await _servicio.ObtenerAsync(id);
            if (item == null) return NoExiste("La matrícula solicitada no existe.");
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Matricula>> Crear(Matricula matricula)
        {
            var resultado = await _servicio.CrearAsync(matricula);
            if (!resultado.Exitoso) return Fallo(resultado);
            return CreatedAtAction(nameof(Obtener), new { id = matricula.Id }, matricula);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, Matricula matricula)
        {
            if (id != matricula.Id) return IdNoCoincide();
            var resultado = await _servicio.ActualizarAsync(matricula);
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
