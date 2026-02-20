using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Core.Entidades
{
    public record Horario
    {
        public string Hora { get; set; } = "";
        public List<int> Dias { get; set; } = [];
    }
}
