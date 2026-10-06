using System.Text.Json;

namespace DesafioTecnico.Core.Estoque
{
    public static class LeitorEstoque
    {
        private static readonly JsonSerializerOptions Opcoes = new(JsonSerializerDefaults.Web);

        internal sealed record ItemEstoqueJson(int CodigoProduto, string DescricaoProduto, int Estoque);
        internal sealed record ArquivoEstoque(List<ItemEstoqueJson>? Estoque);

        public static IReadOnlyList<Produto> Ler(string json)
        {
            var arquivo = JsonSerializer.Deserialize<ArquivoEstoque>(json, Opcoes);

            if (arquivo?.Estoque is null)
                throw new InvalidDataException("JSON inválido: propriedade 'estoque' não encontrada.");

            return arquivo.Estoque
                .Select(i => new Produto(i.CodigoProduto, i.DescricaoProduto, i.Estoque))
                .ToList();
        }
    }
}