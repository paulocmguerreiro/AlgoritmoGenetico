using HorarioEscolar.Helper;
using HorarioEscolar.Factories;

namespace HorarioEscolar.Individuo.Extension
{
    public static class HorarioGeneExtensions
    {
        public static HorarioGene Clone(this HorarioGene gene) =>
            HorarioGeneFactory.GetGene(
                gene.Turma,
                gene.Professor,
                gene.Disciplina,
                gene.EstaEmColisao,
                gene.Aulas.Clone()
            );

        public static List<HorarioGene> Clone(this List<HorarioGene> genes) =>
            genes.Select(gene => gene.Clone()).ToList();

    }
}
