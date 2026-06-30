// DEVGEP+ | Guillermo Estefan Puello
// Tipos de impuesto que el Motor de Liquidacion puede procesar.

namespace SintraRd.Api.Models.Entidades;

public enum TipoImpuesto
{
    // Impuesto sobre Transferencias de Bienes Industrializados y Servicios (Ley 11-92, Titulo III)
    ITBIS,
    // Impuesto Sobre la Renta (Ley 11-92, Titulo II)
    ISR,
    // Impuesto Selectivo al Consumo (Ley 11-92, Titulo IV)
    ISC,
    // Impuesto al Patrimonio Inmobiliario (Ley 18-88)
    IPI,
    // Impuesto sobre Activos (Ley 11-92, Titulo V)
    Activos
}
