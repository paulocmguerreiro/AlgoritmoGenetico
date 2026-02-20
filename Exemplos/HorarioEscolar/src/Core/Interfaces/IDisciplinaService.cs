using HorarioEscolar.Applicacao.GA.Modelo;

namespace HorarioEscolar.Core.Interfaces
{
    /// <summary>
    /// Interface para o serviço de disciplinas, responsável por fornecer métodos relacionados às disciplinas, como carregar dados, obter tempos letivos, salas e distribuir tempos letivos.
    /// </summary>
    public interface IDisciplinaService
    {
        /// <summary>
        /// Carrega os dados das disciplinas, como tempos letivos e salas, a partir de uma fonte de dados.
        /// </summary>
        void CarregarDados();
        /// <summary>
        /// Obtém os tempos letivos associados a uma disciplina específica, identificada por sua sigla.
        /// </summary>
        /// <param name="sigla"></param>
        /// <returns></returns>
        List<int> ObterTemposLetivos(string sigla);
        /// <summary>
        /// Obtém as salas associadas a uma disciplina específica, identificada por sua sigla.
        /// </summary> 
        List<string> ObterSalas(string sigla);
        /// <summary>
        /// Obtém uma sala aleatória associada a uma disciplina específica, identificada por sua sigla.
        /// </summary>
        /// <param name="sigla"></param>
        /// <returns></returns>
        string ObterSalaAleatoria(string sigla);
        /// <summary>
        /// Distribui os tempos letivos de uma disciplina em dias da semana, garantindo que os tempos letivos sejam alocados de forma equilibrada e respeitando as restrições de horários.
        /// </summary>
        /// <param name="disciplinaSigla"></param>
        /// <param name="temposLetivosDaDisciplina"></param>
        /// <returns></returns>
        List<HorarioDiasGene> DistribuirTemposLetivos(string disciplinaSigla, List<int> temposLetivosDaDisciplina);
    }
}
