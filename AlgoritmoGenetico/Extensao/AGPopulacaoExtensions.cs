using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;
using Microsoft.VisualBasic;

namespace AlgoritmoGenetico.Extensao
{
    /// <summary>
    /// Métodos de utilidade interna para manipulação, ordenação e processamento paralelo da população.
    /// </summary>
    internal static class AGPopulacaoExtensions
    {
        private static int WheelNeedle = 0;

        /// <summary>
        /// Ordena a população de acordo com o fitness, respeitando o sentido da evolução (Minimização ou Maximização).
        /// </summary>
        internal static IEnumerable<TCromossoma> OrderByFitness<TCromossoma>(this IEnumerable<TCromossoma> populacao, AGProcessoDeEvolucao processo)
        where TCromossoma : ICromossoma<IGene>
        {
            /*
            return (
                Processo switch
                {
                    AGProcessoDeEvolucao.MINIMIZACAO => Populacao.OrderBy(x => x.GetFitness()),
                    _ => Populacao.OrderByDescending(x => x.GetFitness())
                })
            .ToList();
            */
            // Converte para lista apenas uma vez
            //var lista = Populacao;//.ToList();

            // Para não gerar uma nova lista (.ToList()) 
            List<TCromossoma> lista = populacao as List<TCromossoma> ?? populacao.ToList();
            bool processoMinimizacao = processo == AGProcessoDeEvolucao.MINIMIZACAO;
            lista.Sort((a, b) => processoMinimizacao
                    ? a.Fitness.CompareTo(b.Fitness)
                    : b.Fitness.CompareTo(a.Fitness)
            );

            return lista;
        }

        /// <summary>
        /// Remove duplicados da população baseando-se na combinação de Fitness e Código Único, 
        /// promovendo a diversidade genética.
        /// </summary>
        internal static List<TCromossoma> DistinctByFitness<TCromossoma>(this IEnumerable<TCromossoma> populacao)
        where TCromossoma : ICromossoma<IGene> => populacao.DistinctBy((gene) => (gene.Fitness, gene.CodigoUnico)).ToList();

        /// <summary>
        /// Calcula o fitness de todos os indivíduos em paralelo, utilizando a capacidade multi-core do CPU.
        /// Deixa dois núcleos livres (Environment.ProcessorCount - 2) para manter a fluidez do sistema.
        /// </summary>
        internal static void RecalcularFitness<TCromossoma>(this List<TCromossoma> populacao)
        where TCromossoma : ICromossoma<IGene>
        {

            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount - 2
            };
            Parallel.For(0, populacao.Count, options, filhos =>
            {
                int _ = populacao[filhos].Fitness;
            });
        }

        /// <summary>
        /// Efetua o reset ao fitness dos individuos por forma a forçar o recalculo 
        /// </summary>
        internal static List<TCromossoma> ResetFitness<TCromossoma>(this List<TCromossoma> populacao)
        where TCromossoma : ICromossoma<IGene>
        {
            foreach (TCromossoma individuo in populacao)
            {
                individuo.ResetFitness();
            }
            return populacao;
        }

        /// <summary>
        /// Escolhe aleatoriamente um individuo na população
        /// </summary>
        public static TCromossoma ObterIndividuoAleatorio<TCromossoma>(this List<TCromossoma> populacao)
        {
            return populacao[Random.Shared.Next(populacao.Count)];
        }
        /// <summary>
        /// Implementa a seleção por Roleta. Escolhe um indivíduo onde a probabilidade é proporcional ao seu fitness.
        /// Em casos de minimização, inverte a lógica para beneficiar os valores mais baixos.
        /// </summary>
        public static TCromossoma ObterIndividuoPeloFitness<TCromossoma>(this IEnumerable<TCromossoma> populacao, AGConfiguracao<TCromossoma> configuracao)
        where TCromossoma : ICromossoma<IGene>
        {

            int totalFitness = populacao.Sum(x => x.Fitness);
            int fitnessMaximo = populacao.Max(x => x.Fitness);

            WheelNeedle = (Random.Shared.Next(totalFitness) + WheelNeedle) % totalFitness;

            TCromossoma individuoSelecionado = populacao.ElementAt(0);
            int fitnessAnalisado = 0;

            // No caso do processo de MINIMIZAÇÃO e como a forma do Fitness é normalizada pode acontecer necessitar de mais de uma volta à população
            while (true)
            {
                foreach (TCromossoma individuo in populacao)
                {
                    // Normaliza o fitness para seleção por roleta
                    fitnessAnalisado += configuracao.ProcessoDeEvolucao switch
                    {
                        AGProcessoDeEvolucao.MINIMIZACAO => fitnessMaximo - individuo.Fitness + 1,
                        _ => individuoSelecionado.Fitness
                    };

                    // A agulha parou aqui porque ultrapassou o fitness analisado
                    if (fitnessAnalisado >= WheelNeedle)
                    {
                        return individuo;
                    }

                }
            }
        }

    }

}
