using System.Net.Mail;
using Microsoft.EntityFrameworkCore;

namespace ControlAcademico.Negocio
{
    public class ErrorValidacion
    {
        public string Campo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
    }

    public class Resultado
    {
        public List<ErrorValidacion> Errores { get; } = new();
        public bool NoEncontrado { get; set; }
        public bool Exitoso => Errores.Count == 0;

        public Resultado Agregar(string campo, string mensaje)
        {
            Errores.Add(new ErrorValidacion { Campo = campo, Mensaje = mensaje });
            return this;
        }

        public static Resultado NoExiste(string mensaje)
        {
            var resultado = new Resultado { NoEncontrado = true };
            return resultado.Agregar(string.Empty, mensaje);
        }
    }

    public static class Validador
    {
        public static string Limpiar(string? valor)
        {
            return (valor ?? string.Empty).Trim();
        }

        // Colación de SQL Server que ignora mayúsculas y acentos (CI = sin distinguir mayúsculas, AI = sin distinguir acentos).
        public const string ColacionSinAcentos = "Latin1_General_CI_AI";

        // Divide el texto de búsqueda en palabras (máximo 5); cada palabra debe coincidir con algún campo.
        public static string[] Terminos(string? buscar)
        {
            return (buscar ?? string.Empty)
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.Length > 100 ? t.Substring(0, 100) : t)
                .Take(5)
                .ToArray();
        }

        public static void Texto(Resultado resultado, string campo, string valor, string etiqueta, int maximo)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                resultado.Agregar(campo, $"El campo {etiqueta} es obligatorio.");
            }
            else if (valor.Length > maximo)
            {
                resultado.Agregar(campo, $"El campo {etiqueta} no puede superar los {maximo} caracteres.");
            }
        }

        public static void Correo(Resultado resultado, string campo, string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                resultado.Agregar(campo, "El campo Correo electrónico es obligatorio.");
                return;
            }

            if (valor.Length > 150)
            {
                resultado.Agregar(campo, "El campo Correo electrónico no puede superar los 150 caracteres.");
                return;
            }

            if (!CorreoValido(valor))
            {
                resultado.Agregar(campo, "El correo electrónico no tiene un formato válido.");
            }
        }

        private static bool CorreoValido(string valor)
        {
            try
            {
                var direccion = new MailAddress(valor);
                return direccion.Address == valor && direccion.Host.Contains('.');
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static void Telefono(Resultado resultado, string campo, string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return;

            if (!System.Text.RegularExpressions.Regex.IsMatch(valor, @"^\d{4}-\d{4}$"))
            {
                resultado.Agregar(campo, "El teléfono debe tener el formato 0000-0000.");
            }
        }

        public static void FechaNacimiento(Resultado resultado, string campo, DateTime valor)
        {
            if (valor == default)
            {
                resultado.Agregar(campo, "El campo Fecha de nacimiento es obligatorio.");
            }
            else if (valor.Date > DateTime.Today)
            {
                resultado.Agregar(campo, "La fecha de nacimiento no puede ser futura.");
            }
            else if (valor.Year < 1900)
            {
                resultado.Agregar(campo, "La fecha de nacimiento no es válida.");
            }
        }

        public static async Task<Resultado> GuardarAsync(DbContext contexto, Resultado resultado, string mensajeError)
        {
            try
            {
                await contexto.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                resultado.Agregar(string.Empty, mensajeError);
            }
            return resultado;
        }
    }
}
