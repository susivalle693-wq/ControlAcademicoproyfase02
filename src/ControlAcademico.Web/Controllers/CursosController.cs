using ControlAcademico.Entidades;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlAcademico.Controllers
{
    public class CursosController : Controller
    {
        private readonly ICursoServicio _servicio;
        private readonly IDocenteServicio _docentes;

        public CursosController(ICursoServicio servicio, IDocenteServicio docentes)
        {
            _servicio = servicio;
            _docentes = docentes;
        }

        public async Task<IActionResult> Index(string? buscar)
        {
            ViewBag.Buscar = buscar;
            return View(await _servicio.ListarAsync(buscar));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var item = await _servicio.ObtenerDetalleAsync(id.Value);
            if (item == null) return NotFound();
            return View(item);
        }

        public async Task<IActionResult> Create()
        {
            await CargarListasAsync();
            return View(new Curso());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Codigo,Nombre,Descripcion,Creditos,CupoMaximo,DocenteId")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _servicio.CrearAsync(curso);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Curso registrado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                resultado.AplicarA(ModelState);
            }
            await CargarListasAsync();
            return View(curso);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Codigo,Nombre,Descripcion,Creditos,CupoMaximo,DocenteId")] Curso curso)
        {
            if (id != curso.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var resultado = await _servicio.ActualizarAsync(curso);
                if (resultado.Exitoso)
                {
                    TempData["Mensaje"] = "Curso actualizado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                if (resultado.NoEncontrado) return NotFound();
                resultado.AplicarA(ModelState);
            }
            await CargarListasAsync();
            return View(curso);
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
                TempData["Mensaje"] = "Curso eliminado correctamente.";
            }
            else
            {
                TempData["Error"] = string.Join(" ", resultado.Errores.Select(e => e.Mensaje));
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task CargarListasAsync()
        {
            var docentes = await _docentes.ListarAsync();
            ViewBag.Docentes = new SelectList(docentes, "Id", "NombreCompleto");
        }
    }
}
