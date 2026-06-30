// DEVGEP+ | Guillermo Estefan Puello
// Conjunto de reglas tributarias versionado por fecha de vigencia (nucleo de M16).
// Implementa el principio central del proyecto: la ley es configuracion, no codigo.
// Cuando cambia una ley, se agrega un nuevo registro con su fecha de vigencia;
// el Motor de Liquidacion carga las reglas vigentes a la fecha de la operacion
// y nunca necesita ser modificado ni recompilado.

namespace SintraRd.Api.Models.Entidades;

public class ReglaTributaria
{
    public string Id { get; set; } = string.Empty;

    public TipoImpuesto TipoImpuesto { get; set; }

    // Fecha desde la que estas reglas estan en vigor
    public DateOnly FechaVigenciaDesde { get; set; }

    // Nulo indica que la regla sigue vigente (sin fecha de cierre definida)
    public DateOnly? FechaVigenciaHasta { get; set; }

    // Identificador de version para trazabilidad y auditoria (ej. "ITBIS-2016-v1")
    public string Version { get; set; } = string.Empty;

    // Referencia a la norma o resolucion que origina esta version de reglas
    public string BaseLegal { get; set; } = string.Empty;

    public List<Tasa> Tasas { get; set; } = new();

    public List<Exencion> Exenciones { get; set; } = new();
}
