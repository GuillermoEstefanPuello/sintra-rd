// DEVGEP+ | Guillermo Estefan Puello
// Una tasa tributaria dentro de un conjunto de reglas del Motor de Reglas (M16).
// Valor siempre en decimal: 0.18 para 18%, 0.16 para 16%, 0.00 para tasa cero.

namespace SintraRd.Api.Models.Entidades;

public class Tasa
{
    public TipoTasa Tipo { get; set; }

    // Valor porcentual expresado como fraccion decimal (0.18, 0.16, 0.00)
    public decimal Valor { get; set; }

    public string Descripcion { get; set; } = string.Empty;
}
