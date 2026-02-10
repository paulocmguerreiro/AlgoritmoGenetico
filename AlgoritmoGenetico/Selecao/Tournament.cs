using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;
using AlgoritmoGenetico.Extensao;

namespace AlgoritmoGenetico.Selecao
{
    /// <summary>
    /// Implementa a seleção por Torneio.
    /// Escolhe aleatoriamente dois indivíduos e seleciona o melhor entre eles. 
    /// Reduz a pressão seletiva e ajuda a manter a diversidade.
    /// </summary>
    public class Tournament<TCromossoma> : ISelecao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {

        protected List<TCromossoma>? _populacao;
        protected AGConfiguracao<TCromossoma>? _configuracao;
        public override string ToString()
        {
            return "Tournamento (é escolhido sempre o individuo com o melhor de entre 2).";
        }

        public void Preparar(List<TCromossoma> populacaoCompleta, AGConfiguracao<TCromossoma> configuracaoAtual)
        {
            _configuracao = configuracaoAtual;
            _populacao = populacaoCompleta.OrderByFitness(_configuracao.ProcessoDeEvolucao) as List<TCromossoma>;
        }
        public List<TCromossoma> EscolherPopulacao()
        {

            List<TCromossoma> novaPopulacao = new List<TCromossoma>(_configuracao!.DimensaoDaPopulacao);
            while (novaPopulacao.Count < _configuracao.DimensaoDaPopulacao)
            {
                TCromossoma pai1 = _populacao!.ObterIndividuoAleatorio();
                TCromossoma pai2 = _populacao!.ObterIndividuoAleatorio();
                novaPopulacao.Add(
                    (TCromossoma)
                    (_configuracao.ProcessoDeEvolucao switch
                    {
                        AGProcessoDeEvolucao.MINIMIZACAO => pai1.Fitness <= pai2.Fitness ? pai1 : pai2,
                        _ => pai1.Fitness >= pai2.Fitness ? pai1 : pai2
                    }));
            }
            return novaPopulacao;

        }
        public TCromossoma EscolherIndividuo()
        {
            TCromossoma pai1 = _populacao!.ObterIndividuoAleatorio();
            TCromossoma pai2 = _populacao!.ObterIndividuoAleatorio();
            return (
                _configuracao!.ProcessoDeEvolucao switch
                {
                    AGProcessoDeEvolucao.MINIMIZACAO => pai1.Fitness <= pai2.Fitness ? pai1 : pai2,
                    _ => pai1.Fitness >= pai2.Fitness ? pai1 : pai2
                });
        }
    }
}
