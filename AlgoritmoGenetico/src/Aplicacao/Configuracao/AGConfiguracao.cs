
using AlgoritmoGenetico.Aplicacao.Motor;
using AlgoritmoGenetico.Core.Abstracoes;
using AlgoritmoGenetico.Core.Operadores.Selecao;

namespace AlgoritmoGenetico.Aplicacao.Configuracao;

/// <summary>
/// Define os parâmetros de configuração e as estratégias de execução do Algoritmo Genético.
/// </summary>
/// <typeparam name="TCromossoma">O tipo de cromossoma utilizado, que deve implementar ICromossoma.</typeparam>
public record AGConfiguracao<TCromossoma> where TCromossoma : ICromossoma<IGene>
{

    // --- Estrutura do Algoritmo Genético ---
    /// <summary> Definição do tamanho máximo da população em cada geração. </summary>
    public required int DimensaoDaPopulacao { get; init; }

    /// <summary> Limite máximo de iterações do algoritmo. </summary>
    public required int LimiteMáximoDeGeracoesPermitidas { get; init; }

    /// <summary> Valor de fitness que, se alcançado, interrompe a execução com sucesso. </summary>
    public float FitnessPretendido { get; init; } = 0;

    /// <summary> 
    /// Frequência (em gerações) com que a melhor solução atual é reinserida na população 
    /// para evitar a perda de material genético de elite. 
    /// </summary>
    public int ReporSolucaoCandidataNaPopulacaoACadaGeracao { get; init; } = 0;


    // --- Estratégias do Algoritmo Genético ---

    /// <summary>
    /// Define a função de fitness a ser utilizada para avaliar os cromossomas. O AG irá minimizar ou maximizar este valor conforme o ProcessoDeEvolucao.
    /// </summary>
    public required ICromossomaFitnessService<TCromossoma> ProcessoCalculoFitness { get; init; }
    /// <summary> Define se o processo de evolução é para minimizar ou maximizar a função de fitness. </summary>
    public required AGProcessoDeEvolucao ProcessoDeEvolucao { get; init; } = AGProcessoDeEvolucao.MINIMIZACAO;

    /// <summary> Define a estratégia de mutação a aplicar aos indivíduos. </summary>
    public required IMutacaoService<TCromossoma> ProcessoDeMutacao { get; init; }

    /// <summary> Define a estratégia de crossover (recombinação) entre pais para gerar filhos. </summary>
    public required IRecombinacao<TCromossoma> ProcessoDeRecombinacao { get; init; }

    // --- Seleção da próxima geração ---
    /// <summary> Define a estratégia de sobrevivência (quem passa para a próxima geração). </summary>
    public required ISelecao<TCromossoma> ProcessoDeSelecaoDaProximaGeracao { get; init; }

    /// <summary> Probabilidade (0.0 a 1.0) de escolher indivíduos do pool de pais para a próxima geração. </summary>
    public float ProbabilidadeDeSelecionarDaGeracaoPais { get; init; } = 0.5f;

    /// <summary> Probabilidade (0.0 a 1.0) de escolher indivíduos do pool de filhos para a próxima geração. </summary>
    public float ProbabilidadeDeSelecionarDaGeracaoFilhos { get; init; } = 0.5f;

    // --- Monitorização ---
    /// <summary> Intervalo de tempo (em segundos) para invocar o callback de feedback. 0 desativa o feedback por tempo. </summary>
    public int DarFeedbackACadaSegundo { get; init; } = 0;

    /// <summary>
    /// Fábrica para criação de cromossomas, utilizada para gerar a população inicial e para criar novos indivíduos durante a evolução. Deve ser compatível com o tipo TCromossoma.
    /// </summary>
    public required ICromossomaFactory<TCromossoma> CromossomaFactory { get; init; }
    /// <summary>
    /// Serviço de output para receber feedbacks e atualizações do AG, como a melhor solução encontrada. Pode ser utilizado para monitorização em tempo real ou logging.
    /// </summary>
    public required IAGOutputService<TCromossoma> OutputService { get; init; }

}
