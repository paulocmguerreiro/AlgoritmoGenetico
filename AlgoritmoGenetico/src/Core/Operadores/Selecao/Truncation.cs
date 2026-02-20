using AlgoritmoGenetico.Aplicacao.Configuracao;
using AlgoritmoGenetico.Aplicacao.Extensao;
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Selecao
{
    /// <summary>
    /// Implementa a seleção por Truncamento.
    /// Seleciona os melhores indivíduos da população com base estrita no seu Fitness.
    /// </summary>
    public class Truncation<TCromossoma> : ISelecao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {

        protected List<TCromossoma>? _populacao;
        protected AGConfiguracao<TCromossoma>? _configuracao;
        /// <summary>
        /// Retorna uma descrição da estratégia de crossover.
        /// </summary>
        public override string ToString()
        {
            return "Truncation (manter o top dos individuos de acordo com o seu Fitness).";
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
            return _populacao!
                .Take(_configuracao!.DimensaoDaPopulacao)
                .ToList();
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
