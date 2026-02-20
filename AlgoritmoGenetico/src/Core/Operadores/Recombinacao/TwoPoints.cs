
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Recombinacao
{
    /// <summary>
    /// Implementa o Crossover de Dois Pontos.
    /// Seleciona um segmento central aleatório e troca-o entre os progenitores.
    /// </summary>
    public class TwoPoints<TCromossoma>(ICromossomaFactory<TCromossoma> cromossomaFactory) : RecombinacaoBase<TCromossoma>(cromossomaFactory)
    where TCromossoma : ICromossoma<IGene>
    {
        /// <summary>
        /// Retorna uma descrição da estratégia de crossover.
        /// </summary>
        public override string ToString()
        {
            return "Recombinação TwoPoints (são criados dois pontos de rutura e os filhos são criados intercalando os genes dos pais nos pontos de rutura).";
        }
        /// <summary>
        /// Realiza a combinação trocando o segmento delimitado por dois pontos de corte.
        /// </summary>
        public override List<TCromossoma> Combinar(TCromossoma pai1, TCromossoma pai2)
        {
            TCromossoma filho1 = cromossomaFactory.CriarVazio();
            TCromossoma filho2 = cromossomaFactory.CriarVazio();

            IReadOnlyList<IGene> pai1Genes = pai1.Genes as IReadOnlyList<IGene> ?? pai1.Genes.ToList();
            IReadOnlyList<IGene> pai2Genes = pai2.Genes as IReadOnlyList<IGene> ?? pai2.Genes.ToList();

            int posicaoSubstituicao1 = Random.Shared.Next(pai1Genes.Count);
            int posicaoSubstituicao2 = Random.Shared.Next(pai1Genes.Count);

            // estão ordenados
            if (posicaoSubstituicao1 > posicaoSubstituicao2)
            {
                (posicaoSubstituicao1, posicaoSubstituicao2) = (posicaoSubstituicao2, posicaoSubstituicao1);
            }

            int totalDeGenes = pai1Genes.Count;
            for (int i = 0; i < totalDeGenes; i++)
            {
                bool trocarPosicoes = posicaoSubstituicao1 <= i && i <= posicaoSubstituicao2;
                filho1.AdicionarGene((trocarPosicoes ? pai1Genes : pai2Genes)[i]);
                filho2.AdicionarGene((trocarPosicoes ? pai2Genes : pai1Genes)[i]);
            }
            filho1.Reset();
            filho2.Reset();
            return [filho1, filho2];
        }
    }
}
