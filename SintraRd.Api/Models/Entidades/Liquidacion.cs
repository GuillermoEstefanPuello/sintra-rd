// DEVGEP+ | Guillermo Estefan Puello
// Resultado consolidado de una liquidacion tributaria producido por el Motor de Liquidacion (M3).
// Registra la base, la tasa aplicada, el impuesto calculado, el credito fiscal y el total a pagar,
// junto con la version de regla usada para que el calculo sea reproducible en cualquier auditoria.
// La mecanica debito-credito del ITBIS esta respaldada en el Art. 346 de la Ley 11-92.

namespace SintraRd.Api.Models.Entidades;

public class Liquidacion
{
    public string Id { get; set; } = string.Empty;

    public TipoImpuesto TipoImpuesto { get; set; }

    // Periodo fiscal declarado en formato "YYYY-MM" (ej. "2026-01" para enero 2026)
    public string Periodo { get; set; } = string.Empty;

    public TipoOperacion TipoOperacion { get; set; }

    // Monto sobre el que se aplica la tasa segun tipo de operacion (Art. 339 Ley 11-92)
    public decimal MontoBase { get; set; }

    public TipoTasa TipoTasaAplicada { get; set; }

    // Tasa efectiva usada, expresada como fraccion decimal (ej. 0.18 para 18%)
    public decimal TasaAplicada { get; set; }

    // ITBIS generado en ventas (debito fiscal = MontoBase * TasaAplicada)
    public decimal MontoImpuesto { get; set; }

    // ITBIS pagado en compras del periodo, deducible del debito fiscal (Art. 346 Ley 11-92)
    public decimal CreditoFiscal { get; set; }

    // Impuesto neto a pagar: MontoImpuesto - CreditoFiscal
    public decimal TotalAPagar { get; set; }

    public DateTime FechaLiquidacion { get; set; }

    // Version de ReglaTributaria usada; permite recalcular correctamente periodos pasados
    public string VersionRegla { get; set; } = string.Empty;

    // Desglose linea por linea del calculo para transparencia y auditoria
    public List<DetalleLiquidacion> Detalles { get; set; } = new();
}
