
using AlgoritmoGenetico.Core.Operadores.Mutacao;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Cache;
using HorarioEscolar.Core.Interfaces;

namespace HorarioEscolar.Applicacao.GA.Mutacao
{
    /// <summary>
    /// Fornece métodos de extensão e lógica especializada para aplicar mutações em cromossomas de horários.
    /// Esta classe gere a alteração de salas, dias e horas.
    /// </summary>
    public class MutacaoService(IHorarioService horarioService, IDisciplinaService disciplinaService) : Multipla<HorarioCromossoma>
    {
        /// <summary>
        /// Tenta aplicar sequencialmente mutação de Sala, Dia ou Hora.
        /// </summary>
        /// <returns>True se o gene sofreu alguma alteração física.</returns>
        private bool TryAplicarMutacao(ref HorarioGene gene)
        {
            bool m1 = TryAplicarMutacaoSala(ref gene);
            bool m2 = TryAplicarMutacaoDia(ref gene);
            bool m3 = TryAplicarMutacaoHora(ref gene);
            return m1 || m2 || m3;
        }
        /// <summary>
        /// Ponto de entrada principal para o processo de mutação de um indivíduo.
        /// Escolhe um gene aleatoriamente e tenta aplicar uma das três estratégias de mutação disponíveis.
        /// </summary>
        public override void ProcessarMutacao(HorarioCromossoma individuoAProcessar)
        {
            HorarioCromossoma? individuo = individuoAProcessar;
            IList<HorarioGene> individuoGenes = (IList<HorarioGene>)individuo.Genes;

            int geneIndex = Random.Shared.Next(individuoGenes.Count);
            HorarioGene gene = individuoGenes[geneIndex];

            if (PodeMutar(gene) && TryAplicarMutacao(ref gene))
            {

                individuo.Reset();
                individuoGenes[geneIndex] = gene;
            }
        }

        /// <summary>
        /// Tenta alterar a sala de aula de um bloco letivo. 
        /// </summary>
        private bool TryAplicarMutacaoSala(ref HorarioGene gene)
        {
            if (!PodeMutar(gene))
            {
                return false;
            }
            int totalAulas = gene.Aulas.Count;
            HorarioDiasGene[] listaTemporaria = new HorarioDiasGene[totalAulas];

            int diaAlterar = Random.Shared.Next(totalAulas);
            string salaOriginal = gene.Aulas[diaAlterar].SalaDeAula;
            if (salaOriginal == "")
            {
                return false;
            }
            string novaSala = disciplinaService.ObterSalaAleatoria(gene.Disciplina);
            for (int aulaIndex = 0; aulaIndex < totalAulas; aulaIndex++)
            {
                HorarioDiasGene aula = gene.Aulas[aulaIndex];
                if (aula.SalaDeAula == salaOriginal)
                {
                    listaTemporaria[aulaIndex] = HorarioDiasGeneFactory.GetAula(
                        aula.DiaDaAula,
                        aula.DuracaoTemposLetivos,
                        novaSala,
                        aula.HoraInicioDaAula
                    );
                }
                else
                {
                    listaTemporaria[aulaIndex] = aula;
                }
            }
            gene = HorarioGeneFactory.GetGene(gene.Turma, gene.Professor, gene.Disciplina, gene.EstaEmColisao, listaTemporaria.ToList());
            return true;
        }

        /// <summary>
        /// Tenta alterar o horário de início de uma aula. 
        /// Garante a continuidade pedagógica (se o bloco for de 2 tempos, a hora seguinte é ajustada).
        /// </summary>
        private bool TryAplicarMutacaoHora(ref HorarioGene gene)
        {

            if (!PodeMutar(gene))
            {
                return false;
            }
            List<HorarioDiasGene> diasUtilizados = gene.Aulas;
            int totalAulasDias = diasUtilizados.Count;
            HorarioDiasGene[] listaTemporaria = new HorarioDiasGene[totalAulasDias];
            int diaAlterarIndex = Random.Shared.Next(totalAulasDias);
            int diaAlterar = diasUtilizados[Random.Shared.Next(totalAulasDias)].DiaDaAula;

            string horaInicial = horarioService.ObterHoraAleatoriaDoDia(diasUtilizados[diaAlterarIndex].DuracaoTemposLetivos);
            for (int aulaIndex = 0; aulaIndex < totalAulasDias; aulaIndex++)
            {
                HorarioDiasGene aula = gene.Aulas[aulaIndex];
                if (aula.DiaDaAula == diaAlterar)
                {
                    listaTemporaria[aulaIndex] = HorarioDiasGeneFactory.GetAula(
                        aula.DiaDaAula,
                        aula.DuracaoTemposLetivos,
                        aula.SalaDeAula,
                        horaInicial
                    );
                    horaInicial = horarioService.ObterHoraSeguinte(horaInicial);
                }
                else
                {
                    listaTemporaria[aulaIndex] = aula;
                }
            }
            gene = HorarioGeneFactory.GetGene(gene.Turma, gene.Professor, gene.Disciplina, gene.EstaEmColisao, listaTemporaria.ToList());

            return true;
        }

        /// <summary>
        /// Tenta mover uma aula para um dia da semana diferente. 
        /// Utiliza um HashSet para identificar dias livres e evitar sobreposições no próprio gene da disciplina.
        /// </summary>
        private bool TryAplicarMutacaoDia(ref HorarioGene gene)
        {
            if (!PodeMutar(gene))
            {
                return false;
            }
            HashSet<int> diasUtilizadosDiasBuffer = new HashSet<int>(7);
            List<HorarioDiasGene> diasUtilizados = gene.Aulas;
            int totalAulasDias = diasUtilizados.Count;
            HorarioDiasGene[] listaTemporaria = new HorarioDiasGene[totalAulasDias];
            foreach (HorarioDiasGene aula in diasUtilizados)
            {
                diasUtilizadosDiasBuffer.Add(aula.DiaDaAula);
            }
            List<int> diasDisponiveis = horarioService.DiasDaSemanaIndex.Where(x => !diasUtilizadosDiasBuffer.Contains(x)).ToList();

            int alterarParaDia = diasDisponiveis[Random.Shared.Next(diasDisponiveis.Count)];
            int alterarDoDia = diasUtilizados[Random.Shared.Next(diasUtilizados.Count)].DiaDaAula;

            for (int aulaIndex = 0; aulaIndex < totalAulasDias; aulaIndex++)
            {
                HorarioDiasGene aula = gene.Aulas[aulaIndex];
                if (aula.DiaDaAula == alterarDoDia)
                {
                    listaTemporaria[aulaIndex] = HorarioDiasGeneFactory.GetAula(
                        alterarParaDia,
                        aula.DuracaoTemposLetivos,
                        aula.SalaDeAula,
                        aula.HoraInicioDaAula
                    );
                }
                else
                {
                    listaTemporaria[aulaIndex] = aula;
                }
            }
            gene = HorarioGeneFactory.GetGene(gene.Turma, gene.Professor, gene.Disciplina, gene.EstaEmColisao, listaTemporaria.ToList());

            return true;
        }

    }
}
