# Desafio Técnico

Solução em C# para três desafios: cálculo de comissão de vendedores, movimentação de estoque e cálculo de juros por atraso.

## Como rodar

**Pré-requisito:** SDK do .NET 8 ou superior.

```bash
# Executar a aplicação (menu interativo no console)
dotnet run --project src/DesafioTecnico.App

# Executar os testes
dotnet test
```

No Visual Studio, defina `DesafioTecnico.App` como projeto de inicialização e pressione F5. Os testes ficam em **Teste → Gerenciador de Testes**.

Ao iniciar, o programa exibe o menu:

```
===== Desafio Técnico =====
1 - Calcular comissões
2 - Movimentar estoque
3 - Calcular juros por atraso
0 - Sair
```

## Estrutura

```
DesafioTecnico.sln
├── src/
│   ├── DesafioTecnico.Core/       Regras de negócio (sem dependência de console ou arquivo)
│   │   ├── Comissao/
│   │   ├── Estoque/
│   │   └── Juros/
│   └── DesafioTecnico.App/        Aplicação de console: menu, entrada de dados e exibição
│       └── Dados/                 vendas.json e estoque.json do enunciado
└── tests/
    └── DesafioTecnico.Tests/      Testes unitários (xUnit) das regras do Core
```

As regras ficam no `Core` e não sabem de onde vêm os dados nem como são exibidos. Os leitores de JSON recebem o conteúdo como texto, não o caminho do arquivo, para serem testáveis sem acesso a disco. Com essa separação, a mesma lógica pode ser exposta por uma API, por exemplo, apenas adicionando um novo projeto de apresentação.

Valores monetários usam `decimal` para evitar erros de arredondamento de ponto flutuante.

## Desafio 1: Comissão

Lê `Dados/vendas.json` e calcula a comissão de cada vendedor, aplicando a regra a cada venda:

| Valor da venda | Comissão |
|---|---|
| Abaixo de R$ 100,00 | 0% |
| De R$ 100,00 até R$ 499,99 | 1% |
| A partir de R$ 500,00 | 5% |

**Premissas**
- "A partir de R$ 500,00" inclui o próprio valor: uma venda de exatamente R$ 500,00 rende 5%.
- A comissão de cada venda é calculada com precisão total e só o total por vendedor é arredondado para 2 casas (`MidpointRounding.AwayFromZero`), evitando acúmulo de erro de arredondamento.
- Valores de venda negativos são rejeitados.

**Resultado com os dados do enunciado**

| Vendedor | Vendas | Total vendido | Comissão |
|---|---:|---:|---:|
| João Silva | 10 | R$ 10.754,70 | R$ 495,68 |
| Maria Souza | 9 | R$ 9.874,30 | R$ 465,95 |
| Ana Lima | 9 | R$ 8.763,95 | R$ 404,98 |
| Carlos Oliveira | 8 | R$ 7.928,35 | R$ 379,37 |

## Desafio 2: Movimentação de estoque

Carrega `Dados/estoque.json` e permite listar produtos, lançar entradas e saídas e consultar o histórico. Cada movimentação registra um ID único, o tipo, a quantidade, uma descrição e a data/hora. Após cada lançamento, o programa exibe o estoque final do produto movimentado.

**Premissas**
- A movimentação tem um **tipo** fixo (`Entrada` ou `Saida`), que define se soma ou subtrai do estoque, e uma **descrição** livre que identifica a operação ("Compra fornecedor", "Venda balcão", "Ajuste de inventário"). Assim a regra não depende de interpretar texto digitado.
- O ID é sequencial e único durante a execução, gerado apenas quando a movimentação é efetivada. Movimentações recusadas não consomem ID.
- Os dados ficam em memória: o estoque é carregado do JSON na primeira vez que a opção é acessada, e as movimentações permanecem enquanto o programa estiver aberto.

**Validações**
- Produto inexistente.
- Quantidade igual ou menor que zero.
- Descrição vazia.
- Saída maior que o estoque disponível. O estoque nunca fica negativo, e uma movimentação recusada não altera nada.

## Desafio 3: Juros por atraso

Recebe um valor e uma data de vencimento e calcula os juros na data de hoje.

**Fórmula:** `juros = valor × 2,5% × dias de atraso`

**Premissas**
- O enunciado usa "juros" e "multa de 2,5% ao dia" para a mesma cobrança. Foi adotado **juros simples de 2,5% por dia de atraso**, sobre o valor original.
- Títulos com vencimento hoje ou em data futura não têm juros.
- Os juros são arredondados para 2 casas, e o programa exibe também o valor atualizado (valor + juros).
- O valor deve ser maior que zero. A entrada aceita o formato brasileiro (`1.500,00`) e a data deve estar no formato `dd/MM/aaaa`; datas inexistentes, como 31/02, são recusadas.

**Exemplo:** R$ 1.000,00 com 10 dias de atraso gera R$ 250,00 de juros, totalizando R$ 1.250,00.

A data de hoje é obtida por um `TimeProvider` injetado na calculadora. A aplicação usa o relógio do sistema, e os testes usam uma data fixa, para que o resultado não dependa do dia em que são executados.

## Testes

27 testes unitários cobrindo:
- **Comissão:** os limites de cada faixa (99,99 / 100,00 / 499,99 / 500,00), o agrupamento por vendedor e a leitura do JSON.
- **Estoque:** entrada, saída, saída que zera o estoque, saída maior que o disponível, quantidade inválida, descrição vazia, produto inexistente, geração de IDs sequenciais e a leitura do JSON.
- **Juros:** título antes do vencimento, vencendo hoje, com atraso, arredondamento e valor inválido.