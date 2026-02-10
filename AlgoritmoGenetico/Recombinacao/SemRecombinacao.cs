using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Recombinacao
{
    /// <summary>
    /// Estratégia de preservação integral (Pass-through).
    /// Não realiza mistura genética; os filhos são clones exatos dos pais. 
    /// Útil para algoritmos que dependem exclusivamente de mutação ou para depuração (debugging).
    /// </summary>
    public class SemRecombinacao<TCromossoma> : IRecombinacao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        /// <summary>
        /// Devolve cópias dos dois progenitores sem alterações.
        /// </summary>
        public List<TCromossoma> Combinar(TCromossoma pai1, TCromossoma pai2)
        {
            return [(TCromossoma)pai1.Clone(), (TCromossoma)pai2.Clone()];
        }

        public override string ToString()
        {
            return "Sem estratégia de recombinação.";
        }
    }
}
