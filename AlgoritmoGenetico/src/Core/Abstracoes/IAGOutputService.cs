
using AlgoritmoGenetico.Aplicacao.Motor;

namespace AlgoritmoGenetico.Core.Abstracoes
{
    /// <summary>
    /// Interface para serviços de output do Algoritmo Genético, permitindo a implementação de diferentes formas de exibir o progresso e os resultados do algoritmo, como em console, UI gráfica ou logs.
    /// </summary>
    public interface IAGOutputService<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        // --- Início ---
        /// <summary>Chamado antes de iniciar a primeira geração.</summary>
        void OnEvolucaoIniciada();

        // --- Evolução (Feedback a cada geração ou X gerações) ---
        /// <summary>Chamado após cada geração ser processada.</summary>
        void OnGeracaoProcessada(AG<TCromossoma> algoritmo);

        /// <summary>Chamado quando o AG encontra uma nova melhor solução.</summary>
        void OnMelhorSolucaoEncontrada(TCromossoma melhorIndividuo, int geracao);

        // --- Conclusão ---
        /// <summary>Chamado quando uma condição de paragem é atingida.</summary>
        void OnEvolucaoTerminada();

        /// <summary>
        /// Cria um tabela representativa da distribuição do fitness em classes
        /// </summary>
        (int intervalo, int quantidade)[] GerarHistograma(IList<TCromossoma> populacao);

        /// <summary>
        /// Cria um tabela representativa do TOP10 Fitness
        /// </summary>
        int[] Top10Fitness(IList<TCromossoma> Populacao);

    }
}
