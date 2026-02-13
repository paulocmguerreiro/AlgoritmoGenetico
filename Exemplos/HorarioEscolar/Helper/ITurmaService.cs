using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlgoritmoGenetico.Individuo;
using HorarioEscolar.Estrutura;
using HorarioEscolar.Individuo;

namespace HorarioEscolar.Helper
{
    public interface ITurmaService
    {
        List<TurmaCSV> AsList();
        int ObterTurmaIndicacaoDeTempoBloqueado(string? turma, int diaDaSemana, string hora);
        List<HorarioGene> ObterHorarioTurma(HorarioCromossoma genes, string turma);
        Dictionary<string, TurmaCSV> ToDictionary();
        void CarregarDados();

    }
}
