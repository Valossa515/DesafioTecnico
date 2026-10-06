namespace DesafioTecnico.Core.Juros
{
    public class CalculadoraJuros
    {
        public const decimal TaxaDiaria = 0.025m;
        private readonly TimeProvider _timeProvider;
        public CalculadoraJuros() : this(TimeProvider.System) { }

        public CalculadoraJuros(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public ResultadoJuros Calcular(decimal valor, DateOnly vencimento)
        {
            if (valor <= 0)
                throw new ArgumentOutOfRangeException(nameof(valor), "O valor deve ser maior que zero.");

            var hoje = DateOnly.FromDateTime(_timeProvider.GetLocalNow().DateTime);
            var diasAtraso = Math.Max(0, hoje.DayNumber - vencimento.DayNumber);
            var juros = Math.Round(valor * TaxaDiaria * diasAtraso, 2, MidpointRounding.AwayFromZero);

            return new ResultadoJuros(valor, vencimento, hoje, diasAtraso, juros, valor + juros);
        }
    }
}