// DEVGEP+ | Guillermo Estefan Puello
// Objeto de entrada para una solicitud de liquidacion tributaria.
// El llamador especifica el tipo de tasa porque el motor no tiene catalogo de productos todavia;
// el motor usa ese dato para buscar el valor exacto en la ReglaTributaria vigente.

using SintraRd.Api.Models.Entidades;

namespace SintraRd.Api.Models.Dtos;

public class SolicitudLiquidacionDto
{
    // Impuesto a liquidar (ITBIS para el demo tecnico)
    public TipoImpuesto TipoImpuesto { get; set; }

    // Hecho generador de la operacion (Art. 335 Ley 11-92)
    public TipoOperacion TipoOperacion { get; set; }

    // Categoria de tasa que aplica al bien o servicio: General, Reducida, TasaCero o Exento
    public TipoTasa TipoTasa { get; set; }

    // Base imponible de la operacion (Art. 339 Ley 11-92)
    public decimal MontoBase { get; set; }

    // ITBIS pagado en compras del periodo, deducible del debito fiscal (Art. 346 Ley 11-92)
    public decimal CreditoFiscal { get; set; } = 0m;

    // Periodo fiscal declarado en formato "YYYY-MM" (ej. "2026-06")
    public string Periodo { get; set; } = string.Empty;

    // Fecha real de la operacion: determina que version de reglas carga el motor
    public DateOnly FechaOperacion { get; set; }
}
