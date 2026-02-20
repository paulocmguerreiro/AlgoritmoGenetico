using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Core.Entidades
{
    public record Turma

    {
        public string Sigla { get; set; } = "";
        public List<string> Disciplinas { get; set; } = [];

        public List<int> TemposLetivos = [];

        public Dictionary<string, string> Professores = [];

    }
}
