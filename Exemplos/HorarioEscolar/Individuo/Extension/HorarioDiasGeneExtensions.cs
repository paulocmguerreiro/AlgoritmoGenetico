using HorarioEscolar.Factories;

namespace HorarioEscolar.Individuo.Extension
{
    public static class HorarioDiasGeneExtensions
    {
        public static HorarioDiasGene Clone(this HorarioDiasGene dia) =>
            HorarioDiasGeneFactory.GetAula(
                dia.DiaDaAula,
                dia.DuracaoTemposLetivos,
                dia.HoraInicioDaAula,
                dia.SalaDeAula
            );

        public static List<HorarioDiasGene> Clone(this List<HorarioDiasGene> genes) =>
            genes.Select(gene => gene.Clone()).ToList();

    }
}
