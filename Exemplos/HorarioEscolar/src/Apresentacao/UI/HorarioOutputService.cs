
using AlgoritmoGenetico.Aplicacao.Motor;
using AlgoritmoGenetico.Infraestrutura.Output;
using HorarioEscolar.Applicacao.DTO;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Cache;
using HorarioEscolar.Core.Interfaces;
using Spectre.Console;

namespace HorarioEscolar.Apresentacao.UI
{
    /// <summary>
    /// Implementação personalizada do serviço de output para o algoritmo genético de horário escolar, utilizando a biblioteca Spectre.Console para exibir informações detalhadas sobre a evolução do algoritmo, incluindo pré-visualizações dos horários gerados, métricas de desempenho e informações sobre conflitos. Esta classe é responsável por formatar e apresentar os dados de forma clara e visualmente atraente durante a execução do algoritmo, facilitando a análise e compreensão dos resultados.
    /// </summary>
    public class HorarioOutputService(IProfessorService professorService, IHorarioService horarioService, ITurmaService turmaService) : AGOutputSpectreService<HorarioCromossoma>
    {
        HorarioCromossoma? SolucaoCandidata;
        public override void OnEvolucaoIniciada()
        {
        }

        public override void OnEvolucaoTerminada()
        {
        }

        public override void OnGeracaoProcessada(AG<HorarioCromossoma> algoritmo)
        {
            SolucaoCandidata ??= algoritmo.SolucaoCandidata;
            if (SolucaoCandidata is null) return;

            var preVisualizacaoGrid = new Grid().Expand();
            preVisualizacaoGrid.AddColumn();
            //preVisualizacaoGrid.AddRow(TOP10FitnessInfo(algoritmo));
            preVisualizacaoGrid.AddRow(ImprimirHorarioTurma(turmaService.ObterHorarioTurma(SolucaoCandidata, turmaService.AsList().ElementAt((int)(algoritmo.Relogio.ElapsedMilliseconds / 3000) % turmaService.AsList().Count).Sigla)));
            preVisualizacaoGrid.AddRow(ImprimirHorarioProf(professorService.ObterHorarioProf(SolucaoCandidata, professorService.ToList()!.ElementAt((int)(algoritmo.Relogio.ElapsedMilliseconds / 3000) % professorService.ToList()!.Count).Sigla)));
            preVisualizacaoGrid.AddRow(ConflitosTurmaInfo(SolucaoCandidata));
            //preVisualizacaoGrid.AddRow(ConflitosProfInfo(SolucaoCandidata));

            var performanceGrid = new Grid().Expand();
            performanceGrid.AddColumn();
            performanceGrid.AddRow(PerformanceInfo(algoritmo));
            performanceGrid.AddRow(new Panel(HistogramaFitnessInfo(algoritmo)).Header("> [yellow]Distribuição de Fitness[/] <"));
            performanceGrid.AddRow(GCMemoriaInfo());
            performanceGrid.AddRow(new Panel(GCRecolhasInfo()));
            performanceGrid.AddRow(CacheInfo());

            var rootLayout = new Layout();
            rootLayout.SplitRows(
                    new Layout().Update(MetodosEvolucaoInfo(algoritmo)).Size(5),
                    new Layout("body")
                );
            rootLayout["body"].SplitColumns(
                new Layout("esquerda").Ratio(1),
                new Layout("direita").Ratio(2)
            );

            rootLayout["body"]["esquerda"].Update(new Layout().Update(performanceGrid));
            rootLayout["body"]["direita"].SplitRows(new Layout().Update(new Panel(preVisualizacaoGrid).Expand().Header("> [yellow]Pré-Visualização de Horário (execução do Algoritmo)[/] <")));

            Console.Clear();
            AnsiConsole.Write(rootLayout);
        }

        public override void OnMelhorSolucaoEncontrada(HorarioCromossoma melhorIndividuo, int geracao)
        {
            SolucaoCandidata = melhorIndividuo;
        }


        /// <summary>
        /// Gera uma tabela detalhada com as métricas de eficiência das Fábricas (Pooling).
        /// Calcula a percentagem de Cache Hits para demonstrar a eficácia da reutilização de objetos.
        /// </summary>
        /// <returns>Uma instância de <see cref="Table"/> formatada para o Spectre.Console.</returns>
        public Table CacheInfo()
        {
            var cacheTable = new Table().Expand().Border(TableBorder.Rounded);
            cacheTable.AddColumn(new TableColumn("[yellow]Cache[/]").LeftAligned());
            cacheTable.AddColumn(new TableColumn("[yellow]Dimensão[/]").RightAligned());
            //cacheTable.AddColumn(new TableColumn("[yellow]Chamadas[/]").RightAligned());
            cacheTable.AddColumn(new TableColumn("[yellow]CacheHits[/]").RightAligned());
            cacheTable.AddRow(
                "HorarioDiasGene",
                HorarioDiasGeneFactory.AulaPoolSize.ToString(),
                $"{100.0 * (HorarioDiasGeneFactory.ChamadasDias - HorarioDiasGeneFactory.AulaPoolSize) / HorarioDiasGeneFactory.ChamadasDias:F2}%"
                );
            cacheTable.AddRow(
                "HorarioDiasGene (Listas)",
                HorarioDiasGeneFactory.ListPoolSize.ToString(),
                $"{100.0 * (HorarioDiasGeneFactory.ChamadasLista - HorarioDiasGeneFactory.ListPoolSize) / HorarioDiasGeneFactory.ChamadasLista:F2}%"
                );
            cacheTable.AddRow(
                "HorarioGeneFactory",
                HorarioGeneFactory.PoolSize.ToString(),
                $"{100 * (HorarioGeneFactory.Chamadas - HorarioGeneFactory.PoolSize) / HorarioGeneFactory.Chamadas:F2}%"
                );

            return cacheTable;
        }

        /// <summary>
        /// Gera um painel do horário de um professor específico, destacando visualmente os conflitos e bloqueios de tempo, utilizando cores para indicar a natureza dos conflitos (vermelho para colisões, verde para horários livres, amarelo para bloqueios) e exibindo informações relevantes como disciplina, sala de aula e indicações de bloqueio.
        /// </summary>
        public Panel ImprimirHorarioProf(List<HorarioGene> horario)
        {
            Table horarioTable = new Table().Expand();
            horarioTable.AddColumn("[cyan]HORA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]SEGUNDA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]TERÇA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]QUARTA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]QUINTA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]SEXTA[/]", col => col.Centered());

            List<FlatHorario> geneFlat = horarioService.FlatHorario(horario);

            foreach (string horaAProcessar in horarioService.ObterHorasDoDia())
            {
                string[] colunas = new string[6];
                colunas[0] = $"[cyan]{horaAProcessar}[/]";

                List<FlatHorario> aulasNaHora = geneFlat.Where(x => x.HoraInicioDaAula == horaAProcessar).ToList();

                for (int dia = 0; dia < 5; dia++)
                {
                    string mensagem = "                ";
                    FlatHorario? aulaDoDia = aulasNaHora.FirstOrDefault(x => x.DiaDaAula == dia);

                    bool estaBloqueadoNoProf = professorService.ObterProfIndicacaoDeTempoBloqueado(aulaDoDia?.Professor, dia, horaAProcessar) != 0;
                    bool estaBloqueadoEscola = horarioService.ObterHorarioIndicacaoDeTempoBloqueado(dia, horaAProcessar) != 0;
                    bool estaBloqueadoNaTurma = turmaService.ObterTurmaIndicacaoDeTempoBloqueado(aulaDoDia?.Turma, dia, horaAProcessar) != 0;
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


        /// <summary>
        /// Gera um painel do horário de uma turma específica, destacando visualmente os conflitos e bloqueios de tempo, utilizando cores para indicar a natureza dos conflitos (vermelho para colisões, verde para horários livres, amarelo para bloqueios) e exibindo informações relevantes como disciplina, sala de aula e indicações de bloqueio. Esta visualização é crucial para entender a distribuição das aulas ao longo da semana e identificar rapidamente os pontos problemáticos no horário da turma.
        /// </summary>
        public Panel ImprimirHorarioTurma(List<HorarioGene> horario)
        {

            Table horarioTable = new Table().Expand();
            horarioTable.AddColumn("[cyan]HORA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]SEGUNDA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]TERÇA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]QUARTA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]QUINTA[/]", col => col.Centered());
            horarioTable.AddColumn("[cyan]SEXTA[/]", col => col.Centered());

            List<FlatHorario> geneFlat = horarioService.FlatHorario(horario);

            foreach (string horaAProcessar in horarioService.ObterHorasDoDia())
            {
                string[] colunas = new string[6];
                colunas[0] = $"[cyan]{horaAProcessar}[/]";

                List<FlatHorario> aulasNaHora = geneFlat.Where(x => x.HoraInicioDaAula == horaAProcessar).ToList();

                for (int dia = 0; dia < 5; dia++)
                {
                    string mensagem = "                ";
                    FlatHorario? aulaDoDia = aulasNaHora.FirstOrDefault(x => x.DiaDaAula == dia);
                    bool estaBloqueadoNaTurma = turmaService.ObterTurmaIndicacaoDeTempoBloqueado(aulaDoDia?.Turma, dia, horaAProcessar) != 0;
                    bool estaBloqueadoEscola = horarioService.ObterHorarioIndicacaoDeTempoBloqueado(dia, horaAProcessar) != 0;
                    bool estaBloqueadoNoProf = professorService.ObterProfIndicacaoDeTempoBloqueado(aulaDoDia?.Professor, dia, horaAProcessar) != 0;
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

        /// <summary>
        /// Gera uma tabela de informações sobre os conflitos enfrentados por um professor específico, utilizando cores para indicar a presença de conflitos (vermelho para conflitos, verde para horários livres) e exibindo o status de cada professor em relação ao horário gerado. Esta visualização é essencial para identificar rapidamente quais professores estão enfrentando problemas de sobreposição ou bloqueios de tempo, permitindo uma análise mais aprofundada dos pontos críticos no horário.
        /// </summary>
        public Table ConflitosProfInfo(HorarioCromossoma genes)
        {
            Table profsTable = new Table().Expand();

            professorService.ToList()!.ForEach(professor =>
            {
                bool temConflito = professorService.ObterHorarioProf(genes, professor.Sigla).Count(x => x.EstaEmColisao) > 0;
                string corAlerta = temConflito ? "red" : "green";
                profsTable.AddColumn($"[{corAlerta}]{professor.Sigla}[/]", col => col.Centered());
            });

            return profsTable;
        }


        public Table ConflitosTurmaInfo(HorarioCromossoma genes)
        {
            Table turmaTable = new Table().Expand();

            turmaService.AsList().ForEach(turma =>
            {
                bool temConflito = turmaService.ObterHorarioTurma(genes, turma.Sigla).Count(x => x.EstaEmColisao) > 0;
                string corAlerta = temConflito ? "red" : "green";
                turmaTable.AddColumn($"[{corAlerta}]{turma.Sigla}[/]", col => col.Centered());
            });

            return turmaTable;
        }

    }
}
