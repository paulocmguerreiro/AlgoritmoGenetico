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


        public static void MostrarInformacao(this HorarioGene gene)
        {
            Console.WriteLine("------------------");
            Console.WriteLine($"Turma       : {gene.Turma}");
            Console.WriteLine($"Professor   : {gene.Professor}");
            Console.WriteLine($"Disciplina  : {gene.Disciplina}");
            //Console.WriteLine($"Aulas       : {gene.Aulas.Count}");
            Console.WriteLine();
            gene.Aulas.ForEach(aula =>
            {
                string diaDaSemana = HorarioHelper.DiasDaSemanaDescritivo[aula.DiaDaAula];
                Console.WriteLine($"{diaDaSemana,-11} : {aula.HoraInicioDaAula,5} ({aula.DuracaoTemposLetivos}tl) {aula.SalaDeAula}");
            });
            Console.WriteLine("------------------");
        }
    }
}
