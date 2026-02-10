using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Estrutura
{
    public record DisciplinaCSV
    {
        public string Sigla { get; set; } = "";
        public List<int> TemposLetivos { get; set; } = [];

        public List<string> Salas = [];

    }
}
