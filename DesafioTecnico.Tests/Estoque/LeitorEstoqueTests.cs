using DesafioTecnico.Core.Estoque;

namespace DesafioTecnico.Tests.Estoque
{
    public class LeitorEstoqueTests
    {
        [Fact]
        public void Ler_JsonValido_DeveRetornarProdutos()
        {
            const string json = """
            { "estoque": [ { "codigoProduto": 101, "descricaoProduto": "Caneta Azul", "estoque": 150 } ] }
            """;

            var produto = Assert.Single(LeitorEstoque.Ler(json));

            Assert.Equal(101, produto.Codigo);
            Assert.Equal("Caneta Azul", produto.Descricao);
            Assert.Equal(150, produto.Quantidade);
        }

        [Fact]
        public void Ler_SemPropriedadeEstoque_DeveLancarExcecao()
        {
            Assert.Throws<InvalidDataException>(() => LeitorEstoque.Ler("{}"));
        }
    }
}