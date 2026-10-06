using DesafioTecnico.Core.Estoque;

namespace DesafioTecnico.Tests.Estoque
{
    public class ServicoEstoqueTests
    {
        private static ServicoEstoque CriarServico() => new(
        [
            new Produto(101, "Caneta Azul", 150),
        new Produto(102, "Caderno Universitário", 75),
    ]);

        [Fact]
        public void Movimentar_Entrada_DeveSomarAoEstoque()
        {
            var servico = CriarServico();

            var resultado = servico.Movimentar(101, TipoMovimentacao.Entrada, 50, "Compra fornecedor");

            Assert.Equal(200, resultado.EstoqueFinal);
        }

        [Fact]
        public void Movimentar_Saida_DeveSubtrairDoEstoque()
        {
            var servico = CriarServico();

            var resultado = servico.Movimentar(102, TipoMovimentacao.Saida, 25, "Venda balcão");

            Assert.Equal(50, resultado.EstoqueFinal);
        }

        [Fact]
        public void Movimentar_SaidaDeTodoOEstoque_DeveZerar()
        {
            var servico = CriarServico();

            var resultado = servico.Movimentar(102, TipoMovimentacao.Saida, 75, "Venda total");

            Assert.Equal(0, resultado.EstoqueFinal);
        }

        [Fact]
        public void Movimentar_SaidaMaiorQueEstoque_DeveLancarExcecaoESemAlterarEstoque()
        {
            var servico = CriarServico();

            Assert.Throws<InvalidOperationException>(
                () => servico.Movimentar(102, TipoMovimentacao.Saida, 76, "Venda"));

            Assert.Equal(75, servico.Produtos.Single(p => p.Codigo == 102).Quantidade);
            Assert.Empty(servico.Historico);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public void Movimentar_QuantidadeInvalida_DeveLancarExcecao(int quantidade)
        {
            var servico = CriarServico();

            Assert.Throws<ArgumentOutOfRangeException>(
                () => servico.Movimentar(101, TipoMovimentacao.Entrada, quantidade, "Ajuste"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Movimentar_SemDescricao_DeveLancarExcecao(string descricao)
        {
            var servico = CriarServico();

            Assert.Throws<ArgumentException>(
                () => servico.Movimentar(101, TipoMovimentacao.Entrada, 10, descricao));
        }

        [Fact]
        public void Movimentar_ProdutoInexistente_DeveLancarExcecao()
        {
            var servico = CriarServico();

            Assert.Throws<KeyNotFoundException>(
                () => servico.Movimentar(999, TipoMovimentacao.Entrada, 10, "Compra"));
        }

        [Fact]
        public void Movimentar_DeveGerarIdsUnicosESequenciais()
        {
            var servico = CriarServico();

            var primeira = servico.Movimentar(101, TipoMovimentacao.Entrada, 10, "Compra");
            var segunda = servico.Movimentar(101, TipoMovimentacao.Saida, 5, "Venda");

            Assert.Equal(1, primeira.Movimentacao.Id);
            Assert.Equal(2, segunda.Movimentacao.Id);
            Assert.Equal(2, servico.Historico.Count);
        }
    }
}