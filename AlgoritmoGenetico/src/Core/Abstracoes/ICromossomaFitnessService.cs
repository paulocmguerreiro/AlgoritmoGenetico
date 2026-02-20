using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlgoritmoGenetico.Core.Abstracoes
{
    /// <summary>
    /// Interface para serviço de cálculo de fitness e código único de um cromossoma. O serviço é responsável por avaliar a qualidade de um indivíduo (cromossoma) e gerar um identificador único baseado na composição dos genes, o que é crucial para manter a diversidade genética e para operações de seleção e sobrevivência, especialmente em estratégias como Crowding.
    /// </summary>
    /// <typeparam name="TCromossoma"></typeparam>
    public interface ICromossomaFitnessService<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        /// <summary>
        /// Calcula o valor de fitness para um dado indivíduo (cromossoma). O valor de fitness é uma medida da qualidade da solução representada pelo cromossoma, e o algoritmo genético irá minimizar ou maximizar este valor conforme o processo de evolução definido na configuração.
        /// </summary>
        int CalcularFitness(TCromossoma individuo);

        /// <summary>
        /// Calcula um código identificador único (hash) para um dado indivíduo (cromossoma) baseado na composição dos seus genes. Este código é útil para distinguir indivíduos distintos que podem ter o mesmo valor de fitness, ajudando a evitar perda de diversidade genética e a melhorar as operações de seleção e sobrevivência, especialmente em estratégias como Crowding.
        /// </summary>
        /// <param name="individuo"></param>
        /// <returns></returns>
        int CalcularCodigoUnico(TCromossoma individuo);
    }
}
