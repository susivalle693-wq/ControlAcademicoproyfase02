using System;

namespace ControlAcademico.Modelos
{
    /// <summary>
    /// Representa a un Docente dentro del sistema de Control Académico.
    /// Hereda de la clase abstracta base Persona.
    /// Desarrollado por: Persona 3
    /// </summary>
    public class Docente : Persona
    {
        public string CodigoEmpleado { get; set; }
        public string Especialidad { get; set; }

        public Docente() : base()
        {
        }

        public Docente(
            int id,
            string nombre,
            string apellido,
            string email,
            string telefono,
            DateTime fechaNacimiento,
            string codigoEmpleado,
            string especialidad)
            : base(id, nombre, apellido, email, telefono, fechaNacimiento)
        {
            if (string.IsNullOrWhiteSpace(codigoEmpleado))
                throw new ArgumentException("El código de empleado no puede estar vacío.", nameof(codigoEmpleado));

            CodigoEmpleado = codigoEmpleado.Trim().ToUpper();
            Especialidad = especialidad;
        }

        // Implementación obligatoria del método abstracto de Persona
        public override void MostrarInformacion()
        {
            Console.WriteLine("==================================================");
            Console.WriteLine($"👨‍🏫 FICHA DE DOCENTE - {CodigoEmpleado}");
            Console.WriteLine("==================================================");
            Console.WriteLine($"Nombre Completo : {ObtenerNombreCompleto()}");
            Console.WriteLine($"Código Empleado : {CodigoEmpleado}");
            Console.WriteLine($"Especialidad    : {Especialidad}");
            Console.WriteLine($"Correo          : {Email}");
            Console.WriteLine($"Teléfono        : {Telefono}");
            Console.WriteLine("==================================================");
        }

        public override string ToString()
        {
            return $"[{CodigoEmpleado}] {ObtenerNombreCompleto()} - {Especialidad}";
        }
    }
}
