using AlgoritmoGenetico;
using Spectre.Console;
using AlgoritmoGenetico.Extensao;
using HorarioEscolar.Individuo;
using HorarioEscolar.Factories;
using HorarioEscolar.Individuo.Extension;

namespace HorarioEscolar.Helper
{
    /// <summary>
    /// Classe auxiliar responsável pela renderização visual do estado do Algoritmo Genético.
    /// Utiliza layouts dinâmicos para apresentar métricas de performance, saúde de memória e 
    /// pré-visualização da melhor solução atual.
    /// </summary>
    public static class DashboardHelper
    {
        /// <summary>
        /// Renderiza um Dashboard completo no terminal. 
        /// Divide o ecrã em secções para métodos de evolução, performance e visualização de horários.
        /// </summary>
        /// <typeparam name="TCromossoma">O tipo de cromossoma, restrito a <see cref="HorarioCromossoma"/>.</typeparam>
        /// <param name="ag">Instância do motor de Algoritmo Genético em execução.</param>
        public static void MostrarInformacao<TCromossoma>(AG<TCromossoma> ag)
        where TCromossoma : HorarioCromossoma
        {

            var preVisualizacaoGrid = new Grid().Expand();
            preVisualizacaoGrid.AddColumn();
            //preVisualizacaoGrid.AddRow(ag.TOP10FitnessInfo());
            preVisualizacaoGrid.AddRow(ag.SolucaoCandidata.ObterHorarioTurma(TurmaHelper.AsList().ElementAt((int)(ag.Relogio.ElapsedMilliseconds / 3000) % TurmaHelper.AsList().Count).Sigla).ImprimirHorarioTurma());
            preVisualizacaoGrid.AddRow(ag.SolucaoCandidata.ObterHorarioProf(ProfessorHelper.ToList()!.ElementAt((int)(ag.Relogio.ElapsedMilliseconds / 3000) % ProfessorHelper.ToList()!.Count).Sigla).ImprimirHorarioProf());
            preVisualizacaoGrid.AddRow(ag.SolucaoCandidata.ConflitosTurmaInfo());
            //preVisualizacaoGrid.AddRow(ag.SolucaoCandidata.ConflitosProfInfo());

            var performanceGrid = new Grid().Expand();
            performanceGrid.AddColumn();
            performanceGrid.AddRow(ag.PerformanceInfo());
            performanceGrid.AddRow(new Panel(ag.HistogramaFitnessInfo()).Header("> [yellow]Distribuição de Fitness[/] <"));
            performanceGrid.AddRow(ag.GCMemoriaInfo());
            performanceGrid.AddRow(new Panel(ag.GCRecolhasInfo()));
            performanceGrid.AddRow(DashboardHelper.CacheInfo());

            var rootLayout = new Layout();
            rootLayout.SplitRows(
                new Layout().Update(ag.MetodosEvolucaoInfo()).Size(5),
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
        /// <summary>
        /// Gera uma tabela detalhada com as métricas de eficiência das Fábricas (Pooling).
        /// Calcula a percentagem de Cache Hits para demonstrar a eficácia da reutilização de objetos.
        /// </summary>
        /// <returns>Uma instância de <see cref="Table"/> formatada para o Spectre.Console.</returns>
        public static Table CacheInfo()
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
    }


}
