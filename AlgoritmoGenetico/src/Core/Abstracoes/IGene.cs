namespace AlgoritmoGenetico.Core.Abstracoes
{
    /// <summary>
    /// Representa a unidade mínima de informação genética (um gene).
    /// </summary>
    public abstract class IGene
    {
        /// <summary> 
        /// Indica se este gene específico está em conflito com outros ou com restrições do sistema. 
        /// </summary>
        public bool EstaEmColisao = false;

        /// <summary> Cria uma cópia do gene. </summary>
        public abstract IGene Clone();
    }
}
