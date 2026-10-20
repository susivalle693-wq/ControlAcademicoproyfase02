using System;

namespace ControlAcademico.Modelos
{
	// Representa un curso del sistema, implementa IRegistrablepara mostrar su detalle
	public class Curso : IRegistrable
	{
		private string codigo;
		private string nombre;
		private int creditos;
		private string docenteAsignado;
		private int cupoMaximo;
		private int inscritos;

		public string Codigo
		{
			get { return codigo; }
			set { codigo = value; }
		}

		public string Nombre
		{
			get { return nombre; }
			set { nombre = value; }
		}

		public int Creditos
		{
			get { return creditos; }
			set
			{
				//No tiene sentido un cursocon 0 o menos créditos
				if (value <= 0)
					throw new ArgumentException("Los créditos deben ser mayores a 0");
				creditos = value;
			}
		}

		public string DocenteAsignado
		{
			get { return docenteAsignado; }
			set { docenteAsignado = value; }
		}

		public int CupoMaximo
		{
			get { return cupoMaximo; }
			set
			{
				if (value <= 0)
					throw new ArgumentException("El cupo máximo debe ser mayor a 0");
				cupoMaximo = value;
			}
		}


		//Solo se puede leer desde afuera, se actualiza internamente al matricular

		public int Inscritos
		{
			get { return inscritos; }
			private set { inscritos = value; }
		}

		public Curso(string codigo, string nombre, int creditos, string docenteAsignado, int cupoMaximo)
		{
			Codigo = codigo;
			Nombre = nombre;
			Creditos = creditos;
			DocenteAsignado = docenteAsignado;
			CupoMaximo = cupoMaximo;
			inscritos = 0;
		}

		//Revisa si todavia caben más estudiantes en el curso
		public bool TieneCupoDisponible()
		{
			return inscritos < cupoMaximo;
		}

		//Aumenta el número de inscritos, solo si hay cupo
		public void Matricular()
		{
			if (!TieneCupoDisponible())
				throw new InvalidOperationException($"El curso {codigo} ya no tiene cupo disponible");
			inscritos++;
		}

		//Imprime en consola losdatos principales del curso
		public void MostrarDetalle()
		{
			Console.WriteLine("---------------------------------");
			Console.WriteLine($"Código:        {codigo}");
			Console.WriteLine($"Nombre:        {nombre}");
			Console.WriteLine($"Créditos:      {creditos}");
			Console.WriteLine($"Docente:       {docenteAsignado}");
			Console.WriteLine($"Cupo:          {inscritos}/{cupoMaximo}");
			Console.WriteLine("---------------------------------");
		}

		//Representación corta del curso, es útil para listarlo
		public override string ToString()
		{
			return $"{codigo} - {nombre} ({inscritos}/{cupoMaximo} cupos)";
		}
	}
}
