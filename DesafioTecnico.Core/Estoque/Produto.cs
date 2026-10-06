namespace DesafioTecnico.Core.Estoque
{
    public class Produto
    {
        public int Codigo { get; }
        public string Descricao { get; }
        public int Quantidade { get; private set; }

        public Produto(int codigo, string descricao, int quantidade)
        {
            if (quantidade < 0)
                throw new ArgumentOutOfRangeException(nameof(quantidade),
                    "O estoque inicial não pode ser negativo.");

            Codigo = codigo;
            Descricao = descricao;
            Quantidade = quantidade;
        }

        internal void Adicionar(int quantidade) => Quantidade += quantidade;

        internal void Remover(int quantidade)
        {
            if (quantidade > Quantidade)
                throw new InvalidOperationException(
                    $"Estoque insuficiente para '{Descricao}'. Disponível: {Quantidade}, solicitado: {quantidade}.");

            Quantidade -= quantidade;
        }
    }
}
