using AlgoritmoGenetico.Individuo;
using HorarioEscolar.Factories;
using HorarioEscolar.Helper;
using HorarioEscolar.Individuo.Extension;

namespace HorarioEscolar.Individuo
{
    public class HorarioCromossomaFactory(ITurmaService turmaService, IDisciplinaService disciplinaService) : ICromossomaFactory<HorarioCromossoma>
    {
        public HorarioCromossoma CriarAleatorio()
        {
            HorarioCromossoma cromossoma = new HorarioCromossoma();
            foreach (var turma in turmaService.AsList())
            {
                foreach (var disciplinaProfessor in turma.Professores)
                {
                    List<int> temposLetivosDaDisciplina = disciplinaService.ObterTemposLetivos(disciplinaProfessor.Key);
                    cromossoma.AdicionarGene(HorarioGeneFactory.GetGene(
                        turma.Sigla,
                        disciplinaProfessor.Value,
                        disciplinaProfessor.Key,
                        false,
                        disciplinaService.DistribuirTemposLetivos(disciplinaProfessor.Key, temposLetivosDaDisciplina)
                    ));
                }
            }
            return cromossoma;
        }

        public HorarioCromossoma CriarVazio()
        {
            return new HorarioCromossoma { Genes = new List<HorarioGene>() };
        }
    }
}
