using DesafioTecnico.Core.Comissão;
using DesafioTecnico.Core.Estoque;
using System.Globalization;
using System.Text;
using System.Text.Json;
using DesafioTecnico.Core.Juros;

Console.OutputEncoding = Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("pt-BR");

ServicoEstoque? servicoEstoque = null;

while (true)
{
    Console.WriteLine();
    Console.WriteLine("===== Desafio Técnico =====");
    Console.WriteLine("1 - Calcular comissões");
    Console.WriteLine("2 - Movimentar estoque");
    Console.WriteLine("3 - Calcular juros por atraso");
    Console.WriteLine("0 - Sair");
    Console.Write("Opção: ");

    switch (Console.ReadLine()?.Trim())
    {
        case "1":
            ExecutarComissao();
            break;
        case "2":
            servicoEstoque ??= CarregarEstoque();
            if (servicoEstoque is not null)
                ExecutarEstoque(servicoEstoque);
            break;
        case "3":
            ExecutarJuros();
            break;
        case "0":
            return;
        default:
            Console.WriteLine("Opção inválida.");
            break;
    }
}

static void ExecutarComissao()
{
    try
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, "Dados", "vendas.json");
        var vendas = LeitorVendas.Ler(File.ReadAllText(caminho));
        var resultado = new CalculadoraComissao().CalcularPorVendedor(vendas);

        Console.WriteLine();
        Console.WriteLine($"{"Vendedor",-20}{"Vendas",8}{"Total vendido",18}{"Comissão",14}");
        Console.WriteLine(new string('-', 60));

        foreach (var c in resultado)
            Console.WriteLine($"{c.Vendedor,-20}{c.QuantidadeVendas,8}{c.TotalVendido,18:C}{c.TotalComissao,14:C}");
    }
    catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException or JsonException or InvalidDataException)
    {
        Console.WriteLine($"Erro ao processar vendas: {ex.Message}");
    }
}

static ServicoEstoque? CarregarEstoque()
{
    try
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, "Dados", "estoque.json");
        return new ServicoEstoque(LeitorEstoque.Ler(File.ReadAllText(caminho)));
    }
    catch (Exception ex) when (ex is FileNotFoundException or DirectoryNotFoundException or JsonException or InvalidDataException)
    {
        Console.WriteLine($"Erro ao carregar estoque: {ex.Message}");
        return null;
    }
}

static void ExecutarEstoque(ServicoEstoque servico)
{
    while (true)
    {
        Console.WriteLine();
        Console.WriteLine("----- Estoque -----");
        Console.WriteLine("1 - Listar produtos");
        Console.WriteLine("2 - Lançar movimentação");
        Console.WriteLine("3 - Histórico de movimentações");
        Console.WriteLine("0 - Voltar");
        Console.Write("Opção: ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1": ListarProdutos(servico); break;
            case "2": LancarMovimentacao(servico); break;
            case "3": ListarHistorico(servico); break;
            case "0": return;
            default: Console.WriteLine("Opção inválida."); break;
        }
    }
}

static void ListarProdutos(ServicoEstoque servico)
{
    Console.WriteLine();
    Console.WriteLine($"{"Código",-8}{"Produto",-30}{"Estoque",10}");
    Console.WriteLine(new string('-', 48));

    foreach (var p in servico.Produtos)
        Console.WriteLine($"{p.Codigo,-8}{p.Descricao,-30}{p.Quantidade,10}");
}

static void LancarMovimentacao(ServicoEstoque servico)
{
    var codigo = LerInteiro("Código do produto: ");
    var tipo = LerInteiro("Tipo (1 - Entrada, 2 - Saída): ") switch
    {
        1 => TipoMovimentacao.Entrada,
        2 => TipoMovimentacao.Saida,
        _ => (TipoMovimentacao?)null
    };

    if (tipo is null)
    {
        Console.WriteLine("Tipo inválido.");
        return;
    }

    var quantidade = LerInteiro("Quantidade: ");
    Console.Write("Descrição (ex.: Compra fornecedor, Venda balcão): ");
    var descricao = Console.ReadLine() ?? string.Empty;

    try
    {
        var resultado = servico.Movimentar(codigo, tipo.Value, quantidade, descricao);
        var mov = resultado.Movimentacao;

        Console.WriteLine();
        Console.WriteLine($"Movimentação #{mov.Id} registrada: {mov.Tipo} de {mov.Quantidade} un. ({mov.Descricao})");
        Console.WriteLine($"Estoque final do produto {mov.CodigoProduto}: {resultado.EstoqueFinal}");
    }
    catch (Exception ex) when (ex is ArgumentException or KeyNotFoundException or InvalidOperationException)
    {
        Console.WriteLine($"Movimentação não realizada: {ex.Message}");
    }
}

static void ListarHistorico(ServicoEstoque servico)
{
    if (servico.Historico.Count == 0)
    {
        Console.WriteLine("Nenhuma movimentação registrada.");
        return;
    }

    Console.WriteLine();
    foreach (var m in servico.Historico)
        Console.WriteLine($"#{m.Id} | {m.DataHora:dd/MM/yyyy HH:mm:ss} | Produto {m.CodigoProduto} | {m.Tipo} | {m.Quantidade} un. | {m.Descricao}");
}

static int LerInteiro(string mensagem)
{
    while (true)
    {
        Console.Write(mensagem);
        if (int.TryParse(Console.ReadLine(), out var valor))
            return valor;

        Console.WriteLine("Valor inválido. Digite um número inteiro.");
    }
}

static void ExecutarJuros()
{
    var valor = LerDecimal("Valor do título (ex.: 1500,00): ");
    var vencimento = LerData("Data de vencimento (dd/MM/aaaa): ");

    try
    {
        var r = new CalculadoraJuros().Calcular(valor, vencimento);

        Console.WriteLine();
        Console.WriteLine($"Valor original:   {r.ValorOriginal:C}");
        Console.WriteLine($"Vencimento:       {r.Vencimento:dd/MM/yyyy}");
        Console.WriteLine($"Data do cálculo:  {r.DataCalculo:dd/MM/yyyy}");
        Console.WriteLine($"Dias em atraso:   {r.DiasAtraso}");
        Console.WriteLine($"Juros (2,5%/dia): {r.Juros:C}");
        Console.WriteLine($"Valor atualizado: {r.ValorAtualizado:C}");
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"Cálculo não realizado: {ex.Message}");
    }
}

static decimal LerDecimal(string mensagem)
{
    while (true)
    {
        Console.Write(mensagem);
        if (decimal.TryParse(Console.ReadLine(), NumberStyles.Number, CultureInfo.CurrentCulture, out var valor))
            return valor;

        Console.WriteLine("Valor inválido. Use o formato 1500,00.");
    }
}

static DateOnly LerData(string mensagem)
{
    while (true)
    {
        Console.Write(mensagem);
        if (DateOnly.TryParseExact(Console.ReadLine()?.Trim(), "dd/MM/yyyy", CultureInfo.CurrentCulture, DateTimeStyles.None, out var data))
            return data;

        Console.WriteLine("Data inválida. Use o formato dd/MM/aaaa.");
    }
}