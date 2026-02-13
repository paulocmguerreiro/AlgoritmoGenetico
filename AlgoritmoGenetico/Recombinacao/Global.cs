using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Recombinacao
{
    public class Global<TCromossoma>(ICromossomaFactory<TCromossoma> cromossomaFactory) : RecombinacaoBase<TCromossoma>(cromossomaFactory)
    where TCromossoma : ICromossoma<IGene>
    {
        // @TODO
        public override string ToString()
        {
            return "@TODO : Recombinação Global (o filho é criado com pais aleatorios para cada genes ).";
        }
        public override List<TCromossoma> Combinar(TCromossoma pai1, TCromossoma pai2)
        {
            TCromossoma filho1 = cromossomaFactory.CriarVazio();
            TCromossoma filho2 = cromossomaFactory.CriarVazio();

            IReadOnlyList<IGene> pai1Genes = pai1.Genes as IReadOnlyList<IGene> ?? pai1.Genes.ToList();
            IReadOnlyList<IGene> pai2Genes = pai2.Genes as IReadOnlyList<IGene> ?? pai2.Genes.ToList();
            int totalDeGenes = pai1Genes.Count;

            for (int i = 0; i < totalDeGenes; i++)
            {
                bool trocarPosicoes = Random.Shared.NextDouble() >= 0.5d;
                filho1.AdicionarGene((trocarPosicoes ? pai1Genes : pai2Genes)[i]);
                filho2.AdicionarGene((trocarPosicoes ? pai2Genes : pai1Genes)[i]);
            }
            filho1.Reset();
            filho2.Reset();
            return [filho1, filho2];
        }
    }
}
