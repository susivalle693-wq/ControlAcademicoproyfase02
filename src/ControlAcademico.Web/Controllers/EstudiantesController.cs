using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlAcademico.Controllers
{
    public class EstudiantesController : Controller
    {
        private readonly IEstudianteServicio _servicio;

        public EstudiantesController(IEstudianteServicio servicio)
        {
            _servicio = servicio;
        }

        public async Task<IActionResult> Index(string? buscar)
        {
            ViewBag.Buscar = buscar;
            return View(await _servicio.ListarAsync(buscar));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _servicio.ObtenerAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        public IActionResult Create()
        {
            return View(new Estudiante { FechaIngreso = DateTime.Today, FechaNacimiento = DateTime.Today.AddYears(-18) });
        }

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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var item = await _servicio.ObtenerAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

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
