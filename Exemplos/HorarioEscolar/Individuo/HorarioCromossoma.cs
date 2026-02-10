using AlgoritmoGenetico.Individuo;
using HorarioEscolar.Individuo.Extension;
using HorarioEscolar.Helper;
using HorarioEscolar.Factories;
using System.Net.Quic;

namespace HorarioEscolar.Individuo
{
    /// <summary>
    /// Implementação concreta de um cromossoma para o problema de horários escolares.
    /// Gere uma coleção de genes que representam o horário semanal completo.
    /// </summary>
    public class HorarioCromossoma : ICromossoma<HorarioGene>
    {

        /// <summary> Armazena o valor de aptidão (fitness) em cache para evitar recálculos desnecessários. </summary>
        private int? _fitness;
        /// <summary> Identificador único baseado no conteúdo genético para controlo de diversidade. </summary>
        private int? _codigoUnico;

        /// <summary> Estrutura de memória reutilizável para chaves de colisão (evita pressao no Garbage Collector). </summary>
        private (char, string, int, string)[] _chaves = new (char, string, int, string)[]
        {
            ('\0', "", 0, ""),
            ('\0', "", 0, ""),
            ('\0', "", 0, "")
        };
        private int[] _colisaoTempoBloqueado = { 0, 0, 0 };

        /// <summary> Mapeamento espaço-temporal para deteção rápida de conflitos entre recursos. </summary>
        private Dictionary<(char Tipo, string Id, int Dia, string Hora), (HorarioGene firstGene, int qtdColisoes)> _contagemColisoes = [];

        /// <summary> Lista interna de genes que compõem a solução. </summary>
        private List<HorarioGene> _genes = new List<HorarioGene>();

        /// <summary> Obtém ou define a coleção de genes (unidades de disciplina/turma) do cromossoma. </summary>
        public IEnumerable<HorarioGene> Genes
        {
            get => _genes;
            set => _genes = value.ToList();
        }

        /// <summary> 
        /// Calcula ou retorna o Fitness. O valor representa o total de colisões 
        /// (Turma, Professor, Sala e Tempos Bloqueados). 
        /// </summary>        
        public int Fitness => _fitness ??= ProcessarColisoesAgregado();
        /// <summary> Retorna o código hash único que identifica esta configuração específica de genes. </summary>
        public int CodigoUnico => _codigoUnico ??= ObterCodigoUnicoDoCromossoma();

        /// <summary> Limpa os caches de fitness e código único, forçando uma nova avaliação. </summary>
        public ICromossoma<HorarioGene> ResetFitness()
        {
            _fitness = null;
            _codigoUnico = null;
            return this;
        }

        /// <summary>
        /// Executa a lógica central de avaliação, identificando colisões de recursos 
        /// e atualizando o estado de cada gene individualmente.
        /// </summary>
        /// <returns>O somatório total de conflitos encontrados.</returns>
        private int ProcessarColisoesAgregado()
        {

            int totalColisoes = 0;
            _contagemColisoes.Clear();

            foreach (HorarioGene gene in Genes)
            {
                int totalColisoesGene = 0;
                foreach (HorarioDiasGene aula in gene.Aulas)
                {
                    // Turma, Professor ou Sala com colisão?
                    _chaves[0] = ('T', gene.Turma, aula.DiaDaAula, aula.HoraInicioDaAula);
                    _chaves[1] = ('P', gene.Professor, aula.DiaDaAula, aula.HoraInicioDaAula);
                    _chaves[2] = ('S', aula.SalaDeAula, aula.DiaDaAula, aula.HoraInicioDaAula);
                    // Tempos Bloqueados?
                    _colisaoTempoBloqueado[0] = TurmaHelper.ObterTurmaIndicacaoDeTempoBloqueado(gene.Turma, aula.DiaDaAula, aula.HoraInicioDaAula);
                    _colisaoTempoBloqueado[1] = ProfessorHelper.ObterProfIndicacaoDeTempoBloqueado(gene.Professor, aula.DiaDaAula, aula.HoraInicioDaAula);
                    _colisaoTempoBloqueado[2] = HorarioHelper.ObterHorarioIndicacaoDeTempoBloqueado(aula.DiaDaAula, aula.HoraInicioDaAula);

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
                            // Quando encontra a primeira colisão (tenho que ter em conta a posição original)
                            if (colisoesNaChave.qtdColisoes == 0)
                            {
                                colisoesNaChave.qtdColisoes++;
                                totalColisoes++;
                            }
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

            _codigoUnico = ObterCodigoUnicoDoCromossoma();

            _fitness = totalColisoes;
            return Fitness;
        }


        public void Inicializar()
        {
            _genes.Clear();
            ResetFitness();
        }

        /// <summary> Cria uma cópia profunda (Deep Copy) do cromossoma e de todos os seus genes. </summary>
        public ICromossoma<HorarioGene> Clone() =>
            new HorarioCromossoma
            {
                //Fitness = null,
                Genes = this.Genes.Select(geneOriginal => geneOriginal.Clone()).ToList()
            };


        public void AdicionarGene(IGene gene) => _genes.Add((HorarioGene)gene);


        public static ICromossoma<HorarioGene> CriarVazio() =>
            new HorarioCromossoma
            {
                //Fitness = null,
                Genes = new List<HorarioGene>()
            };



        /// <summary> Gera um individuo com dados aleatórios mas respeitando as cargas horárias. </summary>
        public static ICromossoma<HorarioGene> CriarAleatorio()
        {
            HorarioCromossoma cromossoma = new HorarioCromossoma();
            foreach (var turma in TurmaHelper.AsList())
            {
                foreach (var disciplinaProfessor in turma.Professores)
                {
                    List<int> temposLetivosDaDisciplina = DisciplinaHelper.ObterTemposLetivosDaDisciplinaProfessor(disciplinaProfessor.Key);
                    cromossoma._genes.Add(HorarioGeneFactory.GetGene(
                        turma.Sigla,
                        disciplinaProfessor.Value,
                        disciplinaProfessor.Key,
                        false,
                        HorarioExtensions.DistribuirTemposLetivos(disciplinaProfessor.Key, temposLetivosDaDisciplina)
                    ));
                }
            }
            return cromossoma;
        }


        private int ObterCodigoUnicoDoCromossoma()
        {
            int hash = 19;
            foreach (var gene in Genes)
            {
                unchecked
                {
                    hash = hash * 31 + (gene?.GetHashCode() ?? 0);
                }
            }
            return hash;
        }
    }
}
