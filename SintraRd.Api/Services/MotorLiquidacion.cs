// DEVGEP+ | Guillermo Estefan Puello
// Motor de Liquidacion Generico (M3): calcula cualquier impuesto ejecutando
// las reglas que le entrega el Motor de Reglas (M16 via IReglaTributariaRepository).
// Implementa los sub-componentes 3.1 a 3.8 definidos en la Fase 4B de la documentacion.
// Principio central: el motor no conoce ninguna tasa ni regla; todo viene de configuracion.

using SintraRd.Api.Models.Dtos;
using SintraRd.Api.Models.Entidades;
using SintraRd.Api.Repositories.Interfaces;
using SintraRd.Api.Services.Interfaces;

namespace SintraRd.Api.Services;

public class MotorLiquidacion : IMotorLiquidacion
{
    private readonly IReglaTributariaRepository _repositorioReglas;

    public MotorLiquidacion(IReglaTributariaRepository repositorioReglas)
    {
        _repositorioReglas = repositorioReglas;
    }

    public async Task<Liquidacion> LiquidarAsync(SolicitudLiquidacionDto solicitud)
    {
        // Sub-componente 3.3: carga las reglas vigentes a la fecha de la operacion
        var regla = await _repositorioReglas.ObtenerVigenteAsync(
            solicitud.TipoImpuesto,
            solicitud.FechaOperacion);

        if (regla == null)
            throw new InvalidOperationException(
                $"No existe una regla tributaria vigente para {solicitud.TipoImpuesto} " +
                $"en la fecha {solicitud.FechaOperacion:yyyy-MM-dd}.");

        // Sub-componente 3.4: determina la tasa aplicable segun la categoria informada
        var tasa = regla.Tasas.FirstOrDefault(t => t.Tipo == solicitud.TipoTasa);

        if (tasa == null)
            throw new InvalidOperationException(
                $"La regla {regla.Version} no define una tasa de tipo '{solicitud.TipoTasa}'.");

        // Sub-componente 3.5: operacion exenta — no genera impuesto ni credito fiscal
        if (solicitud.TipoTasa == TipoTasa.Exento)
            return ConstruirLiquidacion(solicitud, regla, tasa, montoImpuesto: 0m, totalAPagar: 0m);

        // Sub-componente 3.4: calculo del impuesto sobre la base imponible
        var montoImpuesto = solicitud.MontoBase * tasa.Valor;

        // Sub-componente 3.8: aplica credito fiscal (debito - credito, Art. 346 Ley 11-92)
        // El resultado puede ser negativo: indica saldo a favor del contribuyente
        var totalAPagar = montoImpuesto - solicitud.CreditoFiscal;

        return ConstruirLiquidacion(solicitud, regla, tasa, montoImpuesto, totalAPagar);
    }

    // Consolida el resultado final de la liquidacion con su desglose (sub-componente 3.8)
    private static Liquidacion ConstruirLiquidacion(
        SolicitudLiquidacionDto solicitud,
        Models.Entidades.ReglaTributaria regla,
        Models.Entidades.Tasa tasa,
        decimal montoImpuesto,
        decimal totalAPagar)
    {
        var porcentaje = tasa.Valor * 100;
        var detalles = new List<DetalleLiquidacion>
        {
            new() { Concepto = "Base imponible", Monto = solicitud.MontoBase },
            new() { Concepto = $"{solicitud.TipoImpuesto} {porcentaje:0.##}%", Monto = montoImpuesto }
        };

        if (solicitud.CreditoFiscal > 0m)
            detalles.Add(new() { Concepto = "Credito fiscal", Monto = -solicitud.CreditoFiscal });

        detalles.Add(new()
        {
            Concepto = totalAPagar < 0m ? "Saldo a favor" : "Total a pagar",
            Monto = totalAPagar
        });

        return new Liquidacion
        {
            Id = Guid.NewGuid().ToString(),
            TipoImpuesto = solicitud.TipoImpuesto,
            Periodo = solicitud.Periodo,
            TipoOperacion = solicitud.TipoOperacion,
            MontoBase = solicitud.MontoBase,
            TipoTasaAplicada = tasa.Tipo,
            TasaAplicada = tasa.Valor,
            MontoImpuesto = montoImpuesto,
            CreditoFiscal = solicitud.CreditoFiscal,
            TotalAPagar = totalAPagar,
            FechaLiquidacion = DateTime.UtcNow,
            VersionRegla = regla.Version,
            Detalles = detalles
        };
    }
}
