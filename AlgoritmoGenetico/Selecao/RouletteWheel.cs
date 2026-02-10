using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;
using AlgoritmoGenetico.Extensao;

namespace AlgoritmoGenetico.Selecao
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
        public override string ToString()
        {
            return "Roulette Wheel (cada individuo é escolhido aleatoriamente e proporcional ao seu fitness).";
        }
        public void Preparar(List<TCromossoma> populacaoCompleta, AGConfiguracao<TCromossoma> configuracaoAtual)
        {
            _configuracao = configuracaoAtual;
            _populacao = populacaoCompleta.OrderByFitness(_configuracao.ProcessoDeEvolucao) as List<TCromossoma>;
        }

        public List<TCromossoma> EscolherPopulacao()
        {
            List<TCromossoma> novaPopulacao = new List<TCromossoma>(_configuracao!.DimensaoDaPopulacao);
            for (int i = 0; i < _configuracao.DimensaoDaPopulacao; i++)
            {
                novaPopulacao.Add(_populacao!.ObterIndividuoPeloFitness(_configuracao!));
            }

            return novaPopulacao;
        }

        public TCromossoma EscolherIndividuo()
        {
            return _populacao!.ObterIndividuoPeloFitness(_configuracao!);
        }

    }
}
