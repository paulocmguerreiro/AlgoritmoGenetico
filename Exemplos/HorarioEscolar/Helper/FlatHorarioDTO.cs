using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Helper
{
    public record FlatHorarioDTO(
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
