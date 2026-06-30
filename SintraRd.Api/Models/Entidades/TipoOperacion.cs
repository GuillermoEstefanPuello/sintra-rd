// DEVGEP+ | Guillermo Estefan Puello
// Hechos generadores del ITBIS segun Art. 335 de la Ley 11-92.
// La base imponible se calcula de forma distinta segun el tipo de operacion (Art. 339).

namespace SintraRd.Api.Models.Entidades;

public enum TipoOperacion
{
    // Transferencia de bienes industrializados en el mercado local
    TransferenciaBienes,
    // Importacion de bienes industrializados (base: valor CIF mas aranceles)
    Importacion,
    // Prestacion o locacion de servicios gravados (base: valor total del servicio)
    Servicio
}
