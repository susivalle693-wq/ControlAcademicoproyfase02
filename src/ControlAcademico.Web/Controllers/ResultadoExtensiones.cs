using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ControlAcademico.Controllers
{
    public static class ResultadoExtensiones
    {
        public static void AplicarA(this Resultado resultado, ModelStateDictionary modelState)
        {
            foreach (var error in resultado.Errores)
            {
                modelState.AddModelError(error.Campo, error.Mensaje);
            }
        }

        public static Dictionary<string, string[]> ComoDiccionario(this Resultado resultado)
        {
            return resultado.Errores
                .GroupBy(e => string.IsNullOrEmpty(e.Campo) ? "General" : e.Campo)
                .ToDictionary(g => g.Key, g => g.Select(e => e.Mensaje).ToArray());
        }
    }
}
