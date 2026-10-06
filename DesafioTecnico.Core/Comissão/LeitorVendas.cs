using System.Text.Json;

namespace DesafioTecnico.Core.Comissão
{
    public static class LeitorVendas
    {
        private static readonly JsonSerializerOptions Opcoes = new(JsonSerializerDefaults.Web);

        internal sealed record ArquivoVendas(List<Venda>? Vendas);

        public static IReadOnlyList<Venda> Ler(string json)
        {
            var arquivo = JsonSerializer.Deserialize<ArquivoVendas>(json, Opcoes);

            return arquivo?.Vendas
                ?? throw new InvalidDataException("JSON inválido: propriedade 'vendas' não encontrada.");
        }
    }
}
