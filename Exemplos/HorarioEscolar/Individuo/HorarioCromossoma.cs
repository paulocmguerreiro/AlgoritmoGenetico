using AlgoritmoGenetico.Individuo;
using HorarioEscolar.Factories;
using HorarioEscolar.Helper;
using HorarioEscolar.Individuo.Extension;

namespace HorarioEscolar.Individuo
{
    /// <summary>
    /// Implementação concreta de um cromossoma para o problema de horários escolares.
    /// Gere uma coleção de genes que representam o horário semanal completo.
    /// </summary>
    public class HorarioCromossoma() : ICromossoma<HorarioGene>
    {
        private int? _fitness;
        private int? _codigoUnico;

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
        public int Fitness
        {
            get => _fitness ?? throw new InvalidOperationException("Tentativa de acesso ao fitness antes da avaliação");
            set => _fitness = value;
        }
        /// <summary> Retorna o código hash único que identifica esta configuração específica de genes. </summary>
        public int CodigoUnico
        {
            get => _codigoUnico ?? throw new InvalidOperationException("Tentativa de acesso ao código único antes da avaliação");
            set => _codigoUnico = value;
        }

        public void Inicializar()
        {
            _genes.Clear();
        }
        public void Reset()
        {
            _fitness = null;
            _codigoUnico = null;
        }

        /// <summary> Cria uma cópia profunda (Deep Copy) do cromossoma e de todos os seus genes. </summary>
        public ICromossoma<HorarioGene> Clone() =>
            new HorarioCromossoma
            {
                Fitness = Fitness,
                CodigoUnico = CodigoUnico,
                Genes = this.Genes.Select(geneOriginal => geneOriginal.Clone()).ToList()
            };


        public void AdicionarGene(IGene gene) => _genes.Add((HorarioGene)gene);
    }
}
