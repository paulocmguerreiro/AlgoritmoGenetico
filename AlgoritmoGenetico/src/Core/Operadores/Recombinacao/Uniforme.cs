
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Recombinacao
{
    /// <summary>
    /// Implementa o Crossover Uniforme.
    /// Para cada gene, decide aleatoriamente (50% de probabilidade) de qual pai o filho herdará a informação.
    /// </summary>
    public class Uniforme<TCromossoma>(ICromossomaFactory<TCromossoma> cromossomaFactory) : RecombinacaoBase<TCromossoma>(cromossomaFactory)
    where TCromossoma : ICromossoma<IGene>
    {
        /// <summary>
        /// Lista para armazenar a nova população gerada após a recombinação. Cada vez que o método Combinar é chamado, os filhos resultantes da combinação dos pais são adicionados a esta lista, permitindo que a nova geração de indivíduos seja construída ao longo do processo de recombinação.
        /// </summary>
        protected List<TCromossoma> NovaPopulacao = [];

        /// <summary>
        /// Retorna uma descrição da estratégia de crossover.
        /// </summary>
        public override string ToString()
        {
            return "Recombinação Uniforme (o filho é criado com o mesmos pais para todos os genes e são distribuídos aleatoriamente).";
        }
        /// <summary>
        /// Realiza a combinação gene a gene com base em sorteio probabilístico.
        /// </summary>
        public override List<TCromossoma> Combinar(TCromossoma pai1, TCromossoma pai2)
        {
            TCromossoma filho1 = cromossomaFactory.CriarVazio();
            TCromossoma filho2 = cromossomaFactory.CriarVazio();

            IReadOnlyList<IGene> pai1Genes = pai1.Genes as IReadOnlyList<IGene> ?? pai1.Genes.ToList();
            IReadOnlyList<IGene> pai2Genes = pai2.Genes as IReadOnlyList<IGene> ?? pai2.Genes.ToList();
            int totalDeGenes = pai1Genes.Count;

            for (int i = 0; i < totalDeGenes; i++)
            {
                bool trocarPosicoes = Random.Shared.NextDouble() <= 0.5d;
                filho1.AdicionarGene((trocarPosicoes ? pai1Genes : pai2Genes)[i]);
                filho2.AdicionarGene((trocarPosicoes ? pai2Genes : pai1Genes)[i]);
            }
            filho1.Reset();
            filho2.Reset();
            return [filho1, filho2];
        }
    }
}
