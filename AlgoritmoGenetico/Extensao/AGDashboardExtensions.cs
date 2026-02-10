
using AlgoritmoGenetico.Individuo;
using Spectre.Console;

namespace AlgoritmoGenetico.Extensao
{
    /// <summary>
    /// Fornece métodos de extensão para gerar representações visuais (dashboards) do estado do Algoritmo Genético.
    /// Utiliza componentes do Spectre.Console para renderizar métricas em tempo real no terminal.
    /// </summary>
    public static class AGDashboardExtensions
    {
        /// <summary>
        /// Gera um gráfico de barras (histograma) que representa a distribuição de fitness na população atual.
        /// As cores mudam dinamicamente (Verde a Vermelho) conforme a proximidade do fitness ideal.
        /// </summary>
        public static BarChart HistogramaFitnessInfo<TCromossoma>(this AG<TCromossoma> ag)
        where TCromossoma : ICromossoma<IGene>
        {
            (int intervalo, int quantidade)[] histograma = ag.Populacao.GerarHistograma();
            var distribuicaoFitness = histograma.Select(x => x.quantidade).ToList();
            int minFitness = ag.Populacao.Min(x => x.Fitness);
            int maxFitness = ag.Populacao.Max(x => x.Fitness);

            var chartTable = new BarChart();
            double intervalo = (maxFitness - minFitness) / (double)distribuicaoFitness.Count;

            for (int indexIntervalo = 0; indexIntervalo < distribuicaoFitness.Count; indexIntervalo++)
            {
                // Cores mudam conforme a proximidade do alvo (0)
                var cor = indexIntervalo switch
                {
                    0 => Color.Green,
                    < 4 => Color.Yellow,
                    < 8 => Color.Orange1,
                    _ => Color.Red
                };
                chartTable.AddItem($"F{indexIntervalo,2}", distribuicaoFitness[indexIntervalo], cor);
            }
            return chartTable;
        }

        /// <summary>
        /// Cria uma tabela resumo com as principais métricas de performance e estado do motor, 
        /// incluindo gerações por segundo, tempo decorrido e progresso do fitness.
        /// </summary>
        public static Table PerformanceInfo<TCromossoma>(this AG<TCromossoma> ag)
         where TCromossoma : ICromossoma<IGene>
        {
            int minFitness = ag.Populacao.Min(x => x.Fitness);
            int maxFitness = ag.Populacao.Max(x => x.Fitness);

            var singleTable = new Table().Expand().Border(TableBorder.Rounded).HideHeaders();
            singleTable.AddColumn(new TableColumn("[cyan]Métrica[/]").LeftAligned());
            singleTable.AddColumn(new TableColumn("").RightAligned());
            singleTable.AddRow("[yellow]População(Genes)[/]", $"{ag.Populacao.Count,5} ({ag.SolucaoCandidata.Genes.Count()})");
            singleTable.AddRow("[yellow]Geração[/]", $"#{ag.GeracaoAtual,5} / {ag.Configuracao.LimiteMáximoDeGeracoesPermitidas}");
            singleTable.AddRow("[yellow]Evolução[/]", ag.Configuracao.ProcessoDeEvolucao.ToString());
            singleTable.AddRow("[yellow]Performance[/]", $"CUR: {ag.PerformanceGeracoesPorSegundo,3}/s ACUM: {1000 * ag.GeracaoAtual / ag.Relogio.ElapsedMilliseconds,5}/s");
            singleTable.AddRow("[yellow]Tempo Decorrido[/]", $"{TimeSpan.FromMilliseconds(ag.Relogio.ElapsedMilliseconds):hh\\:mm\\:ss}");
            singleTable.AddRow($"[yellow]Fitness GOAL:[/] {ag.Configuracao.FitnessPretendido}", $" BEST:{ag.SolucaoCandidata.Fitness,5} [[{minFitness,5}, {maxFitness,5}]]");
            singleTable.AddRow("[yellow]Gerações Sem Evolução[/]", $"{ag.QuantidadeGeracoesSemEvolucao,5} gerações");
            singleTable.AddRow("[yellow]Repor Solução Candidata[/]", $"{ag.Configuracao.ReporSolucaoCandidataNaPopulacaoACadaGeracao,5} gerações");

            return singleTable;
        }
        /// <summary>
        /// Cria uma tabela resumo com top 10 de melhores Fitness
        /// </summary>
        public static Table TOP10FitnessInfo<TCromossoma>(this AG<TCromossoma> ag)
        where TCromossoma : ICromossoma<IGene>
        {

            var top10Fitness = ag.Populacao.Select(x => x.Fitness).Take(10).ToList();

            var top10Table = new Table().Expand().Border(TableBorder.Rounded);
            top10Table.AddColumn("[cyan]TOP[/]");
            top10Fitness.ForEach(item => top10Table.AddColumn($"{item,5}"));

            return top10Table;
        }
        /// <summary>
        /// Cria uma tabela resumo indicação dos processos de seleção, recombinação e mutação
        /// </summary>
        public static Table MetodosEvolucaoInfo<TCromossoma>(this AG<TCromossoma> ag)
        where TCromossoma : ICromossoma<IGene>
        {
            var topTable = new Table().Expand().Border(TableBorder.Rounded).HideHeaders();
            topTable.AddColumn(new TableColumn("[cyan]Processo[/]").LeftAligned());
            topTable.AddColumn(new TableColumn("[cyan]Descrição[/]").LeftAligned());
            topTable.AddRow("[yellow]Seleção[/]", ag.Configuracao.ProcessoDeSelecaoDaProximaGeracao.ToString());
            topTable.AddRow("[yellow]Recombinação[/]", ag.Configuracao.ProcessoDeRecombinacao.ToString());
            topTable.AddRow("[yellow]Mutação[/]", ag.Configuracao.ProcessoDeMutacao.ToString());
            return topTable;
        }

        /// <summary>
        /// Gera uma tabela com as métricas de utilização de memória RAM e tempos de pausa do Garbage Collector.
        /// Crucial para monitorizar a saúde da aplicação em execuções longas (ex: Raspberry Pi).
        /// </summary>
        public static Table GCMemoriaInfo<TCromossoma>(this AG<TCromossoma> ag)
        where TCromossoma : ICromossoma<IGene>
        {
            var gcTableInfo = new Table().Expand().Border(TableBorder.Rounded);
            gcTableInfo.AddColumn("[cyan]RAM[/]", col => col.Alignment = Justify.Right);
            gcTableInfo.AddColumn("[cyan]HEAP[/]", col => col.Alignment = Justify.Right);
            gcTableInfo.AddColumn("[cyan]Pausa[/]", col => col.Alignment = Justify.Right);
            gcTableInfo.AddRow(
                        $"{GC.GetTotalMemory(false) / 1024 / 1024,5} Mib",
                        $"{GC.GetGCMemoryInfo().HeapSizeBytes / 1024 / 1024,5} Mib",
                        $"{GC.GetTotalPauseDuration():hh\\:mm\\:ss} ({GC.GetGCMemoryInfo().PauseTimePercentage,5}%)"
                    );
            return gcTableInfo;
        }

        /// <summary>
        /// Cria um gráfico discriminado das recolhas do Garbage Collector por geração (Gen 0, 1 e 2).
        /// </summary>
        public static BreakdownChart GCRecolhasInfo<TCromossoma>(this AG<TCromossoma> ag)
        where TCromossoma : ICromossoma<IGene>
        {

            int gen0 = GC.CollectionCount(0);
            int gen1 = GC.CollectionCount(1);
            int gen2 = GC.CollectionCount(2);

            var gcGenInfo = new BreakdownChart()
            .AddItem("GEN0", gen0, Color.Green)
            .AddItem("GEN1", gen1, Color.Yellow)
            .AddItem("GEN2", gen2, Color.Red);
            return gcGenInfo;
        }

        /// <summary>
        /// Cria um tabela representativa da distribuição do fitness em classes
        /// </summary>
        public static (int intervalo, int quantidade)[] GerarHistograma<TCromossoma>(this IList<TCromossoma> populacao)
        where TCromossoma : ICromossoma<IGene>
        {
            int dimensaoPopulacao = populacao.Count;
            if (dimensaoPopulacao == 0)
            {
                return [];
            }

            double minFitness = populacao.Min(x => x.Fitness);
            double maxFitness = populacao.Max(x => x.Fitness);

            // Métodos para calcular a quantidade de classes
            int numeroClassesRaizQuadradaDimensao = (int)Math.Sqrt(dimensaoPopulacao);
            int numeroClasses = (int)Math.Clamp(numeroClassesRaizQuadradaDimensao, 1, 15.0);
            (int intervalo, int quantidade)[] histograma = new (int, int)[numeroClasses];

            // Evitar divisão por zero se todos forem iguais, fica tudo na primeira classe
            if (minFitness == maxFitness)
            {
                histograma[0] = (intervalo: (int)minFitness, quantidade: dimensaoPopulacao);
                return histograma;
            }

            double intervalo = (maxFitness - minFitness) / numeroClasses;

            populacao.Select(x => x.Fitness).ToList().ForEach(fitness =>
            {
                int index = Math.Clamp((int)((fitness - minFitness) / intervalo), 0, numeroClasses - 1);
                histograma[index].intervalo = (int)(minFitness + (index + 1) * intervalo);
                histograma[index].quantidade++;
            });

            return histograma;
        }

    }

}
