
namespace AlgoritmoGenetico.Core.Abstracoes
{
    /// <summary>
    /// Define o contrato para as estratégias de mutação no algoritmo genético.
    /// </summary>
    public interface IMutacaoService<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        /// <summary> Retorna uma descrição textual da estratégia de mutação e os seus parâmetros atuais. </summary>
        public string ToString();

        /// <summary>
        /// Aplica a lógica de mutação à população atual indicando a quantidade de gerações sem evolução (nova solução candidata).
        /// </summary>
        public void Mutar(List<TCromossoma> populacao, int geracoesSemEvolucao);

        /// <summary>
        /// Avalia se um gene específico pode sofrer mutação com base nas probabilidades definidas.
        /// </summary>
        public bool PodeMutar(IGene gene);

        /// <summary>
        /// Aplica o processo de mutação ao individuo, deve ser especializado para cada caso
        /// </summary>
        public abstract void ProcessarMutacao(TCromossoma individuoAProcessar);
    }
}
