using System.Reflection.Metadata.Ecma335;
using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Mutacao
{
    /// <summary>
    /// Estratégia de mutação que permite múltiplas tentativas de mutação por indivíduo na mesma geração.
    /// </summary>
    public abstract class Multipla<TCromossoma> : Unica<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        /// <summary> Número máximo de tentativas de mutação a realizar em cada indivíduo. </summary>
        public required int QuantidadeDeMutacoes { get; init; }
        public override string ToString() => $"Mutação em múltiplos genes, permitidas até {QuantidadeDeMutacoes} mutações, probabilidade de [{corAlertaTaxa}]{(FatorMutacaoNormal + _fatorMutacaoColisaoInicial) * 100.0,6:F4}%[/] sem colisão e de [{corAlertaTaxa}]{(FatorMutacaoColisao + _fatorMutacaoColisaoInicial) * 100.0,6:F4}%[/] em genes com colisão .";
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
