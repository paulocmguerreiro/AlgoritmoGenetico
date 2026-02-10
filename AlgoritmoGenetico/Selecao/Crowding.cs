using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;
using AlgoritmoGenetico.Extensao;

namespace AlgoritmoGenetico.Selecao
{
    /// <summary>
    /// Implementa uma variação de Crowding (Substituição por Similaridade).
    /// Utiliza o CodigoUnico para garantir que indivíduos com o mesmo Fitness, 
    /// mas genéticas diferentes, sejam preservados.
    /// </summary>
    public class Crowding<TCromossoma> : ISelecao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        protected List<TCromossoma>? _populacao;
        protected AGConfiguracao<TCromossoma>? _configuracao;
        public override string ToString()
        {
            return "Crowding (manter unicamente individuos distintos com base no Fitness ID para manter a diversidade e soluções distintas e com mesmo Fitness).";
        }

        public void Preparar(List<TCromossoma> populacaoCompleta, AGConfiguracao<TCromossoma> configuracaoAtual)
        {
            _configuracao = configuracaoAtual;
            _populacao = populacaoCompleta
            .OrderByFitness(_configuracao.ProcessoDeEvolucao)
            .DistinctByFitness();
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
