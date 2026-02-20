using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Core.Entidades
{
    public record Professor
    {
        public string Sigla { get; set; } = "";
        public List<int> TemposLetivos { get; set; } = [];
    }
}
