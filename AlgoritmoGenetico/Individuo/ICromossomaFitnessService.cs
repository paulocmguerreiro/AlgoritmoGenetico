using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlgoritmoGenetico.Individuo
{
    public interface ICromossomaFitnessService<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        int RecalcularFitness(TCromossoma individuo);

        int CalcularCodigoUnico(TCromossoma individuo);
    }
}
