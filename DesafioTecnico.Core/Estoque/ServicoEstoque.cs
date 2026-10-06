namespace DesafioTecnico.Core.Estoque
{
    public class ServicoEstoque
    {
        private readonly Dictionary<int, Produto> _produtos;
        private readonly List<Movimentacao> _historico = [];
        private long _ultimoId;

        public ServicoEstoque(IEnumerable<Produto> produtos)
        {
            _produtos = produtos.ToDictionary(p => p.Codigo);
        }

        public IReadOnlyCollection<Produto> Produtos => _produtos.Values;
        public IReadOnlyList<Movimentacao> Historico => _historico;

        public ResultadoMovimentacao Movimentar(int codigoProduto, TipoMovimentacao tipo, int quantidade, string descricao)
        {
            if (quantidade <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantidade), "A quantidade deve ser maior que zero.");

            if (string.IsNullOrWhiteSpace(descricao))
                throw new ArgumentException("A descrição da movimentação é obrigatória.", nameof(descricao));

            if (!_produtos.TryGetValue(codigoProduto, out var produto))
                throw new KeyNotFoundException($"Produto {codigoProduto} não encontrado.");

            if (tipo == TipoMovimentacao.Entrada)
                produto.Adicionar(quantidade);
            else
                produto.Remover(quantidade);

            var movimentacao = new Movimentacao(
                Id: ++_ultimoId,
                CodigoProduto: codigoProduto,
                Tipo: tipo,
                Quantidade: quantidade,
                Descricao: descricao.Trim(),
                DataHora: DateTime.Now);

            _historico.Add(movimentacao);

            return new ResultadoMovimentacao(movimentacao, produto.Quantidade);
        }
    }
}
