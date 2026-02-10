using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Estrutura
{
    public record HorarioCSV
    {
        public string Hora { get; set; } = "";
        public List<int> Dias { get; set; } = [];
    }
}
