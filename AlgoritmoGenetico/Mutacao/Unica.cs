using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Mutacao
{
    /// <summary>
    /// Estratégia de mutação que incide sobre um único ponto ou processo por indivíduo.
    /// Implementa mutação adaptativa, aumentando a probabilidade quando a evolução estagna.
    /// </summary>
    public class Unica<TCromossoma> : IMutacao<TCromossoma>
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

        protected string corAlertaTaxa => _fatorMutacaoColisaoInicial > 0 ? "red" : "green";

        protected double _fatorMutacaoColisaoInicial = 0;


        public override string ToString() => $"Mutação num único Gene, probabilidade de [{corAlertaTaxa}]{(FatorMutacaoNormal + _fatorMutacaoColisaoInicial) * 100.0,6:F4}%[/] sem colisão e de [({corAlertaTaxa}]{(FatorMutacaoColisao + _fatorMutacaoColisaoInicial) * 100.0,6:F4}%[/] em genes com colisão.";


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

        public virtual void Mutar(List<TCromossoma> populacao, AGConfiguracao<TCromossoma> configuracao, int geracoesSemEvolucao)
        {
            AjustarFatorMutacao(geracoesSemEvolucao);

            if (configuracao.ProcessarMutacaoCallback is null)
            {
                return;
            }

            populacao.ForEach(individuo => configuracao.ProcessarMutacaoCallback(individuo, configuracao));
        }

        /// <summary>
        /// Verifica se um gene deve mutar, diferenciando as taxas entre genes com e sem colisão.
        /// </summary>
        public bool PodeMutar(IGene gene)
        {
            double probabilidadeMutacao = (gene.EstaEmColisao ? FatorMutacaoColisao : FatorMutacaoNormal) + _fatorMutacaoColisaoInicial;
            return Random.Shared.NextDouble() < probabilidadeMutacao;

        }

    }
}
