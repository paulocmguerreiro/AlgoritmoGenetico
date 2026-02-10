using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Estrutura
{
    public record DisciplinaSalasCSV
    {
        public string Sigla { get; set; } = "";
        public List<string> Salas { get; set; } = [];

    }
}
