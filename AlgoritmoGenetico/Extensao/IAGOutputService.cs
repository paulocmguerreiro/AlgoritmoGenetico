using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Extensao
{
    public interface IAGOutputService<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        // --- Início ---
        /// <summary>Chamado antes de iniciar a primeira geração.</summary>
        void OnEvolucaoIniciada();

        // --- Evolução (Feedback a cada geração ou X gerações) ---
        /// <summary>Chamado após cada geração ser processada.</summary>
        /// <param name="info">Objeto com dados da geração atual (nº, fitness médio, melhor fitness, diversidade, etc.)</param>
        void OnGeracaoProcessada(AG<TCromossoma> algoritmo);

        /// <summary>Opcional: Chamado quando o AG encontra uma nova melhor solução.</summary>
        void OnMelhorSolucaoEncontrada(TCromossoma melhorIndividuo, int geracao);

        // --- Conclusão ---
        /// <summary>Chamado quando uma condição de paragem é atingida.</summary>
        /// <param name="resultado">Objeto com o resumo final (Tempo total, total de gerações, causa da paragem)</param>
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
