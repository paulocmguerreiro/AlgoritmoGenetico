using System.Collections.ObjectModel;

namespace AlgoritmoGenetico.Core.Abstracoes
{
    /// <summary>
    /// Define o contrato para um Cromossoma no algoritmo genético.
    /// O cromossoma corresponde a uma solução candidata composta por um conjunto de genes.
    /// </summary>
    public interface ICromossoma<out TGene> where TGene : IGene
    {
        /// <summary> Coleção de genes que formam a estrutura deste cromossoma. </summary>
        IEnumerable<TGene> Genes { get; }

        /// <summary> Obtém o valor de avaliação (qualidade) da solução. </summary>
        int Fitness { get; set; }

        /// <summary> 
        /// Obtém um código identificador único (hash) baseado na composição dos genes. 
        /// Útil para distinguir indivíduos distintos com o mesmo Fitness.
        /// Importante para evitar perda de diversidade genética e para operações de seleção e sobrevivência principalmente para Crowding.
        /// </summary>
        int CodigoUnico { get; set; }

        /// <summary> Prepara o cromossoma para uso, executando lógicas de pré-processamento se necessário. </summary>
        void Inicializar();

        /// <summary>
        /// Limpar os valores do Fitness e CodigoUnico
        /// </summary>
        void Reset();

        /// <summary> Adiciona um novo gene à estrutura do cromossoma. </summary>
        void AdicionarGene(IGene gene);

        /// <summary> Cria uma cópia do cromossoma e dos seus genes. </summary>
        public abstract ICromossoma<TGene> Clone();
    }

}
