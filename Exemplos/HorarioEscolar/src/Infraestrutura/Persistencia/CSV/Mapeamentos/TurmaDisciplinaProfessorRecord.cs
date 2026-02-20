using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HorarioEscolar.Infraestrutura.Persistencia.CSV.Mapeamentos
{
    public record TurmaDisciplinaProfessorRecord
    {
        public string Sigla { get; set; } = "";
        public string Disciplina { get; set; } = "";
        public string Professor { get; set; } = "";

    }
}
