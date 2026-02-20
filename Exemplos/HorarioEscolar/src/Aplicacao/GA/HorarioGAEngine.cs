
using AlgoritmoGenetico.Aplicacao.Motor;
using HorarioEscolar.Applicacao.GA.Modelo;

namespace HorarioEscolar.Applicacao.GA
{
    /// <summary>
    /// Especialização do motor de algoritmo genético para o problema de horário escolar, utilizando HorarioCromossoma como tipo de cromossoma. Esta classe pode ser estendida para incluir configurações específicas do problema, operadores personalizados ou lógica adicional necessária para a evolução de soluções de horários escolares.
    /// </summary>
    /// <typeparam name="TCromossoma"></typeparam>
    public class AGHorarioEscolar<TCromossoma> : AG<TCromossoma> where TCromossoma : HorarioCromossoma
    {
    }
}
