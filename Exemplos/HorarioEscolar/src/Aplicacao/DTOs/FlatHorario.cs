using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Applicacao.DTO
{
    /// <summary>
    /// Registro que representa um horário escolar em formato "flat", contendo informações essenciais como disciplina, professor, turma, dia da aula, hora de início e duração. 
    /// </summary>
    /// <param name="Disciplina"></param>
    /// <param name="Professor"></param>
    /// <param name="Turma"></param>
    /// <param name="DiaDaAula"></param>
    /// <param name="HoraInicioDaAula"></param>
    /// <param name="DuracaoTemposLetivos"></param>
    /// <param name="SalaDeAula"></param>
    /// <param name="EstaEmColisao"></param>
    public record FlatHorario(
        string Disciplina,
        string Professor,
        string Turma,
        int DiaDaAula,
        string HoraInicioDaAula,
        int DuracaoTemposLetivos,
        string SalaDeAula,
        bool EstaEmColisao
    );


}
