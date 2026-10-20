using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ControlAcademico.Entidades
{
    public abstract class Persona
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Nombre no puede superar los 100 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Apellido es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Apellido no puede superar los 100 caracteres.")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El campo Correo electrónico es obligatorio.")]
        [StringLength(150, ErrorMessage = "El campo Correo electrónico no puede superar los 150 caracteres.")]
        [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = string.Empty;

        [StringLength(9, ErrorMessage = "El campo Teléfono no puede superar los 9 caracteres.")]
        [RegularExpression(@"^\d{4}-\d{4}$", ErrorMessage = "El teléfono debe tener el formato 0000-0000.")]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [Display(Name = "Fecha de nacimiento")]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellido}";
    }
}
