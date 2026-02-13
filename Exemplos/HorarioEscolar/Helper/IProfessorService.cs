using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HorarioEscolar.Estrutura;
using HorarioEscolar.Individuo;

namespace HorarioEscolar.Helper
{
    public interface IProfessorService
    {
        List<ProfessorCSV>? ToList();

        void CarregarDados();
        int ObterProfIndicacaoDeTempoBloqueado(string? prof, int diaDaSemana, string hora);

        List<HorarioGene> ObterHorarioProf(HorarioCromossoma genes, string prof);
    }
}
