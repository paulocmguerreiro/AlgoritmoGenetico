
using AlgoritmoGenetico.Aplicacao.Configuracao;
using AlgoritmoGenetico.Aplicacao.Extensao;
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Selecao
{
    /// <summary>
    /// Implementa a estratégia de Crowding (Substituição por Similaridade).
    /// Utiliza o CodigoUnico para garantir que indivíduos com o mesmo Fitness, 
    /// mas genéticas diferentes, sejam preservados.
    /// </summary>
    public class Crowding<TCromossoma> : ISelecao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        protected List<TCromossoma>? _populacao;
        protected AGConfiguracao<TCromossoma>? _configuracao;

        /// <summary>
        /// Retorna uma descrição da estratégia de crossover.
        /// </summary>
        public override string ToString()
        {
            return "Crowding (manter unicamente individuos distintos com base no Fitness e num ID para manter a diversidade e soluções distintas mas que tem o mesmo Fitness).";
        }

        /// <summary>
        /// Prepara a população para a seleção, ordenando-a por fitness e removendo duplicados com base no código único.
        /// </summary>
        public void Preparar(List<TCromossoma> populacaoCompleta, AGConfiguracao<TCromossoma> configuracaoAtual)
        {
            _configuracao = configuracaoAtual;
            _populacao = populacaoCompleta
            .OrderByFitness(_configuracao.ProcessoDeEvolucao)
            .DistinctByFitness();
        }

        /// <summary>
        /// Seleciona os indivíduos para a próxima geração, garantindo que apenas indivíduos distintos sejam mantidos, promovendo a diversidade genética.
        /// </summary>
        public List<TCromossoma> EscolherPopulacao()
        {
            return _populacao!
            .Take(_configuracao!.DimensaoDaPopulacao)
            .ToList();
        }

        /// <summary>
        /// Implementa a lógica de seleção por Crowding, onde indivíduos com o mesmo Fitness são comparados usando o Código Único para garantir que apenas soluções distintas sejam mantidas, promovendo a diversidade genética e evitando a perda de soluções candidatas valiosas.
        /// </summary>
        public TCromossoma EscolherIndividuo()
        {
            return _populacao![Random.Shared.Next(_populacao.Count)];
        }
    }
}
