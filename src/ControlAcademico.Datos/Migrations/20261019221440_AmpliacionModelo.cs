using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControlAcademico.Migrations
{
    /// <inheritdoc />
    public partial class AmpliacionModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoMatricula",
                table: "Matriculas",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CUM",
                table: "Estudiantes",
                type: "decimal(4,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Carrera",
                table: "Estudiantes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CicloActual",
                table: "Estudiantes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CreditosAprobados",
                table: "Estudiantes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EstadoAcademico",
                table: "Estudiantes",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Facultad",
                table: "Estudiantes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Estudiantes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Estudiantes",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CodigoEmpleado",
                table: "Docentes",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaNacimiento",
                table: "Docentes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Telefono",
                table: "Docentes",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Cursos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Creditos",
                table: "Cursos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CupoMaximo",
                table: "Cursos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Calificaciones",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "Calificaciones",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()");

            migrationBuilder.CreateTable(
                name: "Actividad",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actividad", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_CodigoMatricula",
                table: "Matriculas",
                column: "CodigoMatricula",
                unique: true,
                filter: "[CodigoMatricula] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_CodigoEmpleado",
                table: "Docentes",
                column: "CodigoEmpleado",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Codigo",
                table: "Cursos",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actividad");

            migrationBuilder.DropIndex(
                name: "IX_Matriculas_CodigoMatricula",
                table: "Matriculas");

            migrationBuilder.DropIndex(
                name: "IX_Docentes_CodigoEmpleado",
                table: "Docentes");

            migrationBuilder.DropIndex(
                name: "IX_Cursos_Codigo",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "CodigoMatricula",
                table: "Matriculas");

            migrationBuilder.DropColumn(
                name: "CUM",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Carrera",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "CicloActual",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "CreditosAprobados",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "EstadoAcademico",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Facultad",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Estudiantes");

            migrationBuilder.DropColumn(
                name: "CodigoEmpleado",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "FechaNacimiento",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "Telefono",
                table: "Docentes");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "Creditos",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "CupoMaximo",
                table: "Cursos");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Calificaciones");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "Calificaciones");
        }
    }
}
