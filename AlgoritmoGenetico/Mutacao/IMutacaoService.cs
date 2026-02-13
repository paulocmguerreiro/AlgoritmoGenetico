using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Mutacao
{
    /// <summary>
    /// Define o contrato para as estratégias de mutação no algoritmo genético.
    /// </summary>
    /// <typeparam name="TCromossoma">O tipo de cromossoma que sofrerá a mutação.</typeparam>
    public interface IMutacaoService<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {

        /// <summary> Retorna uma descrição textual da estratégia de mutação e os seus parâmetros atuais. </summary>
        public string ToString();

        /// <summary>
        /// Aplica a lógica de mutação à população atual.
        /// </summary>
        /// <param name="populacao">A lista de indivíduos a processar.</param>
        /// <param name="geracoesSemEvolucao">O número de gerações desde a última melhoria global (usado para mutação adaptativa).</param>
        public void Mutar(List<TCromossoma> populacao, int geracoesSemEvolucao);

        /// <summary>
        /// Avalia se um gene específico deve sofrer mutação com base nas probabilidades definidas.
        /// </summary>
        /// <param name="gene">O gene a testar.</param>
        /// <returns>Verdadeiro se o gene deve ser mutado.</returns>
        public bool PodeMutar(IGene gene);

        /// <summary>
        /// Aplica o processo de mutação ao individuo, deve ser especializado para cada caso
        /// </summary>
        /// <param name="individuoAProcessar"></param>
        public abstract void ProcessarMutacao(TCromossoma individuoAProcessar);
    }
}
