using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ControlAcademico.Entidades
{
    public class Calificacion
    {
        public int Id { get; set; }

        [Range(0, 10, ErrorMessage = "La nota debe estar entre 0 y 10.")]
        [Display(Name = "Nota")]
        public double Nota { get; set; }

        // Aprobado / Reprobado: lo calcula Negocio con la nota mínima (Persona B).
        [StringLength(20)]
        [Display(Name = "Estado")]
        public string? Estado { get; set; }

        [Display(Name = "Fecha de registro")]
        [DataType(DataType.Date)]
        public DateTime FechaRegistro { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una matrícula.")]
        [Display(Name = "Matrícula")]
        public int MatriculaId { get; set; }

        [ValidateNever]
        public Matricula? Matricula { get; set; }
    }
}
