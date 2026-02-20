
using System.Collections.Frozen;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Entidades;

namespace HorarioEscolar.Core.Interfaces
{
    /// <summary>
    /// Interface para o serviço de turmas, responsável por fornecer métodos relacionados às turmas, como carregar dados, obter indicações de horários bloqueados e obter o horário de uma turma específica.
    /// </summary>
    public interface ITurmaService
    {
        /// <summary>
        /// Obtém uma lista de turmas, onde cada turma é representada por um objeto da classe Turma. 
        /// </summary>
        List<Turma> AsList();

        /// <summary>
        /// Obtém a indicação do horário bloqueado para uma determinada combinação de turma, dia da semana e hora, retornando um valor inteiro que representa o índice do horário bloqueado na estrutura de dados interna. Se não houver um horário bloqueado para a combinação fornecida, o método pode retornar um valor específico, como 0, para indicar que não há bloqueio.
        /// </summary>
        /// <returns></returns>
        int ObterTurmaIndicacaoDeTempoBloqueado(string? turma, int diaDaSemana, string hora);

        /// <summary>
        /// Obtém o horário de uma turma específica, identificada por seu nome ou sigla, a partir de uma lista de objetos HorarioGene, que representam os horários alocados para as aulas. O método retorna uma lista de objetos HorarioGene que correspondem ao horário da turma especificada, facilitando a visualização e manipulação dos dados relacionados ao horário da turma.
        /// </summary>
        /// <param name="genes"></param>
        /// <param name="turma"></param>
        /// <returns></returns>
        List<HorarioGene> ObterHorarioTurma(HorarioCromossoma genes, string turma);

        /// <summary>
        /// Obtém um dicionário congelado (FrozenDictionary) que mapeia os nomes ou siglas das turmas para os objetos Turma correspondentes, facilitando o acesso rápido às informações das turmas com base em seus identificadores. O uso de um FrozenDictionary garante que o dicionário seja imutável e otimizado para leitura, melhorando o desempenho ao acessar os dados das turmas.
        /// </summary>
        /// <returns></returns>
        FrozenDictionary<string, Turma> ToDictionary();

        /// <summary>
        /// Carrega os dados das turmas, como horários bloqueados, a partir de uma fonte de dados.
        /// </summary>
        void CarregarDados();

    }
}
