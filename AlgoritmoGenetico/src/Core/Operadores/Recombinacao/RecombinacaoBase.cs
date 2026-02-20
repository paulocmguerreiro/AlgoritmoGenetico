
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Recombinacao
{
    /// <summary>
    /// Estratégia de combinação Base.
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
