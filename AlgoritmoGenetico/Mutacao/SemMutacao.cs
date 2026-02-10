using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;

namespace AlgoritmoGenetico.Mutacao
{
    /// <summary>
    /// Sem estratégia de mutação .
    /// </summary>
    public class SemMutacao<TCromossoma> : IMutacao<TCromossoma>
    where TCromossoma : ICromossoma<IGene>
    {
        public void Mutar(List<TCromossoma> populacao, AGConfiguracao<TCromossoma> configuracao, int geracoesSemEvolucao)
        {
        }
        public bool PodeMutar(IGene gene) => false;
        public override string ToString() => "Sem estratégia de mutação.";
    }
}
