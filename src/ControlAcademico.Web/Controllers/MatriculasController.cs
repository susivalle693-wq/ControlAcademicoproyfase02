using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlAcademico.Controllers
{
    // Controlador de presentación de matrículas: usa solo servicios de Negocio.
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

        // Historial de matrículas con buscador (código, carnet, estudiante o curso).
        public async Task<IActionResult> Index(string? buscar)
        {
            ViewBag.Buscar = buscar;
            return View(await _servicio.ListarAsync(buscar));
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
            return View(new Matricula { FechaMatricula = DateTime.Today });
        }

        // POST: recibe el formulario. Primero valida con ModelState (reglas de las entidades) y luego con el servicio de Negocio (reglas del sistema). Solo se enlazan los campos permitidos con [Bind] para evitar el sobre-envío de datos.
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
                TempData["Mensaje"] = "Matrícula eliminada correctamente.";
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
            var estudiantes = await _estudiantes.ListarAsync();
            ViewBag.Estudiantes = new SelectList(
                estudiantes.Select(e => new { e.Id, Texto = e.Carnet + " - " + e.NombreCompleto }), "Id", "Texto");

            var cursos = await _cursos.ListarAsync();
            ViewBag.Cursos = new SelectList(cursos, "Id", "Nombre");
        }
    }
}