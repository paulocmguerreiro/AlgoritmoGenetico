using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Recombinacao
{
    /// <summary>
    /// Implementa o Crossover Uniforme.
    /// Para cada gene, decide aleatoriamente (50% de probabilidade) de qual pai o filho herdará a informação.
    /// </summary>
    public class Uniforme<TCromossoma> : IRecombinacao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        protected List<TCromossoma> NovaPopulacao = [];
        public override string ToString()
        {
            return "Recombinação Uniforme (o filho é criado com o mesmos pais para todos os genes e são distribuídos aleatoriamente).";
        }
        /// <summary>
        /// Realiza a combinação gene a gene com base em sorteio probabilístico.
        /// </summary>
        public List<TCromossoma> Combinar(TCromossoma pai1, TCromossoma pai2)
        {
            TCromossoma filho1 = (TCromossoma)TCromossoma.CriarVazio();
            TCromossoma filho2 = (TCromossoma)TCromossoma.CriarVazio();

            IReadOnlyList<IGene> pai1Genes = pai1.Genes as IReadOnlyList<IGene> ?? pai1.Genes.ToList();
            IReadOnlyList<IGene> pai2Genes = pai2.Genes as IReadOnlyList<IGene> ?? pai2.Genes.ToList();
            int totalDeGenes = pai1Genes.Count;

            for (int i = 0; i < totalDeGenes; i++)
            {
                bool trocarPosicoes = Random.Shared.NextDouble() <= 0.5d;
                filho1.AdicionarGene((trocarPosicoes ? pai1Genes : pai2Genes)[i]);
                filho2.AdicionarGene((trocarPosicoes ? pai2Genes : pai1Genes)[i]);
            }
            filho1.ResetFitness();
            filho2.ResetFitness();
            return [filho1, filho2];
        }
    }
}
