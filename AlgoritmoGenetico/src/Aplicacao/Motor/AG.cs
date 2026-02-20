
using System.Diagnostics;
using AlgoritmoGenetico.Aplicacao.Configuracao;
using AlgoritmoGenetico.Aplicacao.Extensao;
using AlgoritmoGenetico.Core.Abstracoes;

namespace AlgoritmoGenetico.Aplicacao.Motor
{
    /// <summary>
    /// Classe abstrata fundamental que orquestra o ciclo de vida do Algoritmo Genético.
    /// Responsável pela evolução da população através de seleção, recombinação e mutação.
    /// </summary>
    /// <typeparam name="TCromossoma">O tipo de cromossoma a evoluir, respeitando ICromossoma<IGene>.</typeparam>
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

        /// <summary>
        /// Número da geração atual, começando em 0. Incrementado a cada ciclo de evolução.
        /// </summary>
        public int GeracaoAtual { get; protected set; }
        /// <summary> A melhor solução (indivíduo) encontrada até ao momento. </summary>
        public TCromossoma? SolucaoCandidata { get; protected set; }

        /// <summary> A população de indivíduos da geração corrente, a ser introduzida como pais. </summary>
        public List<TCromossoma> Populacao { get; protected set; } = new List<TCromossoma>();

        /// <summary> Objeto de configuração com os parâmetros e serviços para o motor AG. </summary>
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
                Configuracao.ProcessoDeSelecaoDaProximaGeracao.Preparar(Populacao, Configuracao);
                Populacao = Configuracao.ProcessoDeSelecaoDaProximaGeracao
                    .EscolherPopulacao()
                    .Take((int)(Configuracao.ProbabilidadeDeSelecionarDaGeracaoPais * Configuracao.DimensaoDaPopulacao))
                    .ToList();

                // Adicionar a Geração dos Filhos
                Configuracao.ProcessoDeSelecaoDaProximaGeracao.Preparar(populacaoFilhos, Configuracao);
                Populacao.AddRange(
                 Configuracao.ProcessoDeSelecaoDaProximaGeracao
                    .EscolherPopulacao()
                    .Take((int)(Configuracao.ProbabilidadeDeSelecionarDaGeracaoFilhos * Configuracao.DimensaoDaPopulacao))
                    .ToList()
                );

                // Limitar a População Final (PAIS + FILHOS) de acordo com a configuração
                Configuracao.ProcessoDeSelecaoDaProximaGeracao.Preparar(Populacao, Configuracao);
                Populacao = Configuracao.ProcessoDeSelecaoDaProximaGeracao.EscolherPopulacao();

                // Algumas estratégias de seleção podem retornar uma população inferior ao definido, pelo que é necessário garantir que se mantém o tamanho definido em AGConfiguracao, preenchendo faltas com novos indivíduos aleatórios.
                if (PreencherAleatoriamentePopulacaoEmFalta())
                {
                    RecalcularFitness(Populacao);
                    Populacao.OrderByFitness(Configuracao.ProcessoDeEvolucao);
                }

                AtualizarSolucaoCandidata();

                FornecerFeedback();
                GeracaoAtual++;
            }

            // Garantir que mostra efetivamente o estado final (pode ter terminado dentro do período de feedback)
            FornecerFeedback(true);
            Configuracao.OutputService.OnEvolucaoTerminada();
        }

        /// <summary>
        /// Garantir que a Solução Candidata é reinserida na população periodicamente a cada determinada quantidade de gerações
        /// </summary>
        private void ReintroduzirSolucaoCandidata()
        {
            if (Configuracao.ReporSolucaoCandidataNaPopulacaoACadaGeracao > 0 && GeracaoAtual % Configuracao.ReporSolucaoCandidataNaPopulacaoACadaGeracao == 0)
            {
                Populacao.Add((TCromossoma)SolucaoCandidata!.Clone());
            }
        }

        /// <summary>
        /// Criar uma nova geração de filhos através da seleção de pais e aplicação de recombinação.
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
            TCromossoma melhorSolucaoPopulacao = Populacao[0];          // A população já se encontra ordenada por fitness, pelo que o melhor indivíduo estará na posição 0
            TCromossoma solucaoRetornar = (Configuracao.ProcessoDeEvolucao, SolucaoCandidata!.Fitness, melhorSolucaoPopulacao.Fitness) switch
            {
                (AGProcessoDeEvolucao.MINIMIZACAO, int sC, int mS) when sC < mS => SolucaoCandidata,
                (AGProcessoDeEvolucao.MAXIMIZACAO, int sC, int mS) when sC > mS => SolucaoCandidata,
                _ => (TCromossoma)melhorSolucaoPopulacao.Clone()
            };

            // Contabilizar a quantidade de gerações sem evolução
            QuantidadeGeracoesSemEvolucao = solucaoRetornar.Fitness == SolucaoCandidata.Fitness
                ? QuantidadeGeracoesSemEvolucao + 1
                : 0;
            if (QuantidadeGeracoesSemEvolucao == 0)
            {
                Configuracao.OutputService.OnMelhorSolucaoEncontrada(solucaoRetornar, GeracaoAtual);
            }

            return SolucaoCandidata = solucaoRetornar;
        }

        /// <summary>
        /// Configura o estado inicial do algoritmo, criando a população inicial e avaliando o fitness dos indivíduos.
        /// </summary>
        protected void InicializarAlgoritmo()
        {
            GeracaoAtual = 0;
            SolucaoCandidata = Configuracao.CromossomaFactory.CriarAleatorio();
            Configuracao.ProcessoCalculoFitness.CalcularFitness(SolucaoCandidata);
            Configuracao.ProcessoCalculoFitness.CalcularCodigoUnico(SolucaoCandidata);

            _performanceDuracaoEmMilisegundos = Relogio.ElapsedMilliseconds;
            PerformanceTotalDeGeracoes = GeracaoAtual;
            PerformanceGeracoesPorSegundo = 0;

            QuantidadeGeracoesSemEvolucao = 0;

            InicializarPopulacao();
        }
        /// <summary>
        /// Preenche a população inicial com indivíduos aleatórios criados pela fábrica de cromossomas, garantindo que o tamanho da população corresponde ao definido em AGConfiguracao.
        /// </summary>
        protected void InicializarPopulacao()
        {
            Populacao.Clear();
            Populacao.Capacity = Configuracao.DimensaoDaPopulacao;
            PreencherAleatoriamentePopulacaoEmFalta();
        }

        /// <summary>
        /// Garante que a população mantém o tamanho definido em AGConfiguracao, preenchendo faltas com novos indivíduos aleatórios.
        /// </summary>
        protected bool PreencherAleatoriamentePopulacaoEmFalta()
        {
            if (Populacao is not List<TCromossoma> || Populacao.Count >= Configuracao.DimensaoDaPopulacao)
            {
                return false;
            }

            Populacao.Capacity = Configuracao.DimensaoDaPopulacao;
            int dimensaoAtual = Populacao.Count;
            while (dimensaoAtual++ < Configuracao.DimensaoDaPopulacao)
            {
                Populacao.Add(Configuracao.CromossomaFactory.CriarAleatorio());
            }
            return true;
        }

        /// <summary>
        /// Invoca o callback de feedback (UI/Consola) respeitando os intervalos de tempo configurados.
        /// </summary>
        /// <param name="forcarValidacaoIntervalo">Se falso, ignora o tempo e força a exibição (útil no final).</param>
        public void FornecerFeedback(bool forcarFeedback = false)
        {
            // Analisar o tempo decorrido desde o último feedback para evitar sobrecarregar a UI ou consola com atualizações muito frequentes, especialmente em algoritmos rápidos.
            long segundosElapsed = Relogio.ElapsedMilliseconds / 1000 + 1;
            if (Relogio.ElapsedMilliseconds - _performanceDuracaoEmMilisegundos > 1000)
            {
                PerformanceGeracoesPorSegundo = GeracaoAtual - PerformanceTotalDeGeracoes;
                _performanceDuracaoEmMilisegundos = Relogio.ElapsedMilliseconds;
                PerformanceTotalDeGeracoes = GeracaoAtual;
            }

            // Validar se pode dar feedback (terminou o período de feedback configurado ou se é uma chamada forçada)
            if (!forcarFeedback)
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
        /// </summary>
        private void RecalcularFitness(List<TCromossoma> populacao)
        {
            ParallelOptions options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount - 2
            };
            Parallel.For(0, populacao.Count, options, filhoIndex =>
            {
                Configuracao.ProcessoCalculoFitness.CalcularFitness(populacao[filhoIndex]);
                Configuracao.ProcessoCalculoFitness.CalcularCodigoUnico(populacao[filhoIndex]);
            });
        }
    }
}
