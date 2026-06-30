// DEVGEP+ | Guillermo Estefan Puello
// Extensiones del contenedor de inyeccion de dependencias (DI) para SINTRA-RD.
// Centraliza el registro de todos los servicios propios del proyecto,
// manteniendo Program.cs limpio y con una sola linea de configuracion.

using SintraRd.Api.Repositories;
using SintraRd.Api.Repositories.Interfaces;

namespace SintraRd.Api.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AgregarServiciosSintraRd(this IServiceCollection services)
    {
        // Repositorio de reglas tributarias (M16): lee los JSON de MotorReglas/Reglas/
        services.AddScoped<IReglaTributariaRepository, ReglaTributariaRepository>();
        return services;
    }
}
