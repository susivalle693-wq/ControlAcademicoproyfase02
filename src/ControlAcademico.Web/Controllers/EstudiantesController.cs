using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlAcademico.Controllers
{
    // Controlador de presentación de estudiantes: solo llama a servicios de Negocio.
    public class EstudiantesController : Controller
    {
        private readonly IEstudianteServicio _servicio;
        private readonly IConsultaEstudianteServicio _consulta;

        public EstudiantesController(IEstudianteServicio servicio, IConsultaEstudianteServicio consulta)
        {
            _servicio = servicio;
            _consulta = consulta;
        }

        // Muestra el listado (con buscador). La capa de presentación solo pide los datos al servicio de Negocio.
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
            // Ficha del estudiante: cursos matriculados, notas y promedio (reutiliza el servicio de consulta).
            ViewBag.Notas = await _consulta.ObtenerNotasAsync(item.Carnet);
            return View(item);
        }

        // GET: muestra el formulario vacío para registrar un nuevo elemento.
        public IActionResult Create()
        {
            return View(new Estudiante { FechaIngreso = DateTime.Today, FechaNacimiento = DateTime.Today.AddYears(-18) });
        }

        // POST: recibe el formulario. Primero valida con ModelState (reglas de las entidades) y luego con el servicio de Negocio (reglas del sistema). Solo se enlazan los campos permitidos con [Bind] para evitar el sobre-envío de datos.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Carnet,Correo,FechaIngreso,Nombre,Apellido,Telefono,FechaNacimiento,Carrera,Facultad,CicloActual,CUM,EstadoAcademico,CreditosAprobados")] Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _servicio.CrearAsync(estudiante);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Estudiante registrado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                resultado.AplicarA(ModelState);
            }
            return View(estudiante);
        }

        // GET: carga el registro existente en el formulario de edición.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _servicio.ObtenerAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        // POST: guarda los cambios; los errores de Negocio se agregan al ModelState para mostrarlos junto al campo.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Carnet,Correo,FechaIngreso,Nombre,Apellido,Telefono,FechaNacimiento,Carrera,Facultad,CicloActual,CUM,EstadoAcademico,CreditosAprobados")] Estudiante estudiante)
        {
            if (id != estudiante.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var resultado = await _servicio.ActualizarAsync(estudiante);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Estudiante actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                if (resultado.NoEncontrado) return NotFound();
                resultado.AplicarA(ModelState);
            }
            return View(estudiante);
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
                TempData["Mensaje"] = "Estudiante eliminado correctamente.";
            }
            else
            {
                TempData["Error"] = string.Join(" ", resultado.Errores.Select(e => e.Mensaje));
            }
            return RedirectToAction(nameof(Index));
        }
    }
}