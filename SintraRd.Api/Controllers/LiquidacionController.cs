// DEVGEP+ | Guillermo Estefan Puello
// Controlador del Motor de Liquidacion (M3).
// Expone el calculo de impuestos como un endpoint HTTP versionado.
// [ApiController] maneja la validacion del modelo automaticamente (400 si el body es invalido).

using Microsoft.AspNetCore.Mvc;
using SintraRd.Api.Models.Dtos;
using SintraRd.Api.Services.Interfaces;

namespace SintraRd.Api.Controllers;

[ApiController]
[Route("api/v1/liquidaciones")]
public class LiquidacionController : ControllerBase
{
    private readonly IMotorLiquidacion _motorLiquidacion;

    public LiquidacionController(IMotorLiquidacion motorLiquidacion)
    {
        _motorLiquidacion = motorLiquidacion;
    }

    // POST api/v1/liquidaciones
    // Recibe una solicitud de liquidacion y retorna el calculo completo del impuesto.
    [HttpPost]
    public async Task<IActionResult> LiquidarAsync([FromBody] SolicitudLiquidacionDto solicitud)
    {
        try
        {
            var resultado = await _motorLiquidacion.LiquidarAsync(solicitud);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            // Logica de negocio no satisfecha: no hay regla vigente o la tasa no esta definida
            return UnprocessableEntity(new { error = ex.Message });
        }
    }
}
