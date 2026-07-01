// DEVGEP+ | Guillermo Estefan Puello
// Implementacion falsa de IReglaTributariaRepository para pruebas unitarias.
// Recibe la regla a devolver en el constructor; permite controlar
// el comportamiento del repositorio sin acceso al sistema de archivos.

using SintraRd.Api.Models.Entidades;
using SintraRd.Api.Repositories.Interfaces;

namespace SintraRd.Tests.Fakes;

public class ReglaTributariaRepositoryFake : IReglaTributariaRepository
{
    private readonly ReglaTributaria? _regla;

    public ReglaTributariaRepositoryFake(ReglaTributaria? regla)
    {
        _regla = regla;
    }

    public Task<ReglaTributaria?> ObtenerVigenteAsync(TipoImpuesto tipoImpuesto, DateOnly fecha)
        => Task.FromResult(_regla);

    public Task<IEnumerable<ReglaTributaria>> ObtenerTodasAsync(TipoImpuesto tipoImpuesto)
    {
        IEnumerable<ReglaTributaria> resultado = _regla is null
            ? Array.Empty<ReglaTributaria>()
            : new[] { _regla };

        return Task.FromResult(resultado);
    }
}
