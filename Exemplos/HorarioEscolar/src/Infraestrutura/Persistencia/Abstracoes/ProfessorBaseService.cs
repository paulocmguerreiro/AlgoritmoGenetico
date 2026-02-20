
using System.Collections.Frozen;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Entidades;
using HorarioEscolar.Core.Interfaces;

namespace HorarioEscolar.Infraestrutura.Persistencia.Abstracoes
{
    /// <summary>
    /// Classe base abstrata para o serviço de professores, fornecendo uma implementação comum para os métodos relacionados aos professores, como carregar dados, obter indicações de horários bloqueados e obter o horário de um professor específico.
    /// </summary>
    /// <param name="horarioService"></param>
    public abstract class ProfessorBaseService(IHorarioService horarioService) : IProfessorService
    {
        protected Dictionary<string, Professor>? _professores;

        public List<Professor>? ToList()
        {
            return _professores!.Values.ToList();
        }

        public int ObterProfIndicacaoDeTempoBloqueado(string? prof, int diaDaSemana, string hora)
        {
            if (prof is null)
            {
                return 0;
            }
            Professor profAConsultar = _professores![prof];
            return profAConsultar.TemposLetivos[horarioService.ObterIndexAPartirDaHora(hora) * horarioService.DiasDaSemanaIndex.Count + diaDaSemana];

        }
        public abstract void CarregarDados();
        public List<HorarioGene> ObterHorarioProf(HorarioCromossoma genes, string prof) =>
            genes.Genes.Where(x => x.Professor.Equals(prof)).ToList();

    }
}
