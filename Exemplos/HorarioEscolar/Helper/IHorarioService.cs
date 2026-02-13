using System.Collections.ObjectModel;
using HorarioEscolar.Individuo;
using HorarioEscolar.Individuo.Extension;

namespace HorarioEscolar.Helper
{
    public interface IHorarioService
    {

        ReadOnlyCollection<string> DiasDaSemanaDescritivo { get; }
        ReadOnlyCollection<int> DiasDaSemanaIndex { get; }
        void CarregarDados();

        ReadOnlyCollection<string> ObterHorasDoDia();

        int ObterHorarioIndicacaoDeTempoBloqueado(int diaDaSemana, string hora);

        int ObterIndexDaHoraAleatoriaDoDia(int duracaoTemposLetivos);

        string ObterHoraAPartirDoIndex(int horaInicioDaAulaIdx);

        string ObterHoraAleatoriaDoDia(int duracaoTemposLetivos);

        string ObterHoraSeguinte(string horaInicial);


        string ObterHoraAnterior(string horaInicial);

        int ObterIndexAPartirDaHora(string hora);

        List<FlatHorarioDTO> FlatHorario(List<HorarioGene> horario);
    }
}
