
using System.Collections.Frozen;
using System.Collections.ObjectModel;
using HorarioEscolar.Applicacao.DTO;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Entidades;
using HorarioEscolar.Core.Interfaces;

namespace HorarioEscolar.Infraestrutura.Persistencia.Abstracoes
{
    /// <summary>
    /// Classe base abstrata para o serviço de horários, fornecendo uma implementação comum para os métodos relacionados aos horários, como carregar dados, obter horas do dia, obter horários bloqueados, obter horas aleatórias e converter horários para um formato plano.
    /// </summary>
    public abstract class HorarioBaseService : IHorarioService
    {
        protected Dictionary<string, Horario>? _horarios;
        protected List<string>? _horariosList;
        protected Dictionary<string, int>? _horariosIndexMap;
        public ReadOnlyCollection<string> DiasDaSemanaDescritivo { get => Array.AsReadOnly(new string[] { "Segunda", "Terça", "Quarta", "Quinta", "Sexta" }); }
        public ReadOnlyCollection<int> DiasDaSemanaIndex { get => Array.AsReadOnly(new int[] { 0, 1, 2, 3, 4 }); }
        public abstract void CarregarDados();

        public string ObterHoraAleatoriaDoDia(int duracaoTemposLetivos) => ObterHoraAPartirDoIndex(ObterIndexDaHoraAleatoriaDoDia(duracaoTemposLetivos));

        public string ObterHoraAnterior(string horaInicial)
        {
            ValidarHorarioInicializacao();
            int posicaoNoDicionario = ObterIndexAPartirDaHora(horaInicial);
            return posicaoNoDicionario <= 0 ? horaInicial : ObterHoraAPartirDoIndex(posicaoNoDicionario - 1);

        }

        public string ObterHoraAPartirDoIndex(int horaInicioDaAulaIdx)
        {
            ValidarHorarioInicializacao();
            return _horarios!.ElementAt(horaInicioDaAulaIdx).Value.Hora;
        }

        public int ObterHorarioIndicacaoDeTempoBloqueado(int diaDaSemana, string hora)
        {
            Horario horaAConsultar = _horarios![hora];
            return horaAConsultar.Dias[diaDaSemana];
        }

        public ReadOnlyCollection<string> ObterHorasDoDia() => _horariosList!.AsReadOnly();

        public string ObterHoraSeguinte(string horaInicial)
        {
            ValidarHorarioInicializacao();
            int posicaoNoDicionario = ObterIndexAPartirDaHora(horaInicial);
            return posicaoNoDicionario switch
            {
                -1 => horaInicial,
                _ when posicaoNoDicionario == _horarios!.Count - 1 => horaInicial,
                _ => ObterHoraAPartirDoIndex(posicaoNoDicionario + 1)
            };
        }

        public int ObterIndexAPartirDaHora(string hora)
        {
            ValidarHorarioInicializacao();
            return _horariosIndexMap!.TryGetValue(hora, out int index) ? index : -1;
        }

        public int ObterIndexDaHoraAleatoriaDoDia(int duracaoTemposLetivos)
        {
            ValidarHorarioInicializacao();
            return Random.Shared.Next(_horarios!.Count - (duracaoTemposLetivos - 1));
        }

        /// <summary>
        /// Valida se os horários foram inicializados, lançando uma exceção caso contrário. Este método é utilizado para garantir que os dados dos horários estejam disponíveis antes de acessar seus métodos, evitando erros de referência nula e garantindo a integridade dos dados durante a execução do programa.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        public void ValidarHorarioInicializacao()
        {
            if (_horarios is null)
            {
                throw new NullReferenceException("Horários não foram inicializados");
            }
        }
        public List<FlatHorario> FlatHorario(List<HorarioGene> horario) =>
            horario
                .SelectMany(gene => gene.Aulas, (gene, aula) => new FlatHorario(
                    gene.Disciplina,
                    gene.Professor,
                    gene.Turma,
                    aula.DiaDaAula,
                    aula.HoraInicioDaAula,
                    aula.DuracaoTemposLetivos,
                    aula.SalaDeAula,
                    gene.EstaEmColisao
                ))
                .OrderBy(aula => aula.HoraInicioDaAula).ToList();


    }
}
