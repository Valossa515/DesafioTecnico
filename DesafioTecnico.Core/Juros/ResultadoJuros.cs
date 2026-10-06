namespace DesafioTecnico.Core.Juros
{
    public record ResultadoJuros(
         decimal ValorOriginal,
         DateOnly Vencimento,
         DateOnly DataCalculo,
         int DiasAtraso,
         decimal Juros,
         decimal ValorAtualizado);
}