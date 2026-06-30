// DEVGEP+ | Guillermo Estefan Puello
// Contrato del repositorio de reglas tributarias (M16).
// Abstrae el origen de las reglas (JSON, base de datos, API externa) del Motor de Liquidacion,
// permitiendo cambiar la implementacion sin modificar el motor ni sus consumidores.

using SintraRd.Api.Models.Entidades;

namespace SintraRd.Api.Repositories.Interfaces;

public interface IReglaTributariaRepository
{
    // Retorna las reglas vigentes para el impuesto en la fecha indicada.
    // Retorna null si no existe ninguna regla activa para esa combinacion.
    Task<ReglaTributaria?> ObtenerVigenteAsync(TipoImpuesto tipoImpuesto, DateOnly fecha);

    // Retorna todas las versiones historicas de un impuesto, ordenadas por fecha de vigencia.
    // Util para auditoria y consulta de reglas pasadas.
    Task<IEnumerable<ReglaTributaria>> ObtenerTodasAsync(TipoImpuesto tipoImpuesto);
}
