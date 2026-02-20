using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Infraestrutura.Persistencia.CSV.Mapeamentos
{
    public record TurmaHorarioRecord
    {
        public string Sigla { get; set; } = "";
        public List<int> TemposLetivos { get; set; } = [];

    }
}
