using AlgoritmoGenetico;
using AlgoritmoGenetico.Recombinacao;
using AlgoritmoGenetico.Selecao;
using AlgoritmoGenetico.Configuracao;
using HorarioEscolar;
using HorarioEscolar.Helper;
using HorarioEscolar.Individuo.Extension;
using HorarioEscolar.Individuo;
using AlgoritmoGenetico.Mutacao;
using HorarioEscolar.Factories;


Console.WriteLine("Obter estrutura dos horários...");
if (!HorarioHelper.TryCarregarHorario())
{
    Console.WriteLine("Não foi possível importar o ficheiro dos Horários");
    return;
}
if (!DisciplinaHelper.TryCarregarDisciplinas())
{
    Console.WriteLine("Não foi possível importar o ficheiro das Disciplinas");
    return;
}
if (!DisciplinaHelper.TryCarregarSalas())
{
    Console.WriteLine("Não foi possível importar o ficheiro das Salas das Disciplinas");
    return;
}
if (!TurmaHelper.TryCarregarTurmas())
{
    Console.WriteLine("Não foi possível importar o ficheiro das Turmas");
    return;
}
if (!TurmaHelper.TryCarregarTurmaHorario())
{
    Console.WriteLine("Não foi possível importar o ficheiro dos Tempos Letivos dos Horários");
    return;
}
if (!TurmaHelper.TryCarregarTurmaProfessores())
{
    Console.WriteLine("Não foi possível importar o ficheiro dos Professores das Disciplinas da Turma");
    return;
}
if (!ProfessorHelper.TryCarregarProfessores())
{
    Console.WriteLine("Não foi possível importar o ficheiro dos Professores");
    return;
}


MutacaoExtensions<HorarioCromossoma> mutacaoExt = new MutacaoExtensions<HorarioCromossoma>();

AGConfiguracao<HorarioCromossoma> agConfig = new AGConfiguracao<HorarioCromossoma>
{
    DimensaoDaPopulacao = 50,
    LimiteMáximoDeGeracoesPermitidas = 50000,
    FitnessPretendido = 0f,
    ReporSolucaoCandidataNaPopulacaoACadaGeracao = 50,
    DarFeedbackACadaSegundo = 1,
    FeedbackCallback = DashboardHelper.MostrarInformacao,
    ProcessoDeEvolucao = AGProcessoDeEvolucao.MINIMIZACAO,

    //ProcessoDeRecombinacao = new SemRecombinacao<HorarioCromossoma>(),
    ProcessoDeRecombinacao = new Uniforme<HorarioCromossoma>(),
    //ProcessoDeRecombinacao = new SinglePoint<HorarioCromossoma>(),
    //ProcessoDeRecombinacao = new TwoPoints<HorarioCromossoma>(),
    //ProcessoDeRecombinacao = new CycleCrossOver<HorarioCromossoma>(),

    //ProcessoDeSelecaoDaProximaGeracao = new Todos<HorarioCromossoma>(),
    //ProcessoDeSelecaoDaProximaGeracao = new Truncation<HorarioCromossoma>(),
    ProcessoDeSelecaoDaProximaGeracao = new Crowding<HorarioCromossoma>(),
    //ProcessoDeSelecaoDaProximaGeracao = new Tournament<HorarioCromossoma>(),
    //ProcessoDeSelecaoDaProximaGeracao = new RouletteWheel<HorarioCromossoma>(),
    ProbabilidadeDeSelecionarDaGeracaoPais = 0.10f,
    ProbabilidadeDeSelecionarDaGeracaoFilhos = .90f,

    //ProcessoDeMutacao = new SemMutacao<HorarioCromossoma>(),
    //ProcessoDeMutacao = new Unica<HorarioCromossoma> { FatorMutacaoColisao = .2d, FatorMutacaoNormal = 0.1d, AjustarMutacaoACadaGeracao = 100 },
    ProcessoDeMutacao = new Multipla<HorarioCromossoma> { FatorMutacaoColisao = .50d, FatorMutacaoNormal = 0.05d, QuantidadeDeMutacoes = 10, AjustarMutacaoACadaGeracao = 100 },
    ProcessarMutacaoCallback = mutacaoExt.ProcessarMutacao

};


AGHorarioEscolar<HorarioCromossoma> ag = new AGHorarioEscolar<HorarioCromossoma>
{
    Configuracao = agConfig,
};

ag.ProcurarSolucao();
ag.FornecerFeedback();

return;
