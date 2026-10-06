using DesafioTecnico.Core.Comissão;

namespace DesafioTecnico.Tests.Comissao
{
    public class CalculadoraComissaoTests
    {
        private readonly CalculadoraComissao _calculadora = new();

        [Theory]
        [InlineData(0, 0)]
        [InlineData(99.99, 0)]
        [InlineData(100, 1)]
        [InlineData(499.99, 4.9999)]
        [InlineData(500, 25)]
        [InlineData(1200.50, 60.025)]
        public void CalcularComissao_DeveAplicarFaixaCorreta(double valor, double esperado)
        {
            var comissao = _calculadora.CalcularComissao((decimal)valor);

            Assert.Equal((decimal)esperado, comissao);
        }

        [Fact]
        public void CalcularComissao_ValorNegativo_DeveLancarExcecao()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _calculadora.CalcularComissao(-1m));
        }

        [Fact]
        public void CalcularPorVendedor_DeveAgruparESomarComissoes()
        {
            var vendas = new List<Venda>
        {
            new("Ana", 1000m),
            new("Ana", 200m),
            new("Ana", 50m),
            new("Bruno", 500m),
        };

            var resultado = _calculadora.CalcularPorVendedor(vendas);

            Assert.Equal(2, resultado.Count);
            Assert.Equal(new ComissaoVendedor("Ana", 3, 1250m, 52m), resultado[0]);
            Assert.Equal(new ComissaoVendedor("Bruno", 1, 500m, 25m), resultado[1]);
        }
    }
}
