using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Core.Entidades
{
    public record DisciplinaSalas
    {
        public string Sigla { get; set; } = "";
        public List<string> Salas { get; set; } = [];

    }
}
