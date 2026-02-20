using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Infraestrutura.Persistencia.CSV.Mapeamentos
{
    public record HorarioRecord
    {
        public string Hora { get; set; } = "";
        public List<int> Dias { get; set; } = [];
    }
}
