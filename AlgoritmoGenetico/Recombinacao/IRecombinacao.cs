using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Recombinacao
{
    /// <summary>
    /// Define o contrato para os operadores de recombinação (Crossover).
    /// Responsável por combinar o material genético de dois progenitores para criar descendentes.
    /// </summary>
    /// <typeparam name="TCromossoma">O tipo de cromossoma a ser recombinado.</typeparam>
    public interface IRecombinacao<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        /// <summary> Retorna a descrição da estratégia de crossover. </summary>
        string ToString();

        /// <summary>
        /// Combina dois cromossomas pais para gerar uma lista de filhos (geralmente dois).
        /// </summary>
        /// <param name="pai1">O primeiro progenitor.</param>
        /// <param name="pai2">O segundo progenitor.</param>
        /// <returns>Uma lista contendo os descendentes gerados.</returns>
        public List<TCromossoma> Combinar(TCromossoma pai1, TCromossoma pai2);
    }
}
