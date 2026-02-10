using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Recombinacao
{
    /// <summary>
    /// Implementa o Crossover de Ponto Único.
    /// Escolhe um índice aleatório e troca as caudas genéticas entre os dois pais.
    /// </summary>
    public class SinglePoint<TCromossoma> : IRecombinacao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        public override string ToString()
        {
            return "Recombinação SinglePoint (é criado um ponto de rutura e os filhos são criados intercalando os genes dos pais no ponto de rutura).";
        }
        /// <summary>
        /// Realiza a combinação trocando genes a partir de um ponto de corte aleatório.
        /// </summary>
        public List<TCromossoma> Combinar(TCromossoma pai1, TCromossoma pai2)
        {
            TCromossoma filho1 = (TCromossoma)TCromossoma.CriarVazio();
            TCromossoma filho2 = (TCromossoma)TCromossoma.CriarVazio();

            IReadOnlyList<IGene> pai1Genes = pai1.Genes as IReadOnlyList<IGene> ?? pai1.Genes.ToList();
            IReadOnlyList<IGene> pai2Genes = pai2.Genes as IReadOnlyList<IGene> ?? pai2.Genes.ToList();

            int posicaoSubstituicao = Random.Shared.Next(pai1Genes.Count);

            int totalDeGenes = pai1Genes.Count;
            for (int i = 0; i < totalDeGenes; i++)
            {
                bool trocarPosicoes = i >= posicaoSubstituicao;
                filho1.AdicionarGene((trocarPosicoes ? pai1Genes : pai2Genes)[i]);
                filho2.AdicionarGene((trocarPosicoes ? pai2Genes : pai1Genes)[i]);
            }
            filho1.ResetFitness();
            filho2.ResetFitness();
            return [filho1, filho2];
        }
    }
}
