using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HorarioEscolar.Estrutura;
using HorarioEscolar.Individuo;

namespace HorarioEscolar.Helper
{
    public abstract class TurmaBaseService(IHorarioService horarioService) : ITurmaService
    {
        protected Dictionary<string, TurmaCSV>? Turmas;
        protected List<TurmaCSV>? _turmasList;

        public List<TurmaCSV> AsList()
        {
            ValidarTurmasInicializacao();
            return _turmasList!;
        }

        public abstract void CarregarDados();

        public int ObterTurmaIndicacaoDeTempoBloqueado(string? turma, int diaDaSemana, string hora)
        {
            if (turma is null)
            {
                return 0;
            }
            ValidarTurmasInicializacao();
            TurmaCSV turmaAConsultar = Turmas![turma];
            return turmaAConsultar.TemposLetivos[horarioService.ObterIndexAPartirDaHora(hora) * horarioService.DiasDaSemanaIndex.Count + diaDaSemana];

        }

        public Dictionary<string, TurmaCSV> ToDictionary()
        {
            ValidarTurmasInicializacao();
            return Turmas!;
        }

        public void ValidarTurmasInicializacao()
        {
            if (Turmas is null)
            {
                throw new NullReferenceException("Turmas não foram inicializadas");
            }
        }

        public List<HorarioGene> ObterHorarioTurma(HorarioCromossoma genes, string turma) =>
            genes.Genes.Where(x => x.Turma.Equals(turma)).ToList();
    }
}
