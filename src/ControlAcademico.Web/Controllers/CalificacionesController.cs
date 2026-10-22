using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlAcademico.Controllers
{
    // Controlador de presentación: solo llama a servicios de Negocio (nunca al DbContext).
    public class CalificacionesController : Controller
    {
        private readonly ICalificacionServicio _servicio;
        private readonly IMatriculaServicio _matriculas;
        private readonly ICursoServicio _cursos;

        public CalificacionesController(ICalificacionServicio servicio, IMatriculaServicio matriculas, ICursoServicio cursos)
        {
            _servicio = servicio;
            _matriculas = matriculas;
            _cursos = cursos;
        }

        // Listado con buscador por estudiante y filtro por curso.
        public async Task<IActionResult> Index(string? buscar, int? cursoId)
        {
            ViewBag.Buscar = buscar;
            ViewBag.CursoId = cursoId;
            ViewBag.Cursos = new SelectList(await _cursos.ListarAsync(), "Id", "Nombre", cursoId);
            return View(await _servicio.ListarAsync(buscar, cursoId));
        }

        // Muestra el detalle de un registro; si el id no existe devuelve 404.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _servicio.ObtenerAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        // GET: muestra el formulario vacío para registrar un nuevo elemento.
        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new Calificacion());
        }

        // POST: recibe el formulario. Primero valida con ModelState (reglas de las entidades) y luego con el servicio de Negocio (reglas del sistema). Solo se enlazan los campos permitidos con [Bind] para evitar el sobre-envío de datos.
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

        // GET: carga el registro existente en el formulario de edición.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _servicio.ObtenerAsync(id.Value);
            if (item == null) return NotFound();
            await CargarListasAsync();
            return View(item);
        }

        // POST: guarda los cambios; los errores de Negocio se agregan al ModelState para mostrarlos junto al campo.
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

        // GET: página de confirmación de eliminación (respaldo cuando no hay JavaScript; con JavaScript se usa el modal).
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var item = await _servicio.ObtenerAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: elimina el registro. Si Negocio lo bloquea (por ejemplo, tiene datos relacionados) se muestra el mensaje con TempData["Error"].
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

        // Llena las listas desplegables del formulario (ViewBag) usando servicios de Negocio.
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