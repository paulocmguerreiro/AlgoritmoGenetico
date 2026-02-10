using AlgoritmoGenetico.Configuracao;
using AlgoritmoGenetico.Individuo;
using HorarioEscolar.Helper;
using HorarioEscolar.Factories;

namespace HorarioEscolar.Individuo.Extension
{
    /// <summary>
    /// Fornece métodos de extensão e lógica especializada para aplicar mutações em cromossomas de horários.
    /// Esta classe gere a alteração cirúrgica de salas, dias e horas, respeitando as restrições de domínio.
    /// </summary>
    public class MutacaoExtensions<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        /// <summary> Referência para as definições globais do motor (taxas de mutação, limites, etc.). </summary>
        private AGConfiguracao<TCromossoma>? _configuracao;

        /// <summary>
        /// Tenta aplicar sequencialmente mutação de Sala, Dia ou Hora até que uma seja bem-sucedida.
        /// </summary>
        /// <param name="gene">Referência do gene a ser modificado.</param>
        /// <returns>True se o gene sofreu alguma alteração física.</returns>
        private bool TryAplicarMutacao(ref HorarioGene gene) =>
            TryAplicarMutacaoSala(ref gene) || TryAplicarMutacaoDia(ref gene) || TryAplicarMutacaoHora(ref gene);


        /// <summary>
        /// Ponto de entrada principal para o processo de mutação de um indivíduo.
        /// Escolhe um gene aleatoriamente e tenta aplicar uma das três estratégias de mutação disponíveis.
        /// </summary>
        /// <param name="individuoAProcessar">O cromossoma que poderá sofrer a mutação.</param>
        /// <param name="Configuracao">A configuração atual do Algoritmo Genético.</param>
        public void ProcessarMutacao(ICromossoma<IGene> individuoAProcessar, AGConfiguracao<TCromossoma> Configuracao)
        {
            _configuracao = Configuracao;
            if (Configuracao is null)
            {
                return;
            }
            HorarioCromossoma individuo = (HorarioCromossoma)individuoAProcessar;
            IList<HorarioGene> individuoGenes = (IList<HorarioGene>)individuo.Genes;

            int geneIndex = Random.Shared.Next(individuoGenes.Count);
            HorarioGene gene = individuoGenes[geneIndex];

            if (_configuracao.ProcessoDeMutacao.PodeMutar(gene) && TryAplicarMutacao(ref gene))
            {
                individuo.ResetFitness();
                individuoGenes[geneIndex] = gene;
            }
        }

        /// <summary>
        /// Tenta alterar a sala de aula de um bloco letivo. 
        /// Utiliza o DisciplinaHelper para garantir que a nova sala é compatível com a disciplina.
        /// </summary>
        private bool TryAplicarMutacaoSala(ref HorarioGene gene)
        {
            if (!_configuracao!.ProcessoDeMutacao.PodeMutar(gene))
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
            string novaSala = DisciplinaHelper.ObterSalaAleatoriaDaDisciplina(gene.Disciplina);
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
            if (!_configuracao!.ProcessoDeMutacao.PodeMutar(gene))
            {
                return false;
            }

            List<HorarioDiasGene> diasUtilizados = gene.Aulas;
            int totalAulasDias = diasUtilizados.Count;
            HorarioDiasGene[] listaTemporaria = new HorarioDiasGene[totalAulasDias];
            int diaAlterarIndex = Random.Shared.Next(totalAulasDias);
            int diaAlterar = diasUtilizados[Random.Shared.Next(totalAulasDias)].DiaDaAula;

            string horaInicial = HorarioHelper.ObterHoraAleatoriaDoDia(diasUtilizados[diaAlterarIndex].DuracaoTemposLetivos);
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
                    horaInicial = HorarioHelper.ObterHoraSeguinte(horaInicial);
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
            if (!_configuracao!.ProcessoDeMutacao.PodeMutar(gene))
            {
                return false;
            }
            HashSet<int> diasUtilizadosDiasBuffer = new HashSet<int>(7);
            List<HorarioDiasGene> diasUtilizados = gene.Aulas;
            int totalAulasDias = diasUtilizados.Count;
            HorarioDiasGene[] listaTemporaria = new HorarioDiasGene[totalAulasDias];
            foreach (HorarioDiasGene aula in diasUtilizados)
            {
                diasUtilizadosDiasBuffer.Add(aula.DiaDaAula); // Preenche o HashSet O(1)
            }
            List<int> diasDisponiveis = HorarioHelper.DiasDaSemanaIndex.Where(x => !diasUtilizadosDiasBuffer.Contains(x)).ToList();

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
