using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;
using AlgoritmoGenetico.Extensao;

namespace AlgoritmoGenetico.Selecao
{
    public class Todos<TCromossoma> : ISelecao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        protected List<TCromossoma>? _populacao;
        protected AGConfiguracao<TCromossoma>? _configuracao;
        public override string ToString()
        {
            return "Todos (manter o top fitness de toda a população na sua totalidade ).";
        }

        public void Preparar(List<TCromossoma> populacaoCompleta, AGConfiguracao<TCromossoma> configuracaoAtual)
        {
            _configuracao = configuracaoAtual;
            _populacao = populacaoCompleta.OrderByFitness(_configuracao.ProcessoDeEvolucao) as List<TCromossoma>;
        }
        public List<TCromossoma> EscolherPopulacao()
        {
            return _populacao ?? new();
        }

        public TCromossoma EscolherIndividuo()
        {
            return _populacao![Random.Shared.Next(_populacao.Count)];
        }

    }
}
