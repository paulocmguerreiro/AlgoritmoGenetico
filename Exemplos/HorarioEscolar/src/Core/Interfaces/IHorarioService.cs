
using System.Collections.ObjectModel;
using HorarioEscolar.Applicacao.DTO;
using HorarioEscolar.Applicacao.GA.Modelo;

namespace HorarioEscolar.Core.Interfaces
{
    /// <summary>
    /// Interface para o serviço de horários, responsável por fornecer métodos relacionados aos horários, como carregar dados, obter horas do dia, obter horários bloqueados, obter horas aleatórias e converter horários para um formato plano.
    /// </summary>
    public interface IHorarioService
    {

        ReadOnlyCollection<string> DiasDaSemanaDescritivo { get; }
        ReadOnlyCollection<int> DiasDaSemanaIndex { get; }

        /// <summary>
        /// Carrega os dados dos horários, como horas do dia e horários bloqueados, a partir de uma fonte de dados.
        /// </summary>
        void CarregarDados();

        /// <summary>
        /// Obtém as horas do dia disponíveis para alocação de aulas, retornando uma coleção de strings representando os horários, como "08:00", "08:50", etc.
        /// </summary>
        /// <returns></returns>
        ReadOnlyCollection<string> ObterHorasDoDia();

        /// <summary>
        /// Obtém a indicação do horário bloqueado para uma determinada combinação de dia da semana e hora, retornando um valor inteiro que representa o índice do horário bloqueado na estrutura de dados interna. Se não houver um horário bloqueado para a combinação fornecida, o método pode retornar um valor específico, como 0, para indicar que não há bloqueio. 
        /// </summary>
        /// <param name="diaDaSemana"></param>
        /// <param name="hora"></param>
        /// <returns></returns>
        int ObterHorarioIndicacaoDeTempoBloqueado(int diaDaSemana, string hora);

        /// <summary>
        /// Obtém o índice de uma hora aleatória do dia, considerando a duração dos tempos letivos, para alocação de aulas. 
        /// </summary>
        /// <param name="duracaoTemposLetivos"></param>
        /// <returns></returns>
        int ObterIndexDaHoraAleatoriaDoDia(int duracaoTemposLetivos);

        /// <summary>
        /// Obtém a representação de uma hora a partir de um índice, onde o índice corresponde à posição da hora na estrutura de dados interna que armazena as horas do dia. 
        /// </summary>
        /// <param name="horaInicioDaAulaIdx"></param>
        /// <returns></returns>
        string ObterHoraAPartirDoIndex(int horaInicioDaAulaIdx);

        /// <summary>
        /// Obtém uma hora aleatória do dia, considerando a duração dos tempos letivos, para alocação de aulas. O método pode retornar uma string representando o horário, como "08:00", "08:50", etc.
        /// </summary>
        /// <param name="duracaoTemposLetivos"></param>
        /// <returns></returns>
        string ObterHoraAleatoriaDoDia(int duracaoTemposLetivos);

        /// <summary>
        /// Obtém a hora seguinte a uma hora inicial fornecida, considerando a duração dos tempos letivos. 
        /// </summary>
        /// <param name="horaInicial"></param>
        /// <returns></returns>
        string ObterHoraSeguinte(string horaInicial);


        /// <summary>
        /// Obtém a hora anterior a uma hora inicial fornecida, considerando a duração dos tempos letivos. 
        /// </summary>
        /// <param name="horaInicial"></param>
        /// <returns></returns>
        string ObterHoraAnterior(string horaInicial);

        /// <summary>
        /// Obtém o índice de uma hora a partir de sua representação em string, onde a string representa um horário específico, como "08:00", "08:50", etc., e o método retorna o índice correspondente na estrutura de dados interna que armazena as horas do dia. 
        /// </summary>
        /// <param name="hora"></param>
        /// <returns></returns>
        int ObterIndexAPartirDaHora(string hora);

        /// <summary>
        /// Converte uma lista de objetos HorarioGene, que representam os horários alocados para as aulas, em uma lista de objetos FlatHorario, que é uma representação plana dos horários, facilitando a visualização e manipulação dos dados. 
        /// </summary>
        /// <param name="horario"></param>
        /// <returns></returns>
        List<FlatHorario> FlatHorario(List<HorarioGene> horario);
    }
}
