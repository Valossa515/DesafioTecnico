using System;
using System.Collections.Generic;
using System.Text;

namespace DesafioTecnico.Core.Comissão
{
    public record ComissaoVendedor(
        string Vendedor, 
        int QuantidadeVendas, 
        decimal TotalVendido, 
        decimal TotalComissao);
}