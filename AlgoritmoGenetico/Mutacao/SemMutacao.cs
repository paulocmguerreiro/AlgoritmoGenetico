using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Mutacao
{
    /// <summary>
    /// Sem estratégia de mutação .
    /// </summary>
    public abstract class SemMutacao<TCromossoma> : IMutacaoService<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        public void Mutar(List<TCromossoma> populacao, int geracoesSemEvolucao)
        {
        }
        public bool PodeMutar(IGene gene) => false;

        public abstract void ProcessarMutacao(TCromossoma individuoAProcessar);

        public override string ToString() => "Sem estratégia de mutação.";
    }
}
