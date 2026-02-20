
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Core.Operadores.Mutacao
{
    /// <summary>
    /// Estratégia de mutação que incide sobre um único ponto ou processo por indivíduo.
    /// Implementa mutação adaptativa, aumentando a probabilidade quando a evolução estagna.
    /// </summary>
    public abstract class Unica<TCromossoma> : IMutacaoService<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        protected const double MUTACAO_FATOR_PROGRESSO = 0.0001d;
        protected const double MUTACAO_CLAMP_MIN = 0d;
        protected const double MUTACAO_CLAMP_MAX = 0.3d;


        /// <summary> Probabilidade base de mutação para genes que estão marcados em colisão. </summary>
        public required double FatorMutacaoColisao { get; init; }
        /// <summary> Probabilidade base de mutação para genes em estado normal. </summary>
        public required double FatorMutacaoNormal { get; init; }
        /// <summary> Gatilho de gerações sem evolução necessário para começar a aumentar as taxas de mutação. </summary>
        public required int AjustarMutacaoACadaGeracao { get; init; }

        protected string _corAlertaTaxa => _fatorMutacaoColisaoInicial > 0 ? "red" : "green";

        protected double _fatorMutacaoColisaoInicial = 0;

        /// <summary>
        /// Retorna uma descrição da estratégia de mutação e os seus parâmetros atuais, incluindo as taxas de mutação para genes em colisão e normais, bem como o gatilho de ajuste adaptativo. A descrição é formatada para destacar visualmente as taxas de mutação, usando cores para indicar se a taxa aumentada está ativa (vermelho) ou não (verde).
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Mutação num único Gene, probabilidade de [{_corAlertaTaxa}]{(FatorMutacaoNormal + _fatorMutacaoColisaoInicial) * 100.0,6:F4}%[/] sem colisão e de [({_corAlertaTaxa}]{(FatorMutacaoColisao + _fatorMutacaoColisaoInicial) * 100.0,6:F4}%[/] em genes com colisão.";


        /// <summary>
        /// Ajusta dinamicamente a agressividade da mutação se a população parar de evoluir.
        /// </summary>
        protected void AjustarFatorMutacao(int geracoesSemEvolucao)
        {
            if (geracoesSemEvolucao > AjustarMutacaoACadaGeracao)
            {
                _fatorMutacaoColisaoInicial += MUTACAO_FATOR_PROGRESSO;
                _fatorMutacaoColisaoInicial = Math.Clamp(_fatorMutacaoColisaoInicial, MUTACAO_CLAMP_MIN, MUTACAO_CLAMP_MAX);
            }
            else
            {
                _fatorMutacaoColisaoInicial = 0;
            }
        }

        /// <summary>
        /// Aplica a lógica de mutação à população atual, indicando a quantidade de gerações sem evolução para permitir ajustes adaptativos. A implementação padrão percorre cada indivíduo da população e aplica o processo de mutação específico definido em ProcessarMutacao, que deve ser implementado por classes derivadas para definir a lógica de mutação concreta. O método também chama AjustarFatorMutacao para atualizar as taxas de mutação com base no progresso da evolução, aumentando a agressividade da mutação quando necessário para tentar escapar de estagnação.
        /// </summary>
        public virtual void Mutar(List<TCromossoma> populacao, int geracoesSemEvolucao)
        {
            AjustarFatorMutacao(geracoesSemEvolucao);

            populacao.ForEach(individuo => ProcessarMutacao(individuo));
        }

        /// <summary>
        /// Aplica o processo de mutação a um indivíduo específico. Este método é abstrato e deve ser implementado por classes derivadas para definir a lógica de mutação concreta, que pode variar dependendo do tipo de cromossoma e do problema em questão. A implementação típica envolve selecionar um gene específico dentro do indivíduo e aplicar uma alteração com base nas probabilidades definidas, utilizando o método PodeMutar para determinar se a mutação deve ocorrer.
        /// </summary>
        public abstract void ProcessarMutacao(TCromossoma individuoAProcessar);

        /// <summary>
        /// Verifica se um gene pode mutar, diferenciando as taxas entre genes com e sem colisão.
        /// </summary>
        public bool PodeMutar(IGene gene)
        {
            double probabilidadeMutacao = (gene.EstaEmColisao ? FatorMutacaoColisao : FatorMutacaoNormal) + _fatorMutacaoColisaoInicial;
            return Random.Shared.NextDouble() < probabilidadeMutacao;

        }

    }
}
