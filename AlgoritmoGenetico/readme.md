# 🧬 AG Engine: Motor de Algoritmo Genético Multi-Propósito

Um motor (Framework) de **Algoritmo Genético (AG)** genérico e desenvolvido em .NET. Este projeto fornece a infraestrutura necessária para resolver problemas de otimização complexos, abstraindo a lógica evolutiva da implementação específica do domínio.

---

## 🏗️ Filosofia e Arquitetura

O motor foi desenhado como uma **biblioteca de infraestrutura**. Ele não contém lógica de problema; em vez disso, define contratos e fluxos de trabalho que permitem ao utilizador focar-se exclusivamente na modelagem do problema.

### Abstração de Domínio

A separação de responsabilidades é garantida através de três pilares:

1. **Domínio do Problema**: Definido pelas implementações de `ICromossoma<IGene>` e `IGene`.
2. **Estratégias de Evolução**: Implementações trocáveis de `ISelecao` e `IRecombinacao`.
3. **Orquestrador (`AG<T>`)**: Gere o ciclo de vida, paralelismo e métricas de performance.

---

## 🛠️ Capacidades Técnicas

### 1. Motor de Evolução Adaptativo

- **Mutação por Feedback**: O motor monitoriza o estado da população. Se detetar estagnação (falta de evolução global), aumenta dinamicamente a taxa de mutação para forçar a exploração de novas áreas do espaço de soluções.
- **Mecânica de Sobrevivência Híbrida**: Implementa um fluxo de substituição onde pais e filhos competem diretamente, garantindo que apenas os indivíduos mais aptos de ambos os grupos transitam para a geração seguinte.

### 2. Otimização e Paralelismo

- **Avaliação Multi-Core**: Utiliza `Parallel.For` para o cálculo de fitness, otimizando o tempo de processamento em problemas onde a função de avaliação é computacionalmente pesada.

---

## 🧩 Estrutura de Integração

Para utilizar este motor num projeto, o fluxo de implementação exige a definição dos componentes de domínio e a personalização dos operadores que dependem da estrutura genética:

1. **Definição Genética (Domínio)**:
    - Implementar `IGene` para representar a unidade básica da solução (ex: uma aula).
    - Implementar `ICromossoma<IGene>` para definir a lógica de avaliação (**Fitness**) e os métodos de fabricação (`CriarVazio`, `CriarAleatorio`).

2. **Escolha e Implementação do Processo de Mutação**:
    - **Flexibilidade**: O utilizador pode escolher entre as estratégias de mutação já incluídas ou criar processos inteiramente novos.
    - **Implementação Obrigatória**: Independentemente da estratégia escolhida, é necessário implementar a lógica de mutação específica. Como a mutação altera diretamente o estado dos genes, ela depende do conhecimento das regras de problema (ex: garantir que um novo valor sorteado para o gene é válido e não viola as restrições do problema).

3. **Configuração e Orquestração**:
    - Estender a classe abstrata `AG<TCromossoma>` para ligar o motor ao domínio específico.
    - Configurar o objeto `AGConfiguracao` com as taxas de mutação, dimensões da população e os delegados de feedback.

---

## 📊 Operadores Disponíveis Out-of-the-box

O motor inclui um conjunto de estratégias predefinidas que podem ser utilizadas imediatamente ou servir de base para novas extensões.

| Categoria        | Operadores Incluídos                                                           | Descrição                                                                                   |
| :--------------- | :----------------------------------------------------------------------------- | :------------------------------------------------------------------------------------------ |
| **Seleção**      | `Truncation`, `Tournament`, `Roulette Wheel`, `Crowding`,`Todos`               | Filtros para definir quem sobrevive e quem procria (focados em performance ou diversidade). |
| **Recombinação** | `Single Point`, `Two Points`, `Uniforme`, `CycleCrossOver`, `Sem recombinação` | Diferentes métodos para misturar o material genético dos progenitores.                      |
| **Mutação**      | `Unica`, `Multipla`, `Sem mutação`                                             | Estratégias que definem a abrangência da alteração genética por indivíduo.                  |

---

---

## ⚙️ Exemplo de Configuração

Exemplo de como configurar o motor para uma implementação concreta:

```csharp
var configuracao = new AGConfiguracao<MeuCromossoma>
{
    // Dimensões e Limites
    DimensaoDaPopulacao = 100,
    LimiteMáximoDeGeracoesPermitidas = 5000,
    FitnessPretendido = 0,

    // Direção da Evolução
    ProcessoDeEvolucao = AGProcessoDeEvolucao.MINIMIZACAO,

    // Injeção de Estratégias
    ProcessoDeSelecaoDaProximaGeracao = new Tournament<MeuCromossoma>(),
    ProbabilidadeDeSelecionarDaGeracaoPais = 0.10f,     // 10% da geração angterior (pais)
    ProbabilidadeDeSelecionarDaGeracaoFilhos = .90f,    // 90% dos filhos
    ProcessoDeRecombinacao = new TwoPoints<MeuCromossoma>(),

    // Implementação de Mutação para o Domínio
    ProcessoDeMutacao = new Unica<MeuCromossoma>
    {
        FatorMutacaoNormal = 0.02d,    // 2% de probabilidade base
        FatorMutacaoColisao = 0.12d,   // 12% se detetar conflitos no gene
        AjustarMutacaoACadaGeracao = 50
    },

    // Feedback e Observabilidade
    DarFeedbackACadaSegundo = 1,
    FeedbackCallback = (motor) => {
        // Integração com a UI (ex: Spectre.Console)
        Console.WriteLine(motor.PerformanceInfo());
    }
};
```
