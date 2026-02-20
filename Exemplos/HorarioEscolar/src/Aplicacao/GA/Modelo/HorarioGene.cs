
using AlgoritmoGenetico.Core.Abstracoes;
using HorarioEscolar.Core.Cache;

namespace HorarioEscolar.Applicacao.GA.Modelo
{
    /// <summary>
    /// Representa a unidade genética de uma disciplina específica para uma determinada turma e professor.
    /// Agrupa o conjunto de aulas semanais associadas a esse vínculo.
    /// </summary>
    public class HorarioGene : IGene
    {
        /// <summary> Identificador da turma à qual o gene pertence. </summary>
        public string Turma { get; set; } = "";
        /// <summary> Nome ou ID do docente responsável. </summary>
        public string Professor { get; set; } = "";
        /// <summary> Nome ou ID da unidade curricular. </summary>
        public string Disciplina { get; set; } = "";
        /// <summary> Lista das sessões (aulas) planeadas para este gene durante a semana. </summary>
        public List<HorarioDiasGene> Aulas { get; set; } = new List<HorarioDiasGene>();

        /// <summary> Duplica o gene mantendo a integridade dos dados para novos indivíduos. </summary>        
        public override HorarioGene Clone() =>
        HorarioGeneFactory.GetGene(
                this.Turma,
                this.Professor,
                this.Disciplina,
                this.EstaEmColisao,
                this.Aulas
            );


    }



}
