// DEVGEP+ | Guillermo Estefan Puello
// Una exencion tributaria del Motor de Reglas (M16).
// Para el ITBIS, las exenciones de bienes estan en el Art. 343 y las de servicios en el Art. 344 (Ley 11-92).

namespace SintraRd.Api.Models.Entidades;

public class Exencion
{
    // Codigo de categoria del bien o servicio exento (ej. "ALIM-BASICO", "SALUD", "EDUCACION")
    public string Codigo { get; set; } = string.Empty;

    public string Descripcion { get; set; } = string.Empty;

    // Articulo y ley que ampara la exencion (ej. "Art. 343 Ley 11-92")
    public string BaseLegal { get; set; } = string.Empty;
}
