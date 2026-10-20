using System.ComponentModel.DataAnnotations;

namespace ControlAcademico.Entidades
{
    public class Docente : Persona
    {
        [Required(ErrorMessage = "El campo Código de empleado es obligatorio.")]
        [StringLength(20, ErrorMessage = "El campo Código de empleado no puede superar los 20 caracteres.")]
        [Display(Name = "Código de empleado")]
        public string CodigoEmpleado { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Especialidad es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Especialidad no puede superar los 100 caracteres.")]
        [Display(Name = "Especialidad")]
        public string Especialidad { get; set; } = string.Empty;
    }
}
