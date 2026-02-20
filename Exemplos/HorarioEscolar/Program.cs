using AlgoritmoGenetico.Aplicacao.Configuracao;
using AlgoritmoGenetico.Aplicacao.Motor;
using AlgoritmoGenetico.Core.Abstracoes;
using HorarioEscolar.Applicacao.GA;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Interfaces;
using HorarioEscolar.Infraestrutura;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

var builder = Host.CreateApplicationBuilder(args);

// DI 
builder.Services.AddCSV();
builder.Services.AddAlgoritmo();

// SERVIÇOS CSV
using IHost host = builder.Build();
var horarioService = host.Services.GetRequiredService<IHorarioService>();
var disciplinaService = host.Services.GetRequiredService<IDisciplinaService>();
var professoresService = host.Services.GetRequiredService<IProfessorService>();
var turmaService = host.Services.GetRequiredService<ITurmaService>();

// SERVIÇOS ALGORITMO
var cromossomaFactory = host.Services.GetRequiredService<ICromossomaFactory<HorarioCromossoma>>();
var mutacaoService = host.Services.GetRequiredService<IMutacaoService<HorarioCromossoma>>();
var recombinacaoService = host.Services.GetRequiredService<IRecombinacao<HorarioCromossoma>>();
var selecaoService = host.Services.GetRequiredService<ISelecao<HorarioCromossoma>>();
var fitnessService = host.Services.GetRequiredService<ICromossomaFitnessService<HorarioCromossoma>>();
var outputService = host.Services.GetRequiredService<IAGOutputService<HorarioCromossoma>>();

CarregarDadosCSV();

var algoritmo = ConfigurarAlgoritmo();

algoritmo.ProcurarSolucao();
algoritmo.FornecerFeedback();

return;

AGHorarioEscolar<HorarioCromossoma> ConfigurarAlgoritmo()
{

    AGConfiguracao<HorarioCromossoma> agConfig = new AGConfiguracao<HorarioCromossoma>
    {
        DimensaoDaPopulacao = 100,
        LimiteMáximoDeGeracoesPermitidas = 50000,
        FitnessPretendido = 0f,
        ReporSolucaoCandidataNaPopulacaoACadaGeracao = 50,
        DarFeedbackACadaSegundo = 1,
        ProcessoDeEvolucao = AGProcessoDeEvolucao.MINIMIZACAO,
        ProcessoCalculoFitness = fitnessService,
        ProcessoDeSelecaoDaProximaGeracao = selecaoService,
        ProcessoDeRecombinacao = recombinacaoService,
        ProcessoDeMutacao = mutacaoService,
        ProbabilidadeDeSelecionarDaGeracaoPais = 0.25f,
        ProbabilidadeDeSelecionarDaGeracaoFilhos = .75f,
        CromossomaFactory = cromossomaFactory,
        OutputService = outputService
    };


    return new AGHorarioEscolar<HorarioCromossoma>
    {
        Configuracao = agConfig,
    };

}

void CarregarDadosCSV()
{
    try
    {
        AnsiConsole.Status()
        .Spinner(Spinner.Known.Dots)
        .Start("Obter estrutura dos horários...", ctx =>
        {
            ctx.Status("A carregar horário...");
            horarioService.CarregarDados();
            ctx.Status("A carregar disciplinas...");
            disciplinaService.CarregarDados();
            ctx.Status("A carregar professores...");
            professoresService.CarregarDados();
            ctx.Status("A carregar turmas...");
            turmaService.CarregarDados();
        });
    }
    catch (Exception ex)
    {
        AnsiConsole.Write("[red]Erro Fatal[/]");
        AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
        return;
    }
}
