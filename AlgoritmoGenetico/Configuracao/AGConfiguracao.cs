using AlgoritmoGenetico.Individuo;
using AlgoritmoGenetico.Mutacao;
using AlgoritmoGenetico.Recombinacao;
using AlgoritmoGenetico.Selecao;

namespace AlgoritmoGenetico.Configuracao;

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

    /// <summary> Define se o processo de evolução é para minimizar ou maximizar a função de fitness. </summary>
    public AGProcessoDeEvolucao ProcessoDeEvolucao { get; init; } = AGProcessoDeEvolucao.MINIMIZACAO;

    /// <summary> Define a estratégia de mutação a aplicar aos indivíduos. </summary>
    public IMutacao<TCromossoma> ProcessoDeMutacao { get; init; } = new SemMutacao<TCromossoma>();

    /// <summary> Callback opcional executado imediatamente após a mutação de um cromossoma. </summary>
    public Action<TCromossoma, AGConfiguracao<TCromossoma>>? ProcessarMutacaoCallback { get; init; }

    /// <summary> Define a estratégia de crossover (recombinação) entre pais para gerar filhos. </summary>
    public IRecombinacao<TCromossoma> ProcessoDeRecombinacao { get; init; } = new SemRecombinacao<TCromossoma>();

    // --- Seleção da próxima geração ---
    /// <summary> Define a estratégia de sobrevivência (quem passa para a próxima geração). </summary>
    public ISelecao<TCromossoma> ProcessoDeSelecaoDaProximaGeracao { get; init; } = new Truncation<TCromossoma>();

    /// <summary> Probabilidade (0.0 a 1.0) de escolher indivíduos do pool de pais para a próxima geração. </summary>
    public float ProbabilidadeDeSelecionarDaGeracaoPais { get; init; } = 0.5f;

    /// <summary> Probabilidade (0.0 a 1.0) de escolher indivíduos do pool de filhos para a próxima geração. </summary>
    public float ProbabilidadeDeSelecionarDaGeracaoFilhos { get; init; } = 0.5f;

    // --- Monitorização ---
    /// <summary> Intervalo de tempo (em segundos) para invocar o callback de feedback. 0 desativa o feedback por tempo. </summary>
    public int DarFeedbackACadaSegundo { get; init; } = 0;
    /// <summary> Método invocado para monitorização da evolução em tempo real. </summary>    
    public Action<AG<TCromossoma>>? FeedbackCallback { get; init; }

}
