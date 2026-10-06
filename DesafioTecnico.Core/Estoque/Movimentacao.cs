namespace DesafioTecnico.Core.Estoque
{
    public record Movimentacao(
        long Id,
        int CodigoProduto,
        TipoMovimentacao Tipo,
        int Quantidade,
        string Descricao,
        DateTime DataHora);

    public record ResultadoMovimentacao(
        Movimentacao Movimentacao,
        int EstoqueFinal);
}
