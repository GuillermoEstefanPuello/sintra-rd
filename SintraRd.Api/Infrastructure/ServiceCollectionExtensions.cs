// DEVGEP+ | Guillermo Estefan Puello
// Extensiones del contenedor de inyeccion de dependencias (DI) para SINTRA-RD.
// Centraliza el registro de todos los servicios propios del proyecto,
// manteniendo Program.cs limpio y con una sola linea de configuracion.

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
}
