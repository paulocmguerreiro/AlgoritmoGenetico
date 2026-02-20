using AlgoritmoGenetico.Aplicacao.Configuracao;
using AlgoritmoGenetico.Aplicacao.Extensao;
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Selecao
{
    /// <summary>
    /// Implementa a seleção por Roleta (Roulette Wheel).
    /// A probabilidade de escolha é proporcional ao Fitness do indivíduo.
    /// </summary>
    public class RouletteWheel<TCromossoma> : ISelecao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        protected List<TCromossoma>? _populacao;
        protected AGConfiguracao<TCromossoma>? _configuracao;

        /// <summary>
        /// Retorna uma descrição da estratégia de crossover.
        /// </summary>
        public override string ToString()
        {
            return "Roulette Wheel (cada individuo é escolhido aleatoriamente e proporcional ao seu fitness).";
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
            List<TCromossoma> novaPopulacao = new List<TCromossoma>(_configuracao!.DimensaoDaPopulacao);
            for (int i = 0; i < _configuracao.DimensaoDaPopulacao; i++)
            {
                novaPopulacao.Add(_populacao!.ObterIndividuoPeloFitness(_configuracao!));
            }

            return novaPopulacao;
        }

        /// <summary>
        /// Implementa a lógica de seleção dum individuo.
        /// </summary>
        public TCromossoma EscolherIndividuo()
        {
            return _populacao!.ObterIndividuoPeloFitness(_configuracao!);
        }

    }
}
