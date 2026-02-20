
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Recombinacao
{
    /// <summary>
    /// Estratégia de preservação integral (Pass-through).
    /// Não realiza mistura genética; os filhos são clones exatos dos pais. 
    /// Útil para algoritmos que dependem exclusivamente de mutação ou para depuração (debugging).
    /// </summary>
    public class SemRecombinacao<TCromossoma>(ICromossomaFactory<TCromossoma> cromossomaFactory) : RecombinacaoBase<TCromossoma>(cromossomaFactory)
    where TCromossoma : ICromossoma<IGene>
    {
        /// <summary>
        /// Devolve cópias dos dois progenitores sem alterações.
        /// </summary>
        public override List<TCromossoma> Combinar(TCromossoma pai1, TCromossoma pai2)
        {
            return [(TCromossoma)pai1.Clone(), (TCromossoma)pai2.Clone()];
        }

        /// <summary>
        /// Retorna uma descrição da estratégia de crossover. 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "Sem estratégia de recombinação.";
        }
    }
}
