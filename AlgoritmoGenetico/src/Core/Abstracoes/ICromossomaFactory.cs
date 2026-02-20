using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlgoritmoGenetico.Core.Abstracoes
{
    /// <summary>
    /// Interface para fábrica de cromossomas, responsável por criar instâncias de cromossomas, seja vazios ou com dados aleatórios, respeitando as regras do problema específico. Permite a implementação de diferentes estratégias de criação de indivíduos para o algoritmo genético.
    /// </summary>
    public interface ICromossomaFactory<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        /// <summary>
        /// Criar uma instância do individuo sem genes
        /// </summary>
        public TCromossoma CriarVazio();


        /// <summary> Gera um individuo com dados aleatórios mas respeitando as cargas horárias. </summary>
        public TCromossoma CriarAleatorio();
    }
}
