
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Mutacao
{
    /// <summary>
    /// Estratégia de mutação que permite múltiplas tentativas de mutação por indivíduo na mesma geração.
    /// </summary>
    public abstract class Multipla<TCromossoma> : Unica<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        /// <summary> Número máximo de tentativas de mutação a realizar em cada indivíduo. </summary>
        public required int QuantidadeDeMutacoes { get; init; }
        /// <summary>
        /// Retorna uma descrição da estratégia de mutação e os seus parâmetros atuais, incluindo as taxas de mutação para genes em colisão e normais, bem como o gatilho de ajuste adaptativo. A descrição é formatada para destacar visualmente as taxas de mutação, usando cores para indicar se a taxa aumentada está ativa (vermelho) ou não (verde). Além disso, informa o número máximo de tentativas de mutação permitidas por indivíduo, indicando que múltiplas mutações podem ocorrer dentro da mesma geração.
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Mutação em múltiplos genes, permitidas até {QuantidadeDeMutacoes} mutações, probabilidade de [{_corAlertaTaxa}]{(FatorMutacaoNormal + _fatorMutacaoColisaoInicial) * 100.0,6:F4}%[/] sem colisão e de [{_corAlertaTaxa}]{(FatorMutacaoColisao + _fatorMutacaoColisaoInicial) * 100.0,6:F4}%[/] em genes com colisão .";
        /// <summary>
        /// Aplica a lógica de mutação à população atual, indicando a quantidade de gerações sem evolução para permitir ajustes adaptativos. A implementação percorre cada indivíduo da população e aplica o processo de mutação específico definido em ProcessarMutacao múltiplas vezes, conforme definido por QuantidadeDeMutacoes. O método também chama AjustarFatorMutacao para atualizar as taxas de mutação com base no progresso da evolução, aumentando a agressividade da mutação quando necessário para tentar escapar de estagnação.
        /// </summary>
        /// <param name="populacao"></param>
        /// <param name="geracoesSemEvolucao"></param>
        public override void Mutar(List<TCromossoma> populacao, int geracoesSemEvolucao)
        {
            AjustarFatorMutacao(geracoesSemEvolucao);

            populacao.ForEach(individuo =>
            {
                int totalMutacoes = QuantidadeDeMutacoes;
                while (totalMutacoes-- > 0)
                {
                    ProcessarMutacao(individuo);
                }

            });

        }
    }
}
