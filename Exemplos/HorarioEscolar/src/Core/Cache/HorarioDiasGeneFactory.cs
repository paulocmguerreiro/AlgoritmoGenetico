using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HorarioEscolar.Applicacao.GA.Modelo;

namespace HorarioEscolar.Core.Cache
{
    /// <summary>
    /// Fábrica responsável pela gestão de instâncias de <see cref="HorarioDiasGene"/>.
    /// Implementa dois níveis de cache (Pool) para otimizar a memória: um para aulas individuais e outro para listas canónicas.
    /// </summary>
    public static class HorarioDiasGeneFactory
    {
        /// <summary> Contador de pedidos de criação de objetos de aula individuais. </summary>
        public static int ChamadasDias = 0;
        /// <summary> Contador de pedidos de normalização/obtenção de listas de aulas. </summary>
        public static int ChamadasLista = 0;
        /// <summary> Pool de instâncias únicas de <see cref="HorarioDiasGene"/> indexado pela tupla de estado. </summary>
        private static readonly ConcurrentDictionary<(int, int, string, string), HorarioDiasGene> _aulaPool = new();
        public static int AulaPoolSize => _aulaPool.Count;

        /// <summary> Pool de listas ordenadas e imutáveis de aulas, utilizando o hash da coleção como chave. </summary>
        private static readonly ConcurrentDictionary<int, List<HorarioDiasGene>> _listPool = new();
        public static int ListPoolSize => _listPool.Count;

        /// <summary>
        /// Obtém uma instância reciclada de uma aula baseada nos seus parâmetros espaciais e temporais.
        /// </summary>
        /// <param name="dia">Dia da semana.</param>
        /// <param name="duracao">Número de tempos letivos.</param>
        /// <param name="sala">Identificador da sala.</param>
        /// <param name="hora">Hora de início.</param>
        /// <returns>Uma instância imutável de <see cref="HorarioDiasGene"/>.</returns>
        public static HorarioDiasGene GetAula(int dia, int duracao, string sala, string hora)
        {
            ChamadasDias++;
            var key = (Dia: dia, Duracao: duracao, Sala: sala, Hora: hora);

            return _aulaPool.GetOrAdd(key, new HorarioDiasGene
            {
                DiaDaAula = key.Dia,
                DuracaoTemposLetivos = key.Duracao,
                SalaDeAula = key.Sala,
                HoraInicioDaAula = key.Hora
            });
        }

        /// <summary>
        /// Transforma uma lista temporária de aulas numa "Forma Canónica": ordenada por dia/hora e partilhada via Pool.
        /// Essencial para garantir que genes idênticos em conteúdo mas diferentes em ordem de criação partilhem a mesma referência.
        /// </summary>
        /// <param name="aulasTemp">A lista de aulas a normalizar.</param>
        /// <returns>Uma lista ordenada e cacheada.</returns>
        public static List<HorarioDiasGene> GetListaCanonica(List<HorarioDiasGene> aulasTemp)
        {
            ChamadasLista++;
            // Evitar recriar lista a reserva extra por conversão em List;
            //var aulasOrdenadas = aulasTemp.OrderBy(a => a.DiaDaAula).ThenBy(a => a.HoraInicioDaAula).ToList();
            List<HorarioDiasGene> aulasOrdenadas = aulasTemp;
            aulasOrdenadas.Sort((a, b) =>
            {
                int res = a.DiaDaAula.CompareTo(b.DiaDaAula);
                return res != 0 ? res : a.HoraInicioDaAula.CompareTo(b.HoraInicioDaAula);
            });

            // Acrescentar algum tipo de aleatoriedade para evitar colisões
            int hash = 19;
            foreach (HorarioDiasGene aula in aulasOrdenadas)
            {
                hash = hash * 31 + aula.GetHashCode();
            }

            return _listPool.GetOrAdd(hash, aulasOrdenadas);

        }
    }

}
