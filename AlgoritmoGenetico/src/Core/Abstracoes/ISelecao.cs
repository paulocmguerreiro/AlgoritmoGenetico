
using AlgoritmoGenetico.Aplicacao.Configuracao;

namespace AlgoritmoGenetico.Core.Abstracoes
{
    /// <summary>
    /// Define o contrato para as estratégias de seleção.
    /// Responsável por filtrar a população e escolher os indivíduos para reprodução ou sobrevivência.
    /// </summary>
    public interface ISelecao<TCromossoma> where TCromossoma : ICromossoma<IGene>
    {
        /// <summary> Retorna a descrição da estratégia de seleção. </summary>
        string ToString();

        /// <summary>
        /// Prepara a população para o processo de seleção (ex: ordenação).
        /// </summary>
        public void Preparar(List<TCromossoma> populacaoCompleta, AGConfiguracao<TCromossoma> configuracaoAtual);

        /// <summary> Seleciona uma sub-lista de indivíduos para formar a nova geração. </summary>
        public List<TCromossoma> EscolherPopulacao();

        /// <summary> Seleciona um único indivíduo para um processo específico (ex: escolha de um pai). </summary>
        public TCromossoma EscolherIndividuo();
    }
}
