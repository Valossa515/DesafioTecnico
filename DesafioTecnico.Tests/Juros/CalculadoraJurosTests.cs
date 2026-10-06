using DesafioTecnico.Core.Juros;

namespace DesafioTecnico.Tests.Juros
{
    public class CalculadoraJurosTests
    {
        private static readonly DateOnly Hoje = new(2026, 10, 6);

        private sealed class RelogioFixo(DateOnly data) : TimeProvider
        {
            public override DateTimeOffset GetUtcNow() =>
                new(data.ToDateTime(new TimeOnly(12, 0)), TimeSpan.Zero);

            public override TimeZoneInfo LocalTimeZone => TimeZoneInfo.Utc;
        }

        private readonly CalculadoraJuros _calculadora = new(new RelogioFixo(Hoje));

        [Fact]
        public void Calcular_AntesDoVencimento_NaoDeveCobrarJuros()
        {
            var resultado = _calculadora.Calcular(1000m, Hoje.AddDays(5));

            Assert.Equal(0, resultado.DiasAtraso);
            Assert.Equal(0m, resultado.Juros);
            Assert.Equal(1000m, resultado.ValorAtualizado);
        }

        [Fact]
        public void Calcular_VencendoHoje_NaoDeveCobrarJuros()
        {
            var resultado = _calculadora.Calcular(1000m, Hoje);

            Assert.Equal(0, resultado.DiasAtraso);
            Assert.Equal(0m, resultado.Juros);
        }

        [Theory]
        [InlineData(1, 25)]
        [InlineData(10, 250)]
        [InlineData(40, 1000)]
        public void Calcular_ComAtraso_DeveCobrar2Virgula5PorCentoAoDia(int diasAtraso, double jurosEsperados)
        {
            var resultado = _calculadora.Calcular(1000m, Hoje.AddDays(-diasAtraso));

            Assert.Equal(diasAtraso, resultado.DiasAtraso);
            Assert.Equal((decimal)jurosEsperados, resultado.Juros);
            Assert.Equal(1000m + (decimal)jurosEsperados, resultado.ValorAtualizado);
        }

        [Fact]
        public void Calcular_DeveArredondarJurosParaDuasCasas()
        {
            var resultado = _calculadora.Calcular(333.33m, Hoje.AddDays(-1));

            Assert.Equal(8.33m, resultado.Juros);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void Calcular_ValorInvalido_DeveLancarExcecao(double valor)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _calculadora.Calcular((decimal)valor, Hoje));
        }
    }
}
