
namespace AlgoritmoGenetico.Core.Abstracoes
{
    /// <summary>
    /// Define o contrato para os operadores de recombinação (Crossover).
    /// Responsável por combinar o material genético de dois progenitores para criar descendentes.
    /// </summary>
    public interface IRecombinacao<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        /// <summary> Retorna a descrição da estratégia de crossover. </summary>
        string ToString();

        /// <summary>
        /// Combina dois cromossomas pais para gerar uma lista de filhos (dois).
        /// </summary>
        public List<TCromossoma> Combinar(TCromossoma pai1, TCromossoma pai2);
    }
}
