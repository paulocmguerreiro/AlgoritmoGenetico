using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Recombinacao
{
    /// <summary>
    /// Implementa uma estratégia de recombinação alternada.
    /// Distribui os genes dos progenitores de forma intercalada: genes em índices pares são herdados de um pai, 
    /// e genes em índices ímpares do outro, garantindo uma mistura equitativa de 50% de cada progenitor.
    /// </summary>
    public class CycleCrossOver<TCromossoma>(ICromossomaFactory<TCromossoma> cromossomaFactory) : RecombinacaoBase<TCromossoma>(cromossomaFactory)
    where TCromossoma : ICromossoma<IGene>
    {
        /// <summary>
        /// Retorna uma descrição da estratégia de crossover, indicando que os genes em índices pares do primeiro pai e os genes em índices ímpares do segundo pai são combinados para formar os filhos. A descrição destaca a natureza alternada da recombinação, enfatizando a mistura equitativa dos genes dos progenitores.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Recombinação CycleCrossOver (os Genes pares vão para o filho1 e os Genes impares para o filho2).";
        }
        /// <summary>
        /// Combina os genes dos pais alternando a origem com base na paridade do índice (i % 2).
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
                bool trocarPosicoes = (i % 2) == 0;
                filho1.AdicionarGene((trocarPosicoes ? pai1Genes : pai2Genes)[i]);
                filho2.AdicionarGene((trocarPosicoes ? pai2Genes : pai1Genes)[i]);
            }
            filho1.Reset();
            filho2.Reset();
            return [filho1, filho2];
        }
    }
}
