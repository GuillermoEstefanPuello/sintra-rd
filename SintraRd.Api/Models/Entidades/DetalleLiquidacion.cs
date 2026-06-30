// DEVGEP+ | Guillermo Estefan Puello
// Una linea del desglose de una liquidacion tributaria (sub-componente 3.8 de M3).
// Permite mostrar el calculo de forma transparente: base, impuesto, credito fiscal, total.

namespace SintraRd.Api.Models.Entidades;

public class DetalleLiquidacion
{
    // Descripcion del concepto (ej. "Base imponible", "ITBIS 18%", "Credito fiscal", "Total a pagar")
    public string Concepto { get; set; } = string.Empty;

    public decimal Monto { get; set; }
}
