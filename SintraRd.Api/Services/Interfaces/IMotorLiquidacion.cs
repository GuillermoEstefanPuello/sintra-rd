// DEVGEP+ | Guillermo Estefan Puello
// Contrato del Motor de Liquidacion Generico (M3).
// Recibe una solicitud de liquidacion y retorna el resultado completo del calculo,
// incluyendo base, tasa aplicada, impuesto, credito fiscal y total a pagar.

using SintraRd.Api.Models.Dtos;
using SintraRd.Api.Models.Entidades;

namespace SintraRd.Api.Services.Interfaces;

public interface IMotorLiquidacion
{
    // Calcula el impuesto indicado en la solicitud usando las reglas vigentes a la fecha de operacion.
    Task<Liquidacion> LiquidarAsync(SolicitudLiquidacionDto solicitud);
}
