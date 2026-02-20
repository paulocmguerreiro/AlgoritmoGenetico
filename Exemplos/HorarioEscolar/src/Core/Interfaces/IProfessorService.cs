
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Entidades;

namespace HorarioEscolar.Core.Interfaces
{
    /// <summary>
    /// Interface para o serviço de professores, responsável por fornecer métodos relacionados aos professores, como carregar dados, obter indicações de horários bloqueados e obter o horário de um professor específico.
    /// </summary>
    public interface IProfessorService
    {
        /// <summary>
        /// Obtém uma lista de professores, onde cada professor é representado por um objeto da classe Professor. 
        /// </summary>
        /// <returns></returns>
        List<Professor>? ToList();

        /// <summary>
        /// Carrega os dados dos professores, como horários bloqueados, a partir de uma fonte de dados.
        /// </summary>
        void CarregarDados();
        /// <summary>
        /// Obtém a indicação do horário bloqueado para uma determinada combinação de professor, dia da semana e hora, retornando um valor inteiro que representa o índice do horário bloqueado na estrutura de dados interna. Se não houver um horário bloqueado para a combinação fornecida, o método pode retornar um valor específico, como 0, para indicar que não há bloqueio.
        /// </summary>
        /// <param name="prof"></param>
        /// <param name="diaDaSemana"></param>
        /// <param name="hora"></param>
        /// <returns></returns>
        int ObterProfIndicacaoDeTempoBloqueado(string? prof, int diaDaSemana, string hora);

        /// <summary>
        /// Obtém o horário de um professor específico, identificado por seu nome ou sigla, a partir de uma lista de objetos HorarioGene, que representam os horários alocados para as aulas. O método retorna uma lista de objetos HorarioGene que correspondem ao horário do professor especificado, facilitando a visualização e manipulação dos dados relacionados ao horário do professor.
        /// </summary>
        /// <param name="genes"></param>
        /// <param name="prof"></param>
        /// <returns></returns>
        List<HorarioGene> ObterHorarioProf(HorarioCromossoma genes, string prof);
    }
}
