using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Extensao
{
    public abstract class AGOutputBaseService<TCromossoma> : IAGOutputService<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {

        public abstract void OnEvolucaoIniciada();

        public abstract void OnEvolucaoTerminada();

        public abstract void OnGeracaoProcessada(AG<TCromossoma> algoritmo);

        public abstract void OnMelhorSolucaoEncontrada(TCromossoma melhorIndividuo, int geracao);

        public int[] Top10Fitness(IList<TCromossoma> Populacao) => Populacao.Take(10).Select(ind => ind.Fitness).ToArray();

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
