// DEVGEP+ | Guillermo Estefan Puello
// Extensiones del contenedor de inyeccion de dependencias (DI) para SINTRA-RD.
// Centraliza el registro de todos los servicios propios del proyecto,
// manteniendo Program.cs limpio y con una sola linea de configuracion por bloque.

using System.Threading.RateLimiting;
using SintraRd.Api.Repositories;
using SintraRd.Api.Repositories.Interfaces;
using SintraRd.Api.Services;
using SintraRd.Api.Services.Interfaces;

namespace SintraRd.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AgregarServiciosSintraRd(this IServiceCollection services)
    {
        // Repositorio de reglas tributarias (M16): lee los JSON de MotorReglas/Reglas/
        services.AddScoped<IReglaTributariaRepository, ReglaTributariaRepository>();

        // Motor de Liquidacion Generico (M3): calcula impuestos con las reglas de M16
        services.AddScoped<IMotorLiquidacion, MotorLiquidacion>();

        return services;
    }

    // Lee la lista blanca de origenes desde appsettings.json (Cors:OrigenesPermitidos).
    // Nunca se usa un comodin abierto: solo los origenes declarados en configuracion son validos.
    public static IServiceCollection AgregarCors(
        this IServiceCollection services,
        IConfiguration configuracion)
    {
        var origenes = configuracion
            .GetSection("Cors:OrigenesPermitidos")
            .Get<string[]>() ?? Array.Empty<string>();

        services.AddCors(opciones =>
            opciones.AddPolicy("PoliticaCors", politica =>
                politica
                    .WithOrigins(origenes)
                    .WithMethods("GET", "POST")
                    .WithHeaders("Content-Type")));

        return services;
    }

    // Rate limiting nativo de .NET 9, sin paquetes externos.
    // Politica global: 100 solicitudes por minuto por IP (todas las rutas).
    // Politica "liquidaciones": 20 por minuto por IP (solo el endpoint de calculo fiscal).
    public static IServiceCollection AgregarRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(opciones =>
        {
            opciones.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Limite global aplicado a todas las rutas de la API
            opciones.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(contexto =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: contexto.Connection.RemoteIpAddress?.ToString() ?? "anonimo",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }));

            // Limite estricto para el endpoint de liquidacion: escritura fiscal con mayor riesgo
            opciones.AddPolicy("liquidaciones", contexto =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: contexto.Connection.RemoteIpAddress?.ToString() ?? "anonimo",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }));
        });

        return services;
    }
}
