
using AlgoritmoGenetico.Aplicacao.Motor;
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Infraestrutura.Output
{
    /// <summary>
    /// Classe base para serviços de output do algoritmo genético, fornecendo métodos abstratos para eventos-chave e utilitários para análise de população.
    /// </summary>
    /// <typeparam name="TCromossoma"></typeparam>
    public abstract class AGOutputBaseService<TCromossoma> : IAGOutputService<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {

        /// <summary>
        /// Evento disparado quando a evolução é iniciada, permitindo que implementações personalizadas realizem ações específicas.
        /// </summary>
        public abstract void OnEvolucaoIniciada();

        /// <summary>
        /// Evento disparado quando a evolução é terminada, permitindo que implementações personalizadas realizem ações específicas, como exibir resultados ou limpar recursos.
        /// </summary>
        public abstract void OnEvolucaoTerminada();

        /// <summary>
        /// Evento disparado quando uma geração é processada, permitindo que implementações personalizadas realizem ações específicas, como atualizar visualizações ou registrar estatísticas.
        /// </summary>
        /// <param name="algoritmo"></param>
        public abstract void OnGeracaoProcessada(AG<TCromossoma> algoritmo);

        /// <summary>
        /// Evento disparado quando a melhor solução é encontrada, permitindo que implementações personalizadas realizem ações específicas, como exibir a solução ou registrar seu desempenho.
        /// </summary>
        /// <param name="melhorIndividuo"></param>
        /// <param name="geracao"></param>
        public abstract void OnMelhorSolucaoEncontrada(TCromossoma melhorIndividuo, int geracao);

        /// <summary>
        /// Método utilitário para obter os valores de fitness dos 10 melhores indivíduos de uma população, facilitando a análise do desempenho da população ao longo das gerações.
        /// </summary>
        /// <param name="Populacao"></param>
        /// <returns></returns>
        public int[] Top10Fitness(IList<TCromossoma> Populacao) => Populacao.Take(10).Select(ind => ind.Fitness).ToArray();

        /// <summary>
        /// Método utilitário para gerar um histograma de fitness a partir de uma população, calculando a distribuição dos valores de fitness em intervalos definidos, o que pode ajudar a visualizar a diversidade e a convergência da população ao longo das gerações.
        /// </summary>
        /// <param name="populacao"></param>
        /// <returns></returns>
        public (int intervalo, int quantidade)[] GerarHistograma(IList<TCromossoma> populacao)
        {

            int dimensaoPopulacao = populacao.Count;
            if (dimensaoPopulacao == 0)
            {
                return [];
            }

            double minFitness = populacao.Min(x => x.Fitness);
            double maxFitness = populacao.Max(x => x.Fitness);

            // Métodos para calcular a quantidade de classes
            int numeroClassesRaizQuadradaDimensao = (int)Math.Sqrt(dimensaoPopulacao);
            int numeroClasses = (int)Math.Clamp(numeroClassesRaizQuadradaDimensao, 1, 15.0);
            (int intervalo, int quantidade)[] histograma = new (int, int)[numeroClasses];

            // Evitar divisão por zero se todos forem iguais, fica tudo na primeira classe
            if (minFitness == maxFitness)
            {
                histograma[0] = (intervalo: (int)minFitness, quantidade: dimensaoPopulacao);
                return histograma;
            }

            double intervalo = (maxFitness - minFitness) / numeroClasses;

            populacao.Select(x => x.Fitness).ToList().ForEach(fitness =>
            {
                int index = Math.Clamp((int)((fitness - minFitness) / intervalo), 0, numeroClasses - 1);
                histograma[index].intervalo = (int)(minFitness + (index + 1) * intervalo);
                histograma[index].quantidade++;
            });

            return histograma;
        }

    }
}
