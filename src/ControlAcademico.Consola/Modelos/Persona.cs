using System;

namespace ControlAcademico.Modelos
{
    /// <summary>
    /// Clase abstracta base del sistema de Control Académico.
    /// Representa los datos y comportamientos comunes a cualquier
    /// persona dentro del sistema (Estudiante, Docente, etc.).
    /// Al ser abstracta, no se puede instanciar directamente:
    /// solo sirve como "molde" para que otras clases hereden de ella.
    /// Desarrollado por: Persona 1
    /// </summary>
    public abstract class Persona
    {
        // ---------- Atributos (encapsulados con propiedades) ----------

        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }

        // ---------- Constructores ----------

        // Constructor vacío: útil cuando se quiere crear el objeto e
        // ir llenando los datos poco a poco (lo usa Estudiante y Docente)
        protected Persona()
        {
        }

        /// <summary>
        /// Constructor con todos los datos base. Las clases hijas
        /// (Estudiante, Docente) lo llaman con ": base(...)".
        /// </summary>
        protected Persona(int id, string nombre, string apellido, string email, string telefono, DateTime fechaNacimiento)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre no puede estar vacío.");

            Id = id;
            Nombre = nombre;
            Apellido = apellido;
            Email = email;
            Telefono = telefono;
            FechaNacimiento = fechaNacimiento;
        }

        // ---------- Métodos ----------

        // Junta nombre y apellido en un solo texto, para no repetir
        // esta concatenación en cada clase hija
        public string ObtenerNombreCompleto()
        {
            return $"{Nombre} {Apellido}".Trim();
        }

        // Calcula la edad actual a partir de la fecha de nacimiento
        public int ObtenerEdad()
        {
            if (FechaNacimiento == default(DateTime))
                return 0;

            int edad = DateTime.Now.Year - FechaNacimiento.Year;
            if (FechaNacimiento.Date > DateTime.Now.AddYears(-edad))
                edad--;

            return edad;
        }

        /// <summary>
        /// Método abstracto: cada clase hija decide cómo mostrar su
        /// propia información en consola (esto es lo que obliga el
        /// polimorfismo: cada tipo de Persona se muestra distinto).
        /// </summary>
        public abstract void MostrarInformacion();
    }
}
