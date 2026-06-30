// DEVGEP+ | Guillermo Estefan Puello
// Implementacion del repositorio de reglas tributarias basada en archivos JSON.
// Lee todos los archivos de MotorReglas/Reglas/, los deserializa y filtra
// por tipo de impuesto y ventana de vigencia (FechaVigenciaDesde / FechaVigenciaHasta).
// JsonStringEnumConverter permite que "General" en el JSON se lea como TipoTasa.General.

using System.Text.Json;
using System.Text.Json.Serialization;
using SintraRd.Api.Models.Entidades;
using SintraRd.Api.Repositories.Interfaces;

namespace SintraRd.Api.Repositories;

public class ReglaTributariaRepository : IReglaTributariaRepository
{
    private readonly string _carpetaReglas;
    private readonly JsonSerializerOptions _opcionesJson;

    public ReglaTributariaRepository(IWebHostEnvironment entorno)
    {
        _carpetaReglas = Path.Combine(entorno.ContentRootPath, "MotorReglas", "Reglas");
        _opcionesJson = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task<ReglaTributaria?> ObtenerVigenteAsync(TipoImpuesto tipoImpuesto, DateOnly fecha)
    {
        var todas = await ObtenerTodasAsync(tipoImpuesto);
        return todas.FirstOrDefault(r =>
            r.FechaVigenciaDesde <= fecha &&
            (r.FechaVigenciaHasta == null || r.FechaVigenciaHasta >= fecha));
    }

    public async Task<IEnumerable<ReglaTributaria>> ObtenerTodasAsync(TipoImpuesto tipoImpuesto)
    {
        if (!Directory.Exists(_carpetaReglas))
            return Enumerable.Empty<ReglaTributaria>();

        var archivos = Directory.GetFiles(_carpetaReglas, "*.json");
        var reglas = new List<ReglaTributaria>();

        foreach (var archivo in archivos)
        {
            await using var stream = File.OpenRead(archivo);
            var regla = await JsonSerializer.DeserializeAsync<ReglaTributaria>(stream, _opcionesJson);
            if (regla != null && regla.TipoImpuesto == tipoImpuesto)
                reglas.Add(regla);
        }

        return reglas.OrderBy(r => r.FechaVigenciaDesde);
    }
}
