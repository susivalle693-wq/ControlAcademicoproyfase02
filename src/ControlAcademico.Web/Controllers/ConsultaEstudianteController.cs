using Microsoft.AspNetCore.Mvc;

namespace ControlAcademico.Controllers
{
    // Página que CONSUME el servicio web (api/estudiantes/{carnet}/notas y /matricula) con fetch.
    public class ConsultaEstudianteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
