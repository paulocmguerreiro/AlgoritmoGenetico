using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HorarioEscolar.Estrutura;
using HorarioEscolar.Individuo;

namespace HorarioEscolar.Helper
{
    public abstract class HorarioBaseService : IHorarioService
    {
        protected Dictionary<string, HorarioCSV>? _horarios;
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
            HorarioCSV horaAConsultar = _horarios![hora];
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

        public void ValidarHorarioInicializacao()
        {
            if (_horarios is null)
            {
                throw new NullReferenceException("Horários não foram inicializados");
            }
        }
        public List<FlatHorarioDTO> FlatHorario(List<HorarioGene> horario) =>
            horario
                .SelectMany(gene => gene.Aulas, (gene, aula) => new FlatHorarioDTO(
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
