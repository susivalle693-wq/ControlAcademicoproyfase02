using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlAcademico.Entidades
{
    public class Estudiante : Persona
    {
        // Valores permitidos para el estado académico (se usan en el formulario y en Negocio).
        public static readonly string[] EstadosAcademicos = { "Activo", "Inactivo", "Egresado" };

        [Required(ErrorMessage = "El campo Carnet es obligatorio.")]
        [StringLength(20, ErrorMessage = "El campo Carnet no puede superar los 20 caracteres.")]
        [RegularExpression(@"^[A-Z]{2}\d{6}$", ErrorMessage = "El carnet debe tener 2 letras mayúsculas y 6 números (ejemplo: AB253001).")]
        [Display(Name = "Carnet")]
        public string Carnet { get; set; } = string.Empty;

        [Display(Name = "Fecha de ingreso")]
        [DataType(DataType.Date)]
        public DateTime FechaIngreso { get; set; }

        [Required(ErrorMessage = "El campo Carrera es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Carrera no puede superar los 100 caracteres.")]
        [Display(Name = "Carrera")]
        public string Carrera { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Facultad es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Facultad no puede superar los 100 caracteres.")]
        [Display(Name = "Facultad")]
        public string Facultad { get; set; } = string.Empty;

        [Range(1, 12, ErrorMessage = "El ciclo actual debe estar entre 1 y 12.")]
        [Display(Name = "Ciclo actual")]
        public int CicloActual { get; set; } = 1;

        [Range(0.0, 10.0, ErrorMessage = "El CUM debe estar entre 0 y 10.")]
        [Column(TypeName = "decimal(4,2)")]
        [Display(Name = "CUM")]
        public decimal CUM { get; set; }

        [Required(ErrorMessage = "El campo Estado académico es obligatorio.")]
        [StringLength(30, ErrorMessage = "El campo Estado académico no puede superar los 30 caracteres.")]
        [Display(Name = "Estado académico")]
        public string EstadoAcademico { get; set; } = "Activo";

        [Range(0, 500, ErrorMessage = "Los créditos aprobados deben estar entre 0 y 500.")]
        [Display(Name = "Créditos aprobados")]
        public int CreditosAprobados { get; set; }
    }
}
