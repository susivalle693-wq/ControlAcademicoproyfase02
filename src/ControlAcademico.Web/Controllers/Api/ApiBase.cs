using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;

namespace ControlAcademico.Controllers.Api
{
    public abstract class ApiBase : ControllerBase
    {
        protected ActionResult Fallo(Resultado resultado)
        {
            if (resultado.NoEncontrado)
            {
                return NotFound(new ProblemDetails
                {
                    Title = "Recurso no encontrado",
                    Detail = resultado.Errores.FirstOrDefault()?.Mensaje,
                    Status = StatusCodes.Status404NotFound
                });
            }

            return BadRequest(new ValidationProblemDetails(resultado.ComoDiccionario())
            {
                Title = "Se encontraron errores de validación.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        protected ActionResult NoExiste(string mensaje)
        {
            return NotFound(new ProblemDetails
            {
                Title = "Recurso no encontrado",
                Detail = mensaje,
                Status = StatusCodes.Status404NotFound
            });
        }

        protected ActionResult IdNoCoincide()
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Solicitud no válida",
                Detail = "El identificador de la ruta no coincide con el del cuerpo de la solicitud.",
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}
