using System;
using System.Collections.Generic;
using System.Linq;
using ControlAcademico.Modelos;

namespace ControlAcademico
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Prototipo Fase 1 - Sistema de Control Académico ===\n");

            // Listas genéricas (List<T>) para guardar todo en memoria,
            // tal como lo pide la Fase 1 (todavía sin base de datos).
            List<Persona> personas = new List<Persona>();
            List<Curso> cursos = new List<Curso>();
            List<Matricula> matriculas = new List<Matricula>();
            List<Calificacion> calificaciones = new List<Calificacion>();

            try
            {
                // ---------- Crear docentes y estudiantes (Persona 2 y 3) ----------
                Docente docente1 = new Docente(
                    id: 1,
                    nombre: "Carlos",
                    apellido: "Martínez",
                    email: "carlos.martinez@udb.edu.sv",
                    telefono: "7000-0001",
                    fechaNacimiento: new DateTime(1985, 4, 10),
                    codigoEmpleado: "DOC-001",
                    especialidad: "Programación");
                personas.Add(docente1);

                Estudiante estudiante1 = new Estudiante(
                    id: 1,
                    nombres: "Mario",
                    apellidos: "Pérez",
                    email: "mario.perez@udb.edu.sv",
                    telefono: "7000-0002",
                    fechaNacimiento: new DateTime(2003, 5, 12),
                    carnet: "CT-0001",
                    carrera: "Ingeniería en Sistemas",
                    facultad: "Ingeniería");
                personas.Add(estudiante1);

                Estudiante estudiante2 = new Estudiante(
                    id: 2,
                    nombres: "Ana",
                    apellidos: "López",
                    email: "ana.lopez@udb.edu.sv",
                    telefono: "7000-0003",
                    fechaNacimiento: new DateTime(2002, 11, 3),
                    carnet: "CT-0002",
                    carrera: "Ingeniería en Sistemas",
                    facultad: "Ingeniería");
                personas.Add(estudiante2);

                // ---------- Crear cursos (Persona 4) ----------
                Curso curso1 = new Curso(
                    codigo: "DSP404",
                    nombre: "Desarrollo de Aplicaciones con Software Propietario",
                    creditos: 4,
                    docenteAsignado: docente1.ObtenerNombreCompleto(),
                    cupoMaximo: 30);
                cursos.Add(curso1);

                // ---------- Matricular estudiantes en el curso (Persona 5) ----------
                Matricula matricula1 = new Matricula("MAT-0001", estudiante1, curso1);
                curso1.Matricular();
                matriculas.Add(matricula1);

                Matricula matricula2 = new Matricula("MAT-0002", estudiante2, curso1);
                curso1.Matricular();
                matriculas.Add(matricula2);

                // ---------- Registrar calificaciones (Persona 6) ----------
                calificaciones.Add(new Calificacion(1, estudiante1, curso1, 8.5));
                calificaciones.Add(new Calificacion(2, estudiante2, curso1, 5.0));

                // ---------- Menú interactivo ----------
                bool salir = false;

                while (!salir)
                {
                    Console.WriteLine("\n================ MENÚ ================");
                    Console.WriteLine("1. Listar personas (docentes y estudiantes)");
                    Console.WriteLine("2. Listar cursos");
                    Console.WriteLine("3. Listar matrículas");
                    Console.WriteLine("4. Listar calificaciones");
                    Console.WriteLine("5. Buscar estudiante por carnet");
                    Console.WriteLine("0. Salir");
                    Console.WriteLine("=======================================");
                    Console.Write("Elija una opción: ");

                    string opcion = Console.ReadLine();

                    switch (opcion)
                    {
                        case "1":
                            Console.WriteLine("\n>> PERSONAS REGISTRADAS (Docentes y Estudiantes)");
                            foreach (Persona p in personas)
                                p.MostrarInformacion();
                            break;

                        case "2":
                            Console.WriteLine("\n>> CURSOS");
                            foreach (Curso c in cursos)
                                c.MostrarDetalle();
                            break;

                        case "3":
                            Console.WriteLine("\n>> MATRÍCULAS");
                            foreach (Matricula m in matriculas)
                                m.MostrarDetalle();
                            break;

                        case "4":
                            Console.WriteLine("\n>> CALIFICACIONES");
                            foreach (Calificacion cal in calificaciones)
                                cal.MostrarDetalle();
                            break;

                        case "5":
                            // Buscar un estudiante por carnet (simula Pantalla de Detalle)
                            Console.Write("\nEscriba el carnet a buscar (ej: CT-0001): ");
                            string carnetBuscado = Console.ReadLine();

                            Estudiante encontrado = personas
                                .OfType<Estudiante>()
                                .FirstOrDefault(e => e.Carnet == carnetBuscado);

                            if (encontrado != null)
                            {
                                Console.WriteLine("Encontrado ->");
                                encontrado.MostrarInformacion();
                            }
                            else
                            {
                                Console.WriteLine("No se encontró ningún estudiante con ese carnet. " +
                                    "Los carnets de prueba disponibles son CT-0001 y CT-0002.");
                            }
                            break;

                        case "0":
                            salir = true;
                            Console.WriteLine("Saliendo del programa...");
                            break;

                        default:
                            // Manejo de errores: opción inválida no rompe el programa
                            Console.WriteLine("Opción no válida, intente de nuevo.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores solicitado en la Fase 1
                Console.WriteLine("Ocurrió un error: " + ex.Message);
            }

            Console.WriteLine("\nPresione una tecla para cerrar...");
            Console.ReadKey();
        }
    }
}