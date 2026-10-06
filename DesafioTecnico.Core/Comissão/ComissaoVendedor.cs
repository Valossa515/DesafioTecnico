namespace DesafioTecnico.Core.Comissão
{
    public record ComissaoVendedor(
        string Vendedor, 
        int QuantidadeVendas, 
        decimal TotalVendido, 
        decimal TotalComissao);
}