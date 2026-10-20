namespace ControlAcademico.Negocio
{
    // Reglas académicas definidas una sola vez (la nota mínima NO se repite en otros archivos).
    public static class ReglasAcademicas
    {
        public const double NotaMinimaAprobacion = 6.0;

        public const string Aprobado = "Aprobado";
        public const string Reprobado = "Reprobado";
        public const string Pendiente = "Pendiente";

        // Aprobado / Reprobado según la nota mínima; Pendiente si todavía no hay nota.
        public static string ObtenerEstado(double? nota)
        {
            if (nota == null) return Pendiente;
            return nota.Value >= NotaMinimaAprobacion ? Aprobado : Reprobado;
        }

        // Promedio de una lista de notas (null si no hay notas), con 2 decimales.
        public static double? CalcularPromedio(IEnumerable<double> notas)
        {
            var lista = notas.ToList();
            if (lista.Count == 0) return null;
            return Math.Round(lista.Average(), 2);
        }
    }
}
