// DEVGEP+ | Guillermo Estefan Puello
// Estructura de tasas del ITBIS segun Art. 23 Ley 253-12 y Fase 2A, seccion 3.1.

namespace SintraRd.Api.Models.Entidades;

public enum TipoTasa
{
    // Tasa general del 18%, aplica a la mayoria de bienes manufacturados y servicios gravados
    General,
    // Tasa reducida del 16%, aplica a productos especificos: yogurt, mantequilla, cafe, cacao, etc.
    Reducida,
    // Tasa cero para exportaciones; conserva el derecho a credito fiscal
    TasaCero,
    // Operacion exenta; no genera obligacion de ITBIS ni credito fiscal
    Exento
}
