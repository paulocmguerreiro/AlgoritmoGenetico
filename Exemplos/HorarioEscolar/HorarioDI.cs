using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlgoritmoGenetico.Extensao;
using AlgoritmoGenetico.Individuo;
using AlgoritmoGenetico.Mutacao;
using AlgoritmoGenetico.Recombinacao;
using HorarioEscolar.Helper;
using HorarioEscolar.Individuo;
using HorarioEscolar.Individuo.Extension;
using Microsoft.Extensions.DependencyInjection;

namespace HorarioEscolar
{
    public static class HorarioDI
    {
        public static IServiceCollection AddCSV(this IServiceCollection services)
        {
            // Services
            services.AddSingleton<IHorarioService, HorarioCSVService>();
            services.AddSingleton<IDisciplinaService, DisciplinaCSVService>();
            services.AddSingleton<IProfessorService, ProfessorCSVService>();
            services.AddSingleton<ITurmaService, TurmaCSVService>();

            return services;
        }

        public static IServiceCollection AddAlgoritmo(this IServiceCollection services)
        {

            // AG Engine
            services.AddSingleton<ICromossomaFactory<HorarioCromossoma>, HorarioCromossomaFactory>();
            services.AddSingleton<IRecombinacao<HorarioCromossoma>, CycleCrossOver<HorarioCromossoma>>();
            services.AddSingleton<ICromossomaFitnessService<HorarioCromossoma>, HorarioCromossomaFitness>();
            services.AddSingleton<IAGOutputService<HorarioCromossoma>, HorarioOutputService>();
            services.AddSingleton<IMutacaoService<HorarioCromossoma>>(sp =>
                new MutacaoService(
                    sp.GetRequiredService<IHorarioService>(),
                    sp.GetRequiredService<IDisciplinaService>())
                {
                    FatorMutacaoColisao = 0.50d,
                    FatorMutacaoNormal = 0.05d,
                    AjustarMutacaoACadaGeracao = 100,
                    QuantidadeDeMutacoes = 10,
                });

            return services;
        }
    }
}
