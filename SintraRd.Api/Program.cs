// DEVGEP+ | Guillermo Estefan Puello
// Punto de entrada de la aplicacion SintraRd.Api.
// Configura los servicios y el pipeline HTTP de ASP.NET Core.

var builder = WebApplication.CreateBuilder(args);

// --- Servicios ---
// Habilita el enrutamiento por controladores (arquitectura MVC sin vistas)
builder.Services.AddControllers();

// OpenAPI (documentacion interactiva de la API, disponible solo en desarrollo)
builder.Services.AddOpenApi();

var app = builder.Build();

// --- Pipeline HTTP ---
if (app.Environment.IsDevelopment())
{
    // Expone el endpoint /openapi/v1.json solo en entorno de desarrollo
    app.MapOpenApi();
}

// Redirige trafico HTTP a HTTPS
app.UseHttpsRedirection();

// Middleware de autorizacion (se expande en el hito de seguridad)
app.UseAuthorization();

// Mapea las rutas definidas en los controladores
app.MapControllers();

app.Run();
