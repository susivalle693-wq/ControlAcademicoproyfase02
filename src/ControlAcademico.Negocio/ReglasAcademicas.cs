namespace ControlAcademico.Negocio
{
    public static class ReglasAcademicas
    {
        public const double NotaMinimaAprobacion = 6.0;

        public const string Aprobado = "Aprobado";
        public const string Reprobado = "Reprobado";
        public const string Pendiente = "Pendiente";

        public static string ObtenerEstado(double? nota)
        {
            if (nota == null) return Pendiente;
            return nota.Value >= NotaMinimaAprobacion ? Aprobado : Reprobado;
        }

        public static double? CalcularPromedio(IEnumerable<double> notas)
        {
            var lista = notas.ToList();
            if (lista.Count == 0) return null;
            return Math.Round(lista.Average(), 2);
        }
    }
}
