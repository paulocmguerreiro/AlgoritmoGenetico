using System.Collections.Frozen;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Entidades;
using HorarioEscolar.Core.Interfaces;

namespace HorarioEscolar.Infraestrutura.Persistencia.Abstracoes
{
    /// <summary>
    /// Classe base abstrata para o serviço de turmas, fornecendo uma implementação comum para os métodos relacionados às turmas, como carregar dados, obter indicações de horários bloqueados e obter o horário de uma turma específica.
    /// </summary>
    /// <param name="horarioService"></param>
    public abstract class TurmaBaseService(IHorarioService horarioService) : ITurmaService
    {
        protected Dictionary<string, Turma>? _turmas;
        protected List<Turma>? _turmasList;

        public List<Turma> AsList()
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
            Turma turmaAConsultar = _turmas![turma];
            return turmaAConsultar.TemposLetivos[horarioService.ObterIndexAPartirDaHora(hora) * horarioService.DiasDaSemanaIndex.Count + diaDaSemana];

        }

        public FrozenDictionary<string, Turma> ToDictionary()
        {
            ValidarTurmasInicializacao();
            return _turmas!.ToFrozenDictionary();
        }

        /// <summary>
        /// Valida se as turmas foram inicializadas, lançando uma exceção caso contrário. Este método é utilizado para garantir que os dados das turmas estejam disponíveis antes de acessar seus métodos, evitando erros de referência nula e garantindo a integridade dos dados durante a execução do programa.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public void ValidarTurmasInicializacao()
        {
            if (_turmas is null)
            {
                throw new NullReferenceException("Turmas não foram inicializadas");
            }
        }

        public List<HorarioGene> ObterHorarioTurma(HorarioCromossoma genes, string turma) =>
            genes.Genes.Where(x => x.Turma.Equals(turma)).ToList();

    }
}
