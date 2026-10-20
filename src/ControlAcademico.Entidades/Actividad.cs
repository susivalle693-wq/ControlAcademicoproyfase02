using System.ComponentModel.DataAnnotations;

namespace ControlAcademico.Entidades
{
    // Registro de acciones del sistema; alimenta "Actividad reciente" del dashboard.
    public class Actividad
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Descripción es obligatorio.")]
        [StringLength(250, ErrorMessage = "El campo Descripción no puede superar los 250 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Fecha")]
        public DateTime Fecha { get; set; }
    }
}
