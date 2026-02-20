namespace HorarioEscolar.Applicacao.GA.Modelo
{

    /// <summary>
    /// Estrutura de dados imutável que define uma aula específica no espaço e no tempo.
    /// </summary>
    public record HorarioDiasGene
    {
        /// <summary> Dia da semana da aula (representação numérica). </summary>
        public int DiaDaAula = 0;
        /// <summary> Quantidade de blocos temporais que a aula ocupa. </summary>
        public int DuracaoTemposLetivos = 0;

        /// <summary> Identificador do slot de tempo de início. </summary>        
        public string HoraInicioDaAula = "";
        /// <summary> Identificador da sala física atribuída. </summary>
        public string SalaDeAula = "";
    }

}
