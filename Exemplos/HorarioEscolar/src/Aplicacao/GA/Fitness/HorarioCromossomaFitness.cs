
using System.Collections.Concurrent;
using AlgoritmoGenetico.Core.Abstracoes;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Interfaces;

namespace HorarioEscolar.Applicacao.GA.Fitness
{
    /// <summary>
    /// Implementação específica de serviço de fitness para o problema de horário escolar, utilizando uma abordagem de mapeamento espaço-temporal para detecção eficiente de conflitos entre recursos (turmas, professores e salas).
    /// </summary>
    public class HorarioCromossomaFitness(IHorarioService horarioService, IProfessorService professorService, ITurmaService turmaService) : ICromossomaFitnessService<HorarioCromossoma>
    {
        /// <summary>
        /// Executa a lógica central de avaliação, identificando colisões de recursos 
        /// e atualizando o estado de cada gene individualmente.
        /// </summary>
        /// <returns>O somatório total de conflitos (Fitness) encontrados.</returns>
        public int CalcularFitness(HorarioCromossoma individuo)
        {
            /// <summary> Estrutura de memória reutilizável para chaves de colisão (evita pressao no Garbage Collector). </summary>
            (char, string, int, string)[] _chaves = new (char, string, int, string)[]
            {
            ('\0', "", 0, ""),
            ('\0', "", 0, ""),
            ('\0', "", 0, "")
            };
            int[] _colisaoTempoBloqueado = { 0, 0, 0 };
            ConcurrentDictionary<(char Tipo, string Id, int Dia, string Hora), (HorarioGene firstGene, int qtdColisoes)> _contagemColisoes = [];
            int totalColisoes = 0;

            foreach (HorarioGene gene in individuo.Genes)
            {
                int totalColisoesGene = 0;
                foreach (HorarioDiasGene aula in gene.Aulas)
                {
                    // Turma, Professor ou Sala com colisão?
                    _chaves[0] = ('T', gene.Turma, aula.DiaDaAula, aula.HoraInicioDaAula);
                    _chaves[1] = ('P', gene.Professor, aula.DiaDaAula, aula.HoraInicioDaAula);
                    _chaves[2] = ('S', aula.SalaDeAula, aula.DiaDaAula, aula.HoraInicioDaAula);
                    // Tempos Bloqueados?
                    _colisaoTempoBloqueado[0] = turmaService.ObterTurmaIndicacaoDeTempoBloqueado(gene.Turma, aula.DiaDaAula, aula.HoraInicioDaAula);
                    _colisaoTempoBloqueado[1] = professorService.ObterProfIndicacaoDeTempoBloqueado(gene.Professor, aula.DiaDaAula, aula.HoraInicioDaAula);
                    _colisaoTempoBloqueado[2] = horarioService.ObterHorarioIndicacaoDeTempoBloqueado(aula.DiaDaAula, aula.HoraInicioDaAula);

                    // Validar as possíveis colisões
                    for (int chaveIndex = 0; chaveIndex < 3; chaveIndex++)
                    {
                        var chave = _chaves[chaveIndex];
                        if (!_contagemColisoes.TryGetValue(chave, out (HorarioGene firstGene, int qtdColisoes) colisoesNaChave))
                        {
                            colisoesNaChave = (firstGene: gene, qtdColisoes: 0);
                        }
                        else
                        {
                            // Quando encontra a segunda colisão (tenho que ter em conta a posição original)
                            if (colisoesNaChave.qtdColisoes == 0)
                            {
                                colisoesNaChave.qtdColisoes++;
                                colisoesNaChave.firstGene.EstaEmColisao = true;
                                totalColisoes++;
                                totalColisoesGene++;
                            }

                            // Contabilizar a colisão detetada
                            totalColisoes++;
                            totalColisoesGene++;
                            colisoesNaChave.qtdColisoes++;
                        }

                        totalColisoes += _colisaoTempoBloqueado[chaveIndex];
                        totalColisoesGene += _colisaoTempoBloqueado[chaveIndex];
                        _contagemColisoes[chave] = (firstGene: colisoesNaChave.firstGene, qtdColisoes: colisoesNaChave.qtdColisoes + _colisaoTempoBloqueado[chaveIndex]);
                        _contagemColisoes[chave].firstGene.EstaEmColisao |= totalColisoesGene > 0;
                    }

                }
                gene.EstaEmColisao = totalColisoesGene > 0;
            }

            individuo.Fitness = totalColisoes;
            return totalColisoes;
        }

        /// <summary>
        /// Gera um código único para o cromossoma com base nos seus genes, utilizando uma função de hash personalizada que combina os valores dos genes para criar um identificador único.
        /// </summary>
        /// <param name="individuo"></param>
        /// <returns></returns>
        public int CalcularCodigoUnico(HorarioCromossoma individuo)
        {
            int hash = 19;
            foreach (var gene in individuo.Genes)
            {
                unchecked
                {
                    hash = hash * 31 + (gene?.GetHashCode() ?? 0);
                }
            }
            individuo.CodigoUnico = hash;
            return hash;

        }
    }
}
