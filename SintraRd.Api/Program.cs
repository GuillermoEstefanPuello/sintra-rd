// DEVGEP+ | Guillermo Estefan Puello
// Punto de entrada de la aplicacion SintraRd.Api.
// Configura los servicios y el pipeline HTTP de ASP.NET Core.

using System.Text.Json.Serialization;
using SintraRd.Api.Infrastructure;
using SintraRd.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

// --- Servicios ---
// Habilita el enrutamiento por controladores (arquitectura MVC sin vistas)
// JsonStringEnumConverter permite que la API acepte enums como strings ("ITBIS", "General")
builder.Services.AddControllers()
    .AddJsonOptions(opciones =>
        opciones.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Registra los servicios propios de SINTRA-RD (repositorios, motor, etc.)
builder.Services.AgregarServiciosSintraRd();

// CORS con lista blanca de origenes leida desde appsettings.json
builder.Services.AgregarCors(builder.Configuration);

// Rate limiting nativo: limite global + limite estricto para el endpoint de liquidacion
builder.Services.AgregarRateLimiting();

// OpenAPI (documentacion interactiva de la API, disponible solo en desarrollo)
builder.Services.AddOpenApi();

var app = builder.Build();

// --- Pipeline HTTP ---
if (app.Environment.IsDevelopment())
{
    // Expone el endpoint /openapi/v1.json solo en entorno de desarrollo
    app.MapOpenApi();
}
else
{
    // HSTS (HTTP Strict Transport Security): fuerza HTTPS en produccion
    app.UseHsts();
}

// Redirige trafico HTTP a HTTPS
app.UseHttpsRedirection();

// Cabeceras de seguridad HTTP en todas las respuestas (equivalente a Helmet)
app.UseMiddleware<CabecerasSeguridad>();

// CORS: valida el origen de la solicitud contra la lista blanca de appsettings.json
app.UseCors("PoliticaCors");

// Rate limiting: aplica los limites configurados antes de que llegue a los controladores
app.UseRateLimiter();

// Middleware de autorizacion (se expande en el Hito 4.3 con JWT)
app.UseAuthorization();

// Mapea las rutas definidas en los controladores
app.MapControllers();

app.Run();
