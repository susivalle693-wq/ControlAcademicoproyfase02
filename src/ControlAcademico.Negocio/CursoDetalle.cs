namespace ControlAcademico.Negocio
{
    public class EstudianteMatriculado
    {
        public int MatriculaId { get; set; }
        public int EstudianteId { get; set; }
        public string Carnet { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public DateTime FechaMatricula { get; set; }
        public int TotalNotas { get; set; }
        public double? Promedio { get; set; }
    }

    public class CursoDetalle
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Creditos { get; set; }
        public int CupoMaximo { get; set; }
        public int CuposDisponibles { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int DocenteId { get; set; }
        public string DocenteNombre { get; set; } = string.Empty;
        public string DocenteCorreo { get; set; } = string.Empty;
        public string DocenteEspecialidad { get; set; } = string.Empty;
        public int TotalEstudiantes { get; set; }
        public double? PromedioGeneral { get; set; }
        public List<EstudianteMatriculado> Estudiantes { get; set; } = new();
    }
}
