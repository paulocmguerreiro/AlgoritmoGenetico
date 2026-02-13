using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AlgoritmoGenetico.Individuo
{
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
