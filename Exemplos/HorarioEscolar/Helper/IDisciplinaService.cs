using HorarioEscolar.Individuo;

namespace HorarioEscolar.Helper
{
    public interface IDisciplinaService
    {
        void CarregarDados();
        List<int> ObterTemposLetivos(string sigla);
        List<string> ObterSalas(string sigla);
        string ObterSalaAleatoria(string sigla);
        List<HorarioDiasGene> DistribuirTemposLetivos(string disciplinaSigla, List<int> temposLetivosDaDisciplina);
    }
}
