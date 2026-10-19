# Sistema de Control Académico

Proyecto de Cátedra, **FASE 02**

## Descripción

Sistema para el registro y control de estudiantes, docentes, cursos, matrículas y calificaciones de una institución educativa.

Este proyecto es la **ampliación de la Fase 1**, que modelaba las clases principales en C# con Programación Orientada a Objetos (herencia, clase abstracta e interfaces) y un prototipo en consola. En la Fase 2 el mismo sistema se convierte en una aplicación web conectada a una base de datos, con arquitectura en capas y un servicio web.

## Nuevas funcionalidades:

- Aplicación web con **ASP.NET Core MVC** (.NET 8).
- Persistencia en **SQL Server** con **Entity Framework Core** (Code First y migraciones).
- **Arquitectura en capas**: Presentación, Lógica de negocio y Acceso a datos. Las páginas nunca acceden directamente a la base de datos.
- CRUD completo de Estudiantes, Docentes, Cursos, Matrículas y Calificaciones.
- Validaciones en formularios (cliente y servidor) y manejo de errores con mensajes claros.
- **Servicio web** (Web API) para consultar las notas y el estado de matrícula de un estudiante por carnet, documentado con Swagger.
- Dashboard con conteos y actividad reciente.
- Diseño nuevo de la interfaz, distinto al wireframe de la Fase 1.

## Tecnologías utilizadas

- C# y .NET 8
- ASP.NET Core MVC
- Entity Framework Core 8
- SQL Server (SSMS)
- Bootstrap 5
- Swagger
- Git / GitHub

## Estructura del proyecto

```
ControlAcademico-fase2/
├── ControlAcademico.sln
├── README.md
├── src/
│   ├── ControlAcademico.Entidades/   (clases del modelo: Persona, Estudiante, Docente, Curso, Matricula, Calificacion)
│   ├── ControlAcademico.Datos/       (DbContext, migraciones y repositorios)
│   ├── ControlAcademico.Negocio/     (servicios con las reglas del sistema)
│   ├── ControlAcademico.Web/         (controladores, vistas y Web API)
│   └── ControlAcademico.Consola/     (prototipo en consola de la Fase 1)
├── wireframes/                       (wireframes de la Fase 1)
└── docs/                             (diagramas y capturas)
```

| Capa | Proyecto | Puede llamar a |
| --- | --- | --- |
| Presentación | ControlAcademico.Web | Negocio |
| Lógica de negocio | ControlAcademico.Negocio | Datos y Entidades |
| Acceso a datos | ControlAcademico.Datos | Entidades |
| Modelo | ControlAcademico.Entidades | nadie |


## Integrantes del equipo

| Integrante | Carnet |
| --- | --- |
| Susana Nicole Valle Méndez | VM253306 |
| Alessandra Guadalupe González Burgos | GB253116 |

## Cómo ejecutar el proyecto

**Requisitos:** .NET 8 SDK (o superior con el runtime 8), SQL Server (LocalDB o SQL Server Express) y Visual Studio 2022 o superior.

1. Clonar el repositorio:
```bash
   git clone https://github.com/susivalle693-wq/ControlAcademico-fase2.git
```
2. Abrir `ControlAcademico.sln` con Visual Studio.
3. Ajustar la cadena de conexión `Default` en `src/ControlAcademico.Web/appsettings.json` (o en `appsettings.Development.json`) con el servidor de SQL Server de su equipo.
4. Crear la base de datos aplicando las migraciones:
```bash
   dotnet ef database update --project src/ControlAcademico.Datos --startup-project src/ControlAcademico.Web
```
5. Ejecutar la aplicación web:
```bash
   dotnet run --project src/ControlAcademico.Web
```

El prototipo de la Fase 1 sigue disponible:
```bash
dotnet run --project src/ControlAcademico.Consola
```
