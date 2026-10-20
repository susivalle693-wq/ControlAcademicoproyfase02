using ControlAcademico.Entidades;
using Microsoft.EntityFrameworkCore;
using ActividadRegistro = ControlAcademico.Entidades.Actividad;

namespace ControlAcademico.Datos
{
    public class ControlAcademicoContext : DbContext
    {
        public ControlAcademicoContext(DbContextOptions<ControlAcademicoContext> options)
            : base(options) { }

        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<Docente> Docentes { get; set; }
        public DbSet<Curso> Cursos { get; set; }
        public DbSet<Matricula> Matriculas { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Actividad> Actividad { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var relacion in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relacion.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<Matricula>()
                .HasIndex(m => new { m.EstudianteId, m.CursoId })
                .IsUnique();

            modelBuilder.Entity<Matricula>().HasIndex(m => m.CodigoMatricula).IsUnique();

            modelBuilder.Entity<Calificacion>().Property(c => c.FechaRegistro).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<Actividad>().Property(a => a.Fecha).HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Estudiante>().HasIndex(e => e.Carnet).IsUnique();
            modelBuilder.Entity<Estudiante>().HasIndex(e => e.Correo).IsUnique();
            modelBuilder.Entity<Docente>().HasIndex(d => d.Correo).IsUnique();
            modelBuilder.Entity<Docente>().HasIndex(d => d.CodigoEmpleado).IsUnique();
            modelBuilder.Entity<Curso>().HasIndex(c => c.Codigo).IsUnique();
            modelBuilder.Entity<Curso>().HasIndex(c => c.Nombre).IsUnique();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            RegistrarActividad();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            RegistrarActividad();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void RegistrarActividad()
        {
            ChangeTracker.DetectChanges();

            var nuevas = new List<ActividadRegistro>();
            foreach (var entrada in ChangeTracker.Entries().ToList())
            {
                if (entrada.State != EntityState.Added && entrada.State != EntityState.Modified && entrada.State != EntityState.Deleted)
                    continue;

                var texto = Describir(entrada.Entity, entrada.State);
                if (texto == null) continue;

                if (texto.Length > 250) texto = texto.Substring(0, 250);
                nuevas.Add(new ActividadRegistro { Descripcion = texto, Fecha = DateTime.Now });
            }

            if (nuevas.Count > 0)
                Set<ActividadRegistro>().AddRange(nuevas);
        }

        private string? Describir(object entidad, EntityState estado)
        {
            switch (entidad)
            {
                case Estudiante e:
                    return $"Estudiante {Accion(estado, false)}: {e.Nombre} {e.Apellido} ({e.Carnet})";
                case Docente d:
                    return $"Docente {Accion(estado, false)}: {d.Nombre} {d.Apellido}";
                case Curso c:
                    return $"Curso {Accion(estado, false)}: {c.Nombre}";
                case Matricula m:
                    return $"Matrícula {Accion(estado, true)}: {NombreEstudiante(m.EstudianteId)} en {NombreCurso(m.CursoId)}";
                case Calificacion n:
                    return $"Calificación {Accion(estado, true)}: {n.Nota:0.##} para {NombreMatricula(n.MatriculaId)}";
                default:
                    return null;
            }
        }

        private static string Accion(EntityState estado, bool femenino)
        {
            return estado switch
            {
                EntityState.Added => femenino ? "registrada" : "registrado",
                EntityState.Modified => femenino ? "actualizada" : "actualizado",
                _ => femenino ? "eliminada" : "eliminado"
            };
        }

        private string NombreEstudiante(int id)
        {
            return Set<Estudiante>().AsNoTracking()
                .Where(e => e.Id == id)
                .Select(e => e.Nombre + " " + e.Apellido)
                .FirstOrDefault() ?? "estudiante";
        }

        private string NombreCurso(int id)
        {
            return Set<Curso>().AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => c.Nombre)
                .FirstOrDefault() ?? "curso";
        }

        private string NombreMatricula(int id)
        {
            var matricula = Set<Matricula>().AsNoTracking()
                .Where(m => m.Id == id)
                .Select(m => new { m.EstudianteId, m.CursoId })
                .FirstOrDefault();

            return matricula == null
                ? "una matrícula"
                : $"{NombreEstudiante(matricula.EstudianteId)} en {NombreCurso(matricula.CursoId)}";
        }
    }
}
