using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Estrutura;
using HorarioEscolar.Individuo;

namespace HorarioEscolar.Helper
{
    public abstract class ProfessorBaseService(IHorarioService horarioService) : IProfessorService
    {
        protected Dictionary<string, ProfessorCSV>? Professores;

        public List<ProfessorCSV>? ToList()
        {
            return Professores!.Values.ToList();
        }

        public int ObterProfIndicacaoDeTempoBloqueado(string? prof, int diaDaSemana, string hora)
        {
            if (prof is null)
            {
                return 0;
            }
            ProfessorCSV profAConsultar = Professores![prof];
            return profAConsultar.TemposLetivos[horarioService.ObterIndexAPartirDaHora(hora) * horarioService.DiasDaSemanaIndex.Count + diaDaSemana];

        }
        public abstract void CarregarDados();
        public List<HorarioGene> ObterHorarioProf(HorarioCromossoma genes, string prof) =>
            genes.Genes.Where(x => x.Professor.Equals(prof)).ToList();

    }
}
