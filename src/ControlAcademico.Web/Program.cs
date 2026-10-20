using System.Globalization;
using System.Text.Json.Serialization;
using ControlAcademico.Datos;
using ControlAcademico.Negocio;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ControlAcademicoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IEstudianteServicio, EstudianteServicio>();
builder.Services.AddScoped<IDocenteServicio, DocenteServicio>();
builder.Services.AddScoped<ICursoServicio, CursoServicio>();
builder.Services.AddScoped<IMatriculaServicio, MatriculaServicio>();
builder.Services.AddScoped<ICalificacionServicio, CalificacionServicio>();
builder.Services.AddScoped<IConsultaEstudianteServicio, ConsultaEstudianteServicio>();
builder.Services.AddScoped<IDashboardServicio, DashboardServicio>();

builder.Services.AddControllersWithViews(opciones =>
{
    opciones.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;

    var mensajes = opciones.ModelBindingMessageProvider;
    mensajes.SetValueMustNotBeNullAccessor(campo => $"El campo {campo} es obligatorio.");
    mensajes.SetMissingBindRequiredValueAccessor(campo => $"Falta el valor del campo {campo}.");
    mensajes.SetMissingKeyOrValueAccessor(() => "Se requiere un valor.");
    mensajes.SetMissingRequestBodyRequiredValueAccessor(() => "El cuerpo de la solicitud es obligatorio.");
    mensajes.SetAttemptedValueIsInvalidAccessor((valor, campo) => $"El valor '{valor}' no es válido para el campo {campo}.");
    mensajes.SetNonPropertyAttemptedValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
    mensajes.SetUnknownValueIsInvalidAccessor(campo => $"El valor proporcionado no es válido para el campo {campo}.");
    mensajes.SetNonPropertyUnknownValueIsInvalidAccessor(() => "El valor proporcionado no es válido.");
    mensajes.SetValueIsInvalidAccessor(valor => $"El valor '{valor}' no es válido.");
    mensajes.SetValueMustBeANumberAccessor(campo => $"El campo {campo} debe ser un número.");
    mensajes.SetNonPropertyValueMustBeANumberAccessor(() => "El campo debe ser un número.");
})
.AddJsonOptions(opciones =>
{
    opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Control Académico API",
        Version = "v1",
        Description = "Servicio web para estudiantes, docentes, cursos, matrículas y calificaciones."
    });
    opciones.DocInclusionPredicate((documento, api) =>
        api.RelativePath != null && api.RelativePath.StartsWith("api/"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(opciones =>
{
    opciones.SwaggerEndpoint("/swagger/v1/swagger.json", "Control Académico API v1");
    opciones.DocumentTitle = "Control Académico - Servicio web";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

// Cultura invariante: los campos numéricos (CUM, notas) llegan con punto decimal sin importar el idioma del equipo.
var culturaInvariante = new[] { CultureInfo.InvariantCulture };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(CultureInfo.InvariantCulture),
    SupportedCultures = culturaInvariante,
    SupportedUICultures = culturaInvariante
});

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
