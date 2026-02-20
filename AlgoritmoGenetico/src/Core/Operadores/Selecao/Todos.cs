
using AlgoritmoGenetico.Aplicacao.Configuracao;
using AlgoritmoGenetico.Aplicacao.Extensao;
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Selecao
{
    /// <summary>
    /// Implementa a seleção por Todos, onde toda a população é mantida para a próxima geração, sem filtragem ou seleção, garantindo que todas as soluções candidatas sejam preservadas e possam contribuir para a diversidade genética e evolução do algoritmo.
    /// </summary>
    /// <typeparam name="TCromossoma"></typeparam>
    public class Todos<TCromossoma> : ISelecao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        protected List<TCromossoma>? _populacao;
        protected AGConfiguracao<TCromossoma>? _configuracao;
        /// <summary>
        /// Retorna uma descrição da estratégia de crossover.
        /// </summary>
        public override string ToString()
        {
            return "Todos (manter o top fitness de toda a população na sua totalidade ).";
        }
        /// <summary>
        /// Prepara a população para a seleção, ordenando-a por fitness.
        /// </summary>
        public void Preparar(List<TCromossoma> populacaoCompleta, AGConfiguracao<TCromossoma> configuracaoAtual)
        {
            _configuracao = configuracaoAtual;
            _populacao = populacaoCompleta.OrderByFitness(_configuracao.ProcessoDeEvolucao) as List<TCromossoma>;
        }
        /// <summary>
        /// Prepara a população para a seleção, ordenando-a por fitness.
        /// </summary>
        public List<TCromossoma> EscolherPopulacao()
        {
            return _populacao ?? new();
        }
        /// <summary>
        /// Implementa a lógica de seleção dum individuo.
        /// </summary>

        public TCromossoma EscolherIndividuo()
        {
            return _populacao![Random.Shared.Next(_populacao.Count)];
        }

    }
}
