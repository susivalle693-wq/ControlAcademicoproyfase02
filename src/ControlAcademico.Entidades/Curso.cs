using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ControlAcademico.Entidades
{
    public class Curso
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Código es obligatorio.")]
        [StringLength(20, ErrorMessage = "El campo Código no puede superar los 20 caracteres.")]
        [Display(Name = "Código")]
        public string Codigo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [StringLength(150, ErrorMessage = "El campo Nombre no puede superar los 150 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Descripción es obligatorio.")]
        [StringLength(1000, ErrorMessage = "El campo Descripción no puede superar los 1000 caracteres.")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Range(1, 6, ErrorMessage = "Los créditos deben estar entre 1 y 6.")]
        [Display(Name = "Créditos")]
        public int Creditos { get; set; } = 4;

        [Range(1, 500, ErrorMessage = "El cupo máximo debe ser mayor que 0 (máximo 500).")]
        [Display(Name = "Cupo máximo")]
        public int CupoMaximo { get; set; } = 30;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un docente.")]
        [Display(Name = "Docente")]
        public int DocenteId { get; set; }

        [ValidateNever]
        public Docente? Docente { get; set; }

        [ValidateNever]
        public List<Matricula> Matriculas { get; set; } = new();
    }
}
