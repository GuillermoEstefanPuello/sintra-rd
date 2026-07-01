// DEVGEP+ | Guillermo Estefan Puello
// Pruebas unitarias del Motor de Liquidacion (M3).
// Cada test controla el comportamiento del repositorio via ReglaTributariaRepositoryFake,
// sin acceso al sistema de archivos ni dependencias externas.

using SintraRd.Api.Models.Dtos;
using SintraRd.Api.Models.Entidades;
using SintraRd.Api.Services;
using SintraRd.Tests.Fakes;

namespace SintraRd.Tests.MotorLiquidacion;

public class MotorLiquidacionTests
{
    // Construye una ReglaTributaria con las 4 tasas del ITBIS para usar en los tests
    private static ReglaTributaria CrearReglaItbis() => new()
    {
        Id = "test-itbis",
        TipoImpuesto = TipoImpuesto.ITBIS,
        FechaVigenciaDesde = new DateOnly(2016, 1, 1),
        Version = "ITBIS-2016-v1",
        BaseLegal = "Art. 23 Ley 253-12",
        Tasas = new()
        {
            new() { Tipo = TipoTasa.General,  Valor = 0.18m },
            new() { Tipo = TipoTasa.Reducida, Valor = 0.16m },
            new() { Tipo = TipoTasa.TasaCero, Valor = 0.00m },
            new() { Tipo = TipoTasa.Exento,   Valor = 0.00m }
        }
    };

    // Construye una solicitud base reutilizable; solo varian los campos relevantes por test
    private static SolicitudLiquidacionDto CrearSolicitud(
        TipoTasa tipoTasa,
        decimal montoBase,
        decimal creditoFiscal = 0m) => new()
    {
        TipoImpuesto   = TipoImpuesto.ITBIS,
        TipoOperacion  = TipoOperacion.TransferenciaBienes,
        TipoTasa       = tipoTasa,
        MontoBase      = montoBase,
        CreditoFiscal  = creditoFiscal,
        Periodo        = "2026-07",
        FechaOperacion = new DateOnly(2026, 7, 1)
    };

    [Fact]
    public async Task General_SinCredito_CalculaImpuestoYTotalCorrectamente()
    {
        var motor = new Api.Services.MotorLiquidacion(
            new ReglaTributariaRepositoryFake(CrearReglaItbis()));

        var resultado = await motor.LiquidarAsync(
            CrearSolicitud(TipoTasa.General, montoBase: 1000m));

        Assert.Equal(180m, resultado.MontoImpuesto);
        Assert.Equal(180m, resultado.TotalAPagar);
    }

    [Fact]
    public async Task General_ConCredito50_DescontaCreditoDelTotal()
    {
        var motor = new Api.Services.MotorLiquidacion(
            new ReglaTributariaRepositoryFake(CrearReglaItbis()));

        var resultado = await motor.LiquidarAsync(
            CrearSolicitud(TipoTasa.General, montoBase: 1000m, creditoFiscal: 50m));

        Assert.Equal(180m, resultado.MontoImpuesto);
        Assert.Equal(130m, resultado.TotalAPagar);
    }

    [Fact]
    public async Task Reducida_CalculaImpuestoConTasa16()
    {
        var motor = new Api.Services.MotorLiquidacion(
            new ReglaTributariaRepositoryFake(CrearReglaItbis()));

        var resultado = await motor.LiquidarAsync(
            CrearSolicitud(TipoTasa.Reducida, montoBase: 500m));

        Assert.Equal(80m, resultado.MontoImpuesto);
    }

    [Fact]
    public async Task Exento_NoGeneraImpuestoNiTotal()
    {
        var motor = new Api.Services.MotorLiquidacion(
            new ReglaTributariaRepositoryFake(CrearReglaItbis()));

        var resultado = await motor.LiquidarAsync(
            CrearSolicitud(TipoTasa.Exento, montoBase: 1000m));

        Assert.Equal(0m, resultado.MontoImpuesto);
        Assert.Equal(0m, resultado.TotalAPagar);
        Assert.Equal(TipoTasa.Exento, resultado.TipoTasaAplicada);
    }

    [Fact]
    public async Task TasaCero_NoGeneraImpuestoPeroCategoriaDistintaDeExento()
    {
        var motor = new Api.Services.MotorLiquidacion(
            new ReglaTributariaRepositoryFake(CrearReglaItbis()));

        var resultado = await motor.LiquidarAsync(
            CrearSolicitud(TipoTasa.TasaCero, montoBase: 1000m));

        Assert.Equal(0m, resultado.MontoImpuesto);
        Assert.Equal(TipoTasa.TasaCero, resultado.TipoTasaAplicada);
    }

    [Fact]
    public async Task SinReglaVigente_LanzaInvalidOperationConMensajeVigente()
    {
        var motor = new Api.Services.MotorLiquidacion(
            new ReglaTributariaRepositoryFake(null));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => motor.LiquidarAsync(CrearSolicitud(TipoTasa.General, montoBase: 1000m)));

        Assert.Contains("vigente", ex.Message);
    }

    [Fact]
    public async Task Credito_MayorQueImpuesto_TotalAPagarEsNegativo()
    {
        var motor = new Api.Services.MotorLiquidacion(
            new ReglaTributariaRepositoryFake(CrearReglaItbis()));

        // 1000 * 0.18 = 180; credito 200 > 180 => saldo a favor de -20
        var resultado = await motor.LiquidarAsync(
            CrearSolicitud(TipoTasa.General, montoBase: 1000m, creditoFiscal: 200m));

        Assert.Equal(-20m, resultado.TotalAPagar);
    }
}
