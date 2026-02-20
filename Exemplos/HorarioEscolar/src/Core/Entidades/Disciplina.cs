using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Core.Entidades
{
    public record Disciplina
    {
        public string Sigla { get; set; } = "";
        public List<int> TemposLetivos { get; set; } = [];

        public List<string> Salas = [];

    }
}
