using System.Buffers;
using System.Diagnostics;
using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;
using AlgoritmoGenetico.Extensao;
using Spectre.Console;

namespace AlgoritmoGenetico
{
    /// <summary>
    /// Classe abstrata fundamental que orquestra o ciclo de vida do Algoritmo Genético.
    /// Responsável pela evolução da população através de seleção, recombinação e mutação.
    /// </summary>
    /// <typeparam name="TCromossoma">O tipo de cromossoma a evoluir, respeitando ICromossoma.</typeparam>
    public abstract class AG<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        /// <summary> Cronómetro para controlo do tempo total de execução. </summary>
        public Stopwatch Relogio
        {
            get;
            private set;
        } = new Stopwatch();

        private long _performanceDuracaoEmMilisegundos = 0;

        public int PerformanceTotalDeGeracoes
        {
            get;
            private set;
        } = 0;

        /// <summary> Calcula a velocidade atual do motor (gerações processadas por segundo). </summary>
        public int PerformanceGeracoesPorSegundo
        {
            get;
            private set;
        } = 0;

        /// <summary> Contabiliza quantas gerações passaram sem que o Fitness da Solução Candidata melhorasse. </summary>
        public int QuantidadeGeracoesSemEvolucao
        {
            get;
            private set;
        } = 0;

        private long _feedbackVisualizadoNoSegundo = 0;


        public int GeracaoAtual { get; protected set; }
        public float SolucaoCandidataFitness { get => SolucaoCandidata?.Fitness ?? 0; }
        /// <summary> A melhor solução (indivíduo) encontrada até ao momento. </summary>
        public TCromossoma? SolucaoCandidata { get; protected set; }

        /// <summary> A população de indivíduos da geração corrente. </summary>
        public List<TCromossoma> Populacao { get; protected set; } = new List<TCromossoma>();

        /// <summary> Objeto de configuração com os parâmetros e delegados do AG. </summary>
        public required AGConfiguracao<TCromossoma> Configuracao { get; init; }

        /// <summary>
        /// Inicia o processo de procura pela solução ideal. 
        /// Corre em loop até atingir o Fitness pretendido ou o limite de gerações.
        /// </summary>
        public void ProcurarSolucao()
        {
            Relogio.Start();
            Configuracao.OutputService.OnEvolucaoIniciada();
            InicializarAlgoritmo();
            RecalcularFitness(Populacao);

            while (!EncontrouSolucaoAdequada() && GeracaoAtual < Configuracao.LimiteMáximoDeGeracoesPermitidas)
            {
                // Gerar Filhos - Ponto de partida
                List<TCromossoma> populacaoFilhos = CriarPopulacaoDeFilhos();
                RecalcularFitness(populacaoFilhos);

                // Mutar Filhos
                Configuracao.ProcessoDeSelecaoDaProximaGeracao.Preparar(populacaoFilhos, Configuracao);
                Configuracao.ProcessoDeMutacao.Mutar(populacaoFilhos, QuantidadeGeracoesSemEvolucao);
                RecalcularFitness(populacaoFilhos);

                ReintroduzirSolucaoCandidata();

                // Geração dos Pais
                //RecalcularFitness(Populacao);
                Configuracao.ProcessoDeSelecaoDaProximaGeracao.Preparar(Populacao, Configuracao);
                Populacao = Configuracao.ProcessoDeSelecaoDaProximaGeracao
                    .EscolherPopulacao()
                    .Take((int)(Configuracao.ProbabilidadeDeSelecionarDaGeracaoPais * Configuracao.DimensaoDaPopulacao))
                    .ToList();

                // Geração dos Filhos
                Configuracao.ProcessoDeSelecaoDaProximaGeracao.Preparar(populacaoFilhos, Configuracao);
                Populacao.AddRange(
                    Configuracao.ProcessoDeSelecaoDaProximaGeracao
                        .EscolherPopulacao()
                        .Take((int)(Configuracao.ProbabilidadeDeSelecionarDaGeracaoFilhos * Configuracao.DimensaoDaPopulacao))
                        .ToList()
                    );

                // População Final (PAIS + FILHOS) 
                Configuracao.ProcessoDeSelecaoDaProximaGeracao.Preparar(Populacao, Configuracao);
                Populacao = Configuracao.ProcessoDeSelecaoDaProximaGeracao.EscolherPopulacao();

                PreencherAleatoriamentePopulacaoEmFalta();
                Populacao.OrderByFitness(Configuracao.ProcessoDeEvolucao);

                AtualizarSolucaoCandidata();

                FornecerFeedback();
                GeracaoAtual++;
            }

            // Garantir que mostra efetivamente o estado final (pode ter terminado dentro do período de feedback)
            Configuracao.OutputService.OnEvolucaoTerminada();
        }

        /// <summary>
        /// Garantir que a Solução Candidata é reinserida na população periodicamente a cada determinada quantidade de gerações
        /// </summary>
        private void ReintroduzirSolucaoCandidata()
        {
            if (Configuracao.ReporSolucaoCandidataNaPopulacaoACadaGeracao > 0 && GeracaoAtual % Configuracao.ReporSolucaoCandidataNaPopulacaoACadaGeracao == 0)
            {
                Populacao.Add((TCromossoma)(SolucaoCandidata ?? Configuracao.CromossomaFactory.CriarAleatorio()).Clone());
            }
        }

        /// <summary>
        /// Cria uma nova geração de filhos através da seleção de pais e aplicação de crossover.
        /// </summary>
        private List<TCromossoma> CriarPopulacaoDeFilhos()
        {
            int totalFilhosNecessarios = Configuracao.DimensaoDaPopulacao;
            TCromossoma[] populacaoFilhos = new TCromossoma[totalFilhosNecessarios];

            Configuracao.ProcessoDeSelecaoDaProximaGeracao.Preparar(Populacao, Configuracao);
            for (int i = 0; i < totalFilhosNecessarios; i += 2)
            {
                List<TCromossoma> filhos = Configuracao.ProcessoDeRecombinacao.Combinar(
                        Configuracao.ProcessoDeSelecaoDaProximaGeracao.EscolherIndividuo(),
                        Configuracao.ProcessoDeSelecaoDaProximaGeracao.EscolherIndividuo()
                    );
                populacaoFilhos[i] = filhos[0]; ;
                populacaoFilhos[i + 1] = filhos[1];
            }
            return populacaoFilhos.ToList();
        }

        /// <summary>
        /// Avalia se a melhor solução atual satisfaz os critérios de evolução (Minimização ou Maximização).
        /// </summary>
        protected bool EncontrouSolucaoAdequada() => Configuracao.ProcessoDeEvolucao switch
        {
            AGProcessoDeEvolucao.MINIMIZACAO => SolucaoCandidata!.Fitness <= Configuracao.FitnessPretendido,
            AGProcessoDeEvolucao.MAXIMIZACAO => SolucaoCandidata!.Fitness >= Configuracao.FitnessPretendido,
            _ when GeracaoAtual >= Configuracao.LimiteMáximoDeGeracoesPermitidas => true,
            _ => false
        };

        /// <summary>
        /// Verifica se algum indivíduo da nova população supera a Solução Candidata atual e atualiza-a se necessário.
        /// </summary>
        protected TCromossoma? AtualizarSolucaoCandidata()
        {
            TCromossoma melhorSolucaoPopulacao = Populacao[0];
            TCromossoma solucaoRetornar = (Configuracao.ProcessoDeEvolucao, SolucaoCandidata!.Fitness, melhorSolucaoPopulacao.Fitness) switch
            {
                (AGProcessoDeEvolucao.MINIMIZACAO, int sC, int mS) when sC < mS => SolucaoCandidata,
                (AGProcessoDeEvolucao.MAXIMIZACAO, int sC, int mS) when sC > mS => SolucaoCandidata,
                _ => (TCromossoma)melhorSolucaoPopulacao.Clone()
            };

            // Contabilizar a quantidade de gerações sem evolução
            if (solucaoRetornar.Fitness == SolucaoCandidata.Fitness)
            {
                QuantidadeGeracoesSemEvolucao++;
            }
            else
            {
                QuantidadeGeracoesSemEvolucao = 0;
                // Informar que foi atualizado 
                Configuracao.OutputService.OnMelhorSolucaoEncontrada(SolucaoCandidata, GeracaoAtual);
            }
            SolucaoCandidata = solucaoRetornar;

            return SolucaoCandidata;
        }

        protected void InicializarAlgoritmo()
        {
            GeracaoAtual = 0;
            SolucaoCandidata = Configuracao.CromossomaFactory.CriarAleatorio();
            Configuracao.ProcessoCalculoFitness.RecalcularFitness(SolucaoCandidata);
            Configuracao.ProcessoCalculoFitness.CalcularCodigoUnico(SolucaoCandidata);

            _performanceDuracaoEmMilisegundos = Relogio.ElapsedMilliseconds;
            PerformanceTotalDeGeracoes = GeracaoAtual;
            PerformanceGeracoesPorSegundo = 0;

            QuantidadeGeracoesSemEvolucao = 0;

            InicializarPopulacao();
        }
        protected void InicializarPopulacao()
        {
            Populacao.Clear();
            Populacao.Capacity = Configuracao.DimensaoDaPopulacao;
            PreencherAleatoriamentePopulacaoEmFalta();
        }

        /// <summary>
        /// Garante que a população mantém o tamanho definido em AGConfiguracao, preenchendo faltas com novos indivíduos aleatórios.
        /// </summary>
        protected void PreencherAleatoriamentePopulacaoEmFalta()
        {
            if (Populacao is not List<TCromossoma>)
            {
                return;
            }
            if (Populacao.Count >= Configuracao.DimensaoDaPopulacao)
            {
                return;
            }

            Populacao.Capacity = Configuracao.DimensaoDaPopulacao;
            int dimensaoAtual = Populacao.Count;
            while (dimensaoAtual++ < Configuracao.DimensaoDaPopulacao)
            {
                Populacao.Add(Configuracao.CromossomaFactory.CriarAleatorio());
            }
            RecalcularFitness(Populacao);
        }

        /// <summary>
        /// Invoca o callback de feedback (UI/Consola) respeitando os intervalos de tempo configurados.
        /// </summary>
        /// <param name="forcarValidacaoIntervalo">Se falso, ignora o tempo e força a exibição (útil no final).</param>
        public void FornecerFeedback(bool forcarValidacaoIntervalo = true)
        {
            // Cores ANSI
            long segundosElapsed = Relogio.ElapsedMilliseconds / 1000 + 1;

            if (Relogio.ElapsedMilliseconds - _performanceDuracaoEmMilisegundos > 1000)
            {
                PerformanceGeracoesPorSegundo = GeracaoAtual - PerformanceTotalDeGeracoes;
                _performanceDuracaoEmMilisegundos = Relogio.ElapsedMilliseconds;
                PerformanceTotalDeGeracoes = GeracaoAtual;
            }

            // Confirmar que já pode dar feedback
            if (forcarValidacaoIntervalo)
            {
                if (Configuracao.DarFeedbackACadaSegundo <= 0)
                    return;
                if (segundosElapsed == _feedbackVisualizadoNoSegundo)
                    return;
                if (segundosElapsed % Configuracao.DarFeedbackACadaSegundo != 0)
                    return;
            }
            _feedbackVisualizadoNoSegundo = segundosElapsed;

            Configuracao.OutputService.OnGeracaoProcessada(this);

        }

        /// <summary>
        /// Calcula o fitness de todos os indivíduos em paralelo, utilizando a capacidade multi-core do CPU.
        /// Deixa dois núcleos livres (Environment.ProcessorCount - 2) para manter a fluidez do sistema.
        /// </summary>
        private void RecalcularFitness(List<TCromossoma> populacao)
        {
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount - 2
            };
            Parallel.For(0, populacao.Count, options, filhos =>
            {
                populacao[filhos].Fitness = Configuracao.ProcessoCalculoFitness.RecalcularFitness(populacao[filhos]);
                populacao[filhos].CodigoUnico = Configuracao.ProcessoCalculoFitness.CalcularCodigoUnico(populacao[filhos]);
            });
        }
    }
}
