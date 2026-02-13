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
    public abstract class RecombinacaoBase<TCromossoma>(ICromossomaFactory<TCromossoma> cromossomaFactory) : IRecombinacao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        public abstract List<TCromossoma> Combinar(TCromossoma pai1, TCromossoma pai2);
        public override string ToString()
        {
            return "";
        }
    }
}
