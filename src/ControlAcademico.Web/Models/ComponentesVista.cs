namespace ControlAcademico.Models
{
    // Datos para el parcial de tarjeta de estadística del dashboard.
    public class TarjetaEstadistica
    {
        public string Titulo { get; set; } = string.Empty;
        public int Valor { get; set; }
        public string Icono { get; set; } = "bi-bar-chart";
        public string Controlador { get; set; } = string.Empty;
        public string ClaseBorde { get; set; } = string.Empty;
    }

    // Datos para el parcial de botones de acción de cada fila (editar, ver, eliminar).
    public class AccionesFila
    {
        public AccionesFila(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }

        public int Id { get; }
        public string Nombre { get; }
    }
}