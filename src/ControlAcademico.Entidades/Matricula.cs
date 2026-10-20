using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ControlAcademico.Entidades
{
    public class Matricula
    {
        public int Id { get; set; }

        // Lo genera Negocio (MAT253-001, MAT253-002...). Es nulo hasta que Persona B implemente la generación.
        [StringLength(20)]
        [Display(Name = "Código de matrícula")]
        public string? CodigoMatricula { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estudiante.")]
        [Display(Name = "Estudiante")]
        public int EstudianteId { get; set; }

        [ValidateNever]
        public Estudiante? Estudiante { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un curso.")]
        [Display(Name = "Curso")]
        public int CursoId { get; set; }

        [ValidateNever]
        public Curso? Curso { get; set; }

        [Display(Name = "Fecha de matrícula")]
        [DataType(DataType.Date)]
        public DateTime FechaMatricula { get; set; }

        [ValidateNever]
        public List<Calificacion> Calificaciones { get; set; } = new();
    }
}
