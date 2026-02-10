using System.Runtime.CompilerServices;
using HorarioEscolar.Helper;
using HorarioEscolar.Factories;
using Spectre.Console;

namespace HorarioEscolar.Individuo.Extension
{
    public record FlatHorarioDto(
        string Disciplina,
        string Professor,
        string Turma,
        int DiaDaAula,
        string HoraInicioDaAula,
        int DuracaoTemposLetivos,
        string SalaDeAula,
        bool EstaEmColisao
    );

    public static class HorarioExtensions
    {

        public static List<HorarioGene> ObterHorarioTurma(this HorarioCromossoma genes, string turma) =>
            genes.Genes.Where(x => x.Turma.Equals(turma)).ToList();


        public static List<HorarioGene> ObterHorarioProf(this HorarioCromossoma genes, string prof) =>
            genes.Genes.Where(x => x.Professor.Equals(prof)).ToList();

        public static List<FlatHorarioDto> FlatHorario(this List<HorarioGene> horario) =>
            horario
                .SelectMany(gene => gene.Aulas, (gene, aula) => new FlatHorarioDto(
                    gene.Disciplina,
                    gene.Professor,
                    gene.Turma,
                    aula.DiaDaAula,
                    aula.HoraInicioDaAula,
                    aula.DuracaoTemposLetivos,
                    aula.SalaDeAula,
                    gene.EstaEmColisao
                ))
                .OrderBy(aula => aula.HoraInicioDaAula).ToList();

        public static Panel ImprimirHorarioProf(this List<HorarioGene> horario)
        {
            Table horarioTable = new Table().Expand();
            horarioTable.AddColumn("[cyan]HORA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]SEGUNDA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]TERÇA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]QUARTA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]QUINTA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]SEXTA[/]", col => col.Centered());

            List<FlatHorarioDto> geneFlat = horario.FlatHorario();

            foreach (string horaAProcessar in HorarioHelper.ObterHorasDoDia())
            {
                string[] colunas = new string[6];
                colunas[0] = $"[cyan]{horaAProcessar}[/]";

                List<FlatHorarioDto> aulasNaHora = geneFlat.Where(x => x.HoraInicioDaAula == horaAProcessar).ToList();

                for (int dia = 0; dia < 5; dia++)
                {
                    string mensagem = "                ";
                    FlatHorarioDto? aulaDoDia = aulasNaHora.FirstOrDefault(x => x.DiaDaAula == dia);

                    bool estaBloqueadoNoProf = ProfessorHelper.ObterProfIndicacaoDeTempoBloqueado(aulaDoDia?.Professor, dia, horaAProcessar) != 0;
                    bool estaBloqueadoEscola = HorarioHelper.ObterHorarioIndicacaoDeTempoBloqueado(dia, horaAProcessar) != 0;
                    bool estaBloqueadoNaTurma = TurmaHelper.ObterTurmaIndicacaoDeTempoBloqueado(aulaDoDia?.Turma, dia, horaAProcessar) != 0;
                    string corDisciplina = estaBloqueadoNaTurma || estaBloqueadoNoProf || estaBloqueadoEscola || (aulaDoDia?.EstaEmColisao ?? false) ? "red" : "yellow";
                    string corSala = estaBloqueadoNaTurma || estaBloqueadoNoProf || estaBloqueadoEscola || (aulaDoDia?.EstaEmColisao ?? false) ? "red" : "green";

                    if (aulaDoDia is not null)
                    {
                        mensagem = $"[{corDisciplina}]{aulaDoDia.Turma.PadRight(4).Substring(0, 4),4}{aulaDoDia.Disciplina.PadRight(5).Substring(0, 5),5}[/] [{corSala}]{aulaDoDia.SalaDeAula.PadLeft(6).Substring(0, 6)}[/]";
                    }
                    else
                    {
                        mensagem = estaBloqueadoEscola ? "[cyan]  ** ESCOLA **  [/]" : mensagem;
                        mensagem = estaBloqueadoNaTurma ? "[cyan]   ** TURMA **  [/]" : mensagem;
                        mensagem = estaBloqueadoNoProf ? "[cyan]   ** PROF **   [/]" : mensagem;
                    }


                    colunas[dia + 1] = mensagem;
                }
                horarioTable.AddRow(colunas);

            }

            return new Panel(horarioTable.Expand()).Header($"> [green]PROF :[/] [yellow]{horario.First().Professor}[/] <");

        }
        public static Panel ImprimirHorarioTurma(this List<HorarioGene> horario)
        {

            Table horarioTable = new Table().Expand();
            horarioTable.AddColumn("[cyan]HORA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]SEGUNDA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]TERÇA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]QUARTA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]QUINTA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]SEXTA[/]", col => col.Centered());

            List<FlatHorarioDto> geneFlat = horario.FlatHorario();

            foreach (string horaAProcessar in HorarioHelper.ObterHorasDoDia())
            {
                string[] colunas = new string[6];
                colunas[0] = $"[cyan]{horaAProcessar}[/]";

                List<FlatHorarioDto> aulasNaHora = geneFlat.Where(x => x.HoraInicioDaAula == horaAProcessar).ToList();

                for (int dia = 0; dia < 5; dia++)
                {
                    string mensagem = "                ";
                    FlatHorarioDto? aulaDoDia = aulasNaHora.FirstOrDefault(x => x.DiaDaAula == dia);
                    bool estaBloqueadoNaTurma = TurmaHelper.ObterTurmaIndicacaoDeTempoBloqueado(aulaDoDia?.Turma, dia, horaAProcessar) != 0;
                    bool estaBloqueadoEscola = HorarioHelper.ObterHorarioIndicacaoDeTempoBloqueado(dia, horaAProcessar) != 0;
                    bool estaBloqueadoNoProf = ProfessorHelper.ObterProfIndicacaoDeTempoBloqueado(aulaDoDia?.Professor, dia, horaAProcessar) != 0;
                    string corDisciplina = estaBloqueadoNoProf || estaBloqueadoNaTurma || estaBloqueadoEscola || (aulaDoDia?.EstaEmColisao ?? false) ? "red" : "yellow";
                    string corSala = estaBloqueadoNoProf || estaBloqueadoNaTurma || estaBloqueadoEscola || (aulaDoDia?.EstaEmColisao ?? false) ? "red" : "green";

                    if (aulaDoDia != null)
                    {
                        mensagem = ($"[{corDisciplina}]{aulaDoDia.Disciplina.PadRight(8).Substring(0, 8),8}[/] [{corSala}]{aulaDoDia.SalaDeAula.PadLeft(6).Substring(0, 6),6}[/]");
                    }
                    else
                    {
                        mensagem = estaBloqueadoEscola ? "[cyan]  ** ESCOLA **  [/]" : mensagem;
                        mensagem = estaBloqueadoNaTurma ? "[cyan]   ** TURMA **  [/]" : mensagem;
                        mensagem = estaBloqueadoNoProf ? "[cyan]   ** PROF **   [/]" : mensagem;
                    }
                    colunas[dia + 1] = mensagem;
                }
                horarioTable.AddRow(colunas);
            }
            return new Panel(horarioTable.Expand()).Header($"> [green]TURMA :[/] [yellow]{horario.First().Turma}[/] <");

        }
        public static List<HorarioDiasGene> DistribuirTemposLetivos(string disciplinaSigla, List<int> temposLetivosDaDisciplina)
        {
            List<int> diasDisponiveis = HorarioHelper.DiasDaSemanaIndex.ToList();

            List<HorarioDiasGene> aulas = new List<HorarioDiasGene>();

            foreach (int tempoLetivo in temposLetivosDaDisciplina)
            {
                int diaDaAula = Random.Shared.Next(diasDisponiveis.Count);
                string horaInicioDaAula = HorarioHelper.ObterHoraAleatoriaDoDia(tempoLetivo);
                string salaDeAula = DisciplinaHelper.ObterSalaAleatoriaDaDisciplina(disciplinaSigla);
                int distribuirTempoLetivo = tempoLetivo;
                while (distribuirTempoLetivo >= 1)
                {
                    aulas.Add(
                       HorarioDiasGeneFactory.GetAula(
                            diasDisponiveis[diaDaAula],
                            tempoLetivo,
                            salaDeAula,
                            horaInicioDaAula
                        ));
                    horaInicioDaAula = HorarioHelper.ObterHoraSeguinte(horaInicioDaAula);
                    distribuirTempoLetivo--;
                }
                diasDisponiveis.RemoveAt(diaDaAula);
            }
            return HorarioDiasGeneFactory.GetListaCanonica(aulas);
        }

        public static Table ConflitosTurmaInfo(this HorarioCromossoma genes)
        {
            Table turmaTable = new Table().Expand();

            TurmaHelper.AsList().ForEach(turma =>
            {
                bool temConflito = genes.ObterHorarioTurma(turma.Sigla).Count(x => x.EstaEmColisao) > 0;
                string corAlerta = temConflito ? "red" : "green";
                turmaTable.AddColumn($"[{corAlerta}]{turma.Sigla}[/]", col => col.Centered());
            });

            return turmaTable;
        }
        public static Table ConflitosProfInfo(this HorarioCromossoma genes)
        {
            Table profsTable = new Table().Expand();

            ProfessorHelper.Professores!.Values.ToList().ForEach(professor =>
            {
                bool temConflito = genes.ObterHorarioProf(professor.Sigla).Count(x => x.EstaEmColisao) > 0;
                string corAlerta = temConflito ? "red" : "green";
                profsTable.AddColumn($"[{corAlerta}]{professor.Sigla}[/]", col => col.Centered());
            });

            return profsTable;
        }
    }
}
