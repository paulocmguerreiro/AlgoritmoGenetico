using AlgoritmoGenetico.Core.Abstracoes;
using AlgoritmoGenetico.Core.Operadores.Recombinacao;
using AlgoritmoGenetico.Core.Operadores.Selecao;
using HorarioEscolar.Applicacao.GA;
using HorarioEscolar.Applicacao.GA.Fitness;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Applicacao.GA.Mutacao;
using HorarioEscolar.Apresentacao.UI;
using HorarioEscolar.Core.Interfaces;
using HorarioEscolar.Infraestrutura.Persistencia.CSV;
using Microsoft.Extensions.DependencyInjection;

namespace HorarioEscolar.Infraestrutura
{
    /// <summary>
    /// Classe de configuração de injeção de dependências para a aplicação de horário escolar, responsável por registrar os serviços e implementações necessários para o funcionamento da aplicação, incluindo os serviços de carregamento de dados a partir de arquivos CSV e os componentes do algoritmo genético utilizados para gerar os horários escolares. Esta classe centraliza a configuração das dependências, facilitando a manutenção e a escalabilidade da aplicação, além de garantir que as implementações corretas sejam injetadas nos componentes que as utilizam ao longo da aplicação.
    /// </summary>
    public static class HorarioDI
    {
        /// <summary>
        /// Configura os serviços de injeção de dependências para a aplicação, registrando as implementações dos serviços de dados (como IHorarioService, IDisciplinaService, IProfessorService e ITurmaService) que carregam os dados a partir de arquivos CSV, bem como os componentes do algoritmo genético (como ICromossomaFactory, IRecombinacao, ICromossomaFitnessService, IAGOutputService, ISelecao e IMutacaoService) utilizados para gerar os horários escolares. O método retorna uma instância de IServiceCollection com os serviços registrados, permitindo que a aplicação utilize a injeção de dependências para resolver as dependências necessárias em tempo de execução.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddCSV(this IServiceCollection services)
        {
            // Services
            services.AddSingleton<IHorarioService, HorarioCSVService>();
            services.AddSingleton<IDisciplinaService, DisciplinaCSVService>();
            services.AddSingleton<IProfessorService, ProfessorCSVService>();
            services.AddSingleton<ITurmaService, TurmaCSVService>();

            return services;
        }

        /// <summary>
        /// Configura os serviços relacionados ao algoritmo genético para a aplicação, registrando as implementações dos componentes necessários para a execução do algoritmo genético, como a fábrica de cromossomas, os operadores de recombinação e seleção, o serviço de fitness e o serviço de mutação. O método retorna uma instância de IServiceCollection com os serviços registrados, permitindo que a aplicação utilize a injeção de dependências para resolver as dependências necessárias em tempo de execução e facilitar a manutenção e escalabilidade da aplicação.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAlgoritmo(this IServiceCollection services)
        {

            // AG Engine
            services.AddSingleton<ICromossomaFactory<HorarioCromossoma>, HorarioCromossomaFactory>();
            services.AddSingleton<IRecombinacao<HorarioCromossoma>, Uniforme<HorarioCromossoma>>();
            services.AddSingleton<ICromossomaFitnessService<HorarioCromossoma>, HorarioCromossomaFitness>();
            services.AddSingleton<IAGOutputService<HorarioCromossoma>, HorarioOutputService>();
            services.AddSingleton<ISelecao<HorarioCromossoma>, Crowding<HorarioCromossoma>>();
            services.AddSingleton<IMutacaoService<HorarioCromossoma>>(sp =>
                new MutacaoService(
                    sp.GetRequiredService<IHorarioService>(),
                    sp.GetRequiredService<IDisciplinaService>())
                {
                    FatorMutacaoColisao = 0.25d,
                    FatorMutacaoNormal = 0.05d,
                    AjustarMutacaoACadaGeracao = 100,
                    QuantidadeDeMutacoes = 10,

                });

            return services;
        }
    }
}
