using System;

namespace ControlAcademico.Modelos
{
    /// <summary>
    /// Representa la relación entre un Estudiante y un Curso
    /// (una matrícula). Desarrollado por: Persona 5
    /// </summary>
    public class Matricula
    {
        public string IdMatricula { get; private set; }
        public Estudiante Alumno { get; private set; }
        public Curso CursoAsignado { get; private set; }
        public DateTime FechaMatricula { get; private set; }

        public Matricula(string idMatricula, Estudiante alumno, Curso curso)
        {
            IdMatricula = !string.IsNullOrEmpty(idMatricula) ? idMatricula : throw new ArgumentException("ID inválido.");
            Alumno = alumno ?? throw new ArgumentNullException(nameof(alumno));
            CursoAsignado = curso ?? throw new ArgumentNullException(nameof(curso));
            FechaMatricula = DateTime.Now;
        }

        public void MostrarDetalle()
        {
            Console.WriteLine($"[Matrícula] ID: {IdMatricula} | Fecha: {FechaMatricula.ToShortDateString()}");
            Console.WriteLine($" -> Estudiante: {Alumno.ObtenerNombreCompleto()}");
            Console.WriteLine($" -> Curso: {CursoAsignado.Nombre}");
        }
    }
}
