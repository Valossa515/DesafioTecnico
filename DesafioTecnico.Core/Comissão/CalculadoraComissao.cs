using System;
using System.Collections.Generic;
using System.Text;

namespace DesafioTecnico.Core.Comissão
{
    public class CalculadoraComissao
    {
        private const decimal ValorMinimoComissao = 100m;
        private const decimal ValorFaixaSuperior = 500m;
        private const decimal PercentualFaixaInferior = 0.01m;
        private const decimal PercentualFaixaSuperior = 0.05m;


        public decimal CalcularComissao(decimal valorVenda)
        {
            if(valorVenda < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(valorVenda),
                    "O valor da venda não pode ser negativo.");

            return valorVenda switch
            {
                < ValorMinimoComissao => 0m,
                < ValorFaixaSuperior => valorVenda * PercentualFaixaInferior,
                _ => valorVenda * PercentualFaixaSuperior
            };
        }

        public IReadOnlyList<ComissaoVendedor> CalcularPorVendedor(
            IEnumerable<Venda> vendas) => vendas
                .GroupBy(v => v.Vendedor)
                .Select(g => new ComissaoVendedor(
                    Vendedor: g.Key,
                    QuantidadeVendas: g.Count(),
                    TotalVendido: g.Sum(v => v.Valor),
                    TotalComissao: Math.Round(
                        g.Sum(v => CalcularComissao(v.Valor)), 2, MidpointRounding.AwayFromZero)))
                .OrderByDescending(c => c.TotalComissao)
                .ToList();
    }

}
