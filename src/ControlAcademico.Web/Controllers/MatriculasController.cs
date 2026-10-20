using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlAcademico.Controllers
{
    public class MatriculasController : Controller
    {
        private readonly IMatriculaServicio _servicio;
        private readonly IEstudianteServicio _estudiantes;
        private readonly ICursoServicio _cursos;

        public MatriculasController(IMatriculaServicio servicio, IEstudianteServicio estudiantes, ICursoServicio cursos)
        {
            _servicio = servicio;
            _estudiantes = estudiantes;
            _cursos = cursos;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _servicio.ListarAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _servicio.ObtenerAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new Matricula { FechaMatricula = DateTime.Today });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EstudianteId,CursoId,FechaMatricula")] Matricula matricula)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _servicio.CrearAsync(matricula);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Matrícula registrada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                resultado.AplicarA(ModelState);
            }
            await CargarListasAsync();
            return View(matricula);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _servicio.ObtenerAsync(id.Value);
            if (item == null) return NotFound();
            await CargarListasAsync();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EstudianteId,CursoId,FechaMatricula")] Matricula matricula)
        {
            if (id != matricula.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var resultado = await _servicio.ActualizarAsync(matricula);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Matrícula actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                if (resultado.NoEncontrado) return NotFound();
                resultado.AplicarA(ModelState);
            }
            await CargarListasAsync();
            return View(matricula);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _servicio.ObtenerAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resultado = await _servicio.EliminarAsync(id);
            if (resultado.Exitoso)
            {
                TempData["Mensaje"] = "Matrícula eliminada correctamente.";
            }
            else
            {
                TempData["Error"] = string.Join(" ", resultado.Errores.Select(e => e.Mensaje));
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync()
        {
            var estudiantes = await _estudiantes.ListarAsync();
            ViewBag.Estudiantes = new SelectList(
                estudiantes.Select(e => new { e.Id, Texto = e.Carnet + " - " + e.NombreCompleto }), "Id", "Texto");

            var cursos = await _cursos.ListarAsync();
            ViewBag.Cursos = new SelectList(cursos, "Id", "Nombre");
        }
    }
}
