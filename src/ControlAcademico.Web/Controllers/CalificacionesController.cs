using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlAcademico.Controllers
{
    public class CalificacionesController : Controller
    {
        private readonly ICalificacionServicio _servicio;
        private readonly IMatriculaServicio _matriculas;

        public CalificacionesController(ICalificacionServicio servicio, IMatriculaServicio matriculas)
        {
            _servicio = servicio;
            _matriculas = matriculas;
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
            return View(new Calificacion());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MatriculaId,Nota")] Calificacion calificacion)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _servicio.CrearAsync(calificacion);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Calificación registrada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                resultado.AplicarA(ModelState);
            }
            await CargarListasAsync();
            return View(calificacion);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,MatriculaId,Nota")] Calificacion calificacion)
        {
            if (id != calificacion.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var resultado = await _servicio.ActualizarAsync(calificacion);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Calificación actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                if (resultado.NoEncontrado) return NotFound();
                resultado.AplicarA(ModelState);
            }
            await CargarListasAsync();
            return View(calificacion);
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
                TempData["Mensaje"] = "Calificación eliminada correctamente.";
            }
            else
            {
                TempData["Error"] = string.Join(" ", resultado.Errores.Select(e => e.Mensaje));
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync()
        {
            var matriculas = await _matriculas.ListarAsync();
            ViewBag.Matriculas = new SelectList(
                matriculas.Select(m => new
                {
                    m.Id,
                    Texto = (m.Estudiante?.Carnet ?? "") + " - " + (m.Estudiante?.NombreCompleto ?? "") + " | " + (m.Curso?.Nombre ?? "")
                }), "Id", "Texto");
        }
    }
}
