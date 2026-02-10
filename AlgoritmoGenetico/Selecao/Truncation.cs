using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;
using AlgoritmoGenetico.Extensao;
namespace AlgoritmoGenetico.Selecao
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
        public override string ToString()
        {
            return "Truncation (manter o top dos individuos de acordo com o seu Fitness).";
        }

        public void Preparar(List<TCromossoma> populacaoCompleta, AGConfiguracao<TCromossoma> configuracaoAtual)
        {
            _configuracao = configuracaoAtual;
            _populacao = populacaoCompleta.OrderByFitness(_configuracao.ProcessoDeEvolucao) as List<TCromossoma>;
        }
        public List<TCromossoma> EscolherPopulacao()
        {
            return _populacao!
                .Take(_configuracao!.DimensaoDaPopulacao)
                .ToList();
        }
        public TCromossoma EscolherIndividuo()
        {
            return _populacao![Random.Shared.Next(_populacao.Count)];
        }
    }
}
