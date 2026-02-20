
using AlgoritmoGenetico.Core.Abstracoes;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Cache;
using HorarioEscolar.Core.Interfaces;

namespace HorarioEscolar.Applicacao.GA
{
    /// <summary>
    /// Fábrica de cromossomas especializada para o problema de horário escolar, responsável por criar instâncias de HorarioCromossoma, seja aleatórias (com genes gerados com base nas turmas e disciplinas disponíveis) ou vazias (sem genes, para uso em operações genéticas como crossover e mutação).
    /// </summary>
    public class HorarioCromossomaFactory(ITurmaService turmaService, IDisciplinaService disciplinaService) : ICromossomaFactory<HorarioCromossoma>
    {
        /// <summary>
        /// Cria um cromossoma aleatório, preenchendo seus genes com combinações de turmas, disciplinas e professores disponíveis, utilizando os serviços de turma e disciplina para obter as informações necessárias para a construção dos genes.
        /// </summary>
        /// <returns></returns>
        public HorarioCromossoma CriarAleatorio()
        {
            HorarioCromossoma cromossoma = new HorarioCromossoma();
            foreach (var turma in turmaService.AsList())
            {
                foreach (var disciplinaProfessor in turma.Professores)
                {
                    List<int> temposLetivosDaDisciplina = disciplinaService.ObterTemposLetivos(disciplinaProfessor.Key);
                    cromossoma.AdicionarGene(HorarioGeneFactory.GetGene(
                        turma.Sigla,
                        disciplinaProfessor.Value,
                        disciplinaProfessor.Key,
                        false,
                        disciplinaService.DistribuirTemposLetivos(disciplinaProfessor.Key, temposLetivosDaDisciplina)
                    ));
                }
            }
            return cromossoma;
        }

        /// <summary>
        /// Cria um cromossoma vazio, sem genes, preparado para ser preenchido posteriormente em operações genéticas como crossover e mutação. Este método é útil para criar novos indivíduos que serão derivados de outros através de manipulações genéticas, garantindo que comecem sem informações pré-existentes.
        /// </summary>
        /// <returns></returns>
        public HorarioCromossoma CriarVazio()
        {
            return new HorarioCromossoma { Genes = new List<HorarioGene>() };
        }
    }
}
