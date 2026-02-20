using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HorarioEscolar.Applicacao.GA.Modelo;

namespace HorarioEscolar.Core.Cache
{
    /// <summary>
    /// Fábrica especializada para a criação e gestão de instâncias de <see cref="HorarioGene"/>.
    /// Implementa um padrão de Object Pooling (Flyweight) para garantir que genes com 
    /// a mesma configuração sejam reutilizados em vez de instanciados novamente.
    /// </summary>
    public static class HorarioGeneFactory
    {
        /// <summary> Contador global de pedidos de criação de genes. </summary>
        public static int Chamadas = 0;

        /// <summary> 
        /// Dicionário thread-safe que armazena os genes únicos. 
        /// Utiliza uma <see cref="GeneKey"/> como identificador de unicidade.
        /// </summary>
        private static readonly ConcurrentDictionary<GeneKey, HorarioGene> _genePool = new();

        /// <summary> Retorna o número total de instâncias únicas de genes armazenadas em memória. </summary>
        public static int PoolSize => _genePool.Count;

        /// <summary>
        /// Estrutura imutável interna utilizada como chave de cache. 
        /// Define a igualdade baseada nos valores da Turma, Professor, Disciplina e na lista canónica de aulas.
        /// </summary>
        private record GeneKey(
            string Turma,
            string Professor,
            string Disciplina,
            List<HorarioDiasGene> AulasPointer
        );

        /// <summary>
        /// Obtém um gene do pool ou cria um novo se não existir uma configuração idêntica.
        /// </summary>
        /// <param name="turma">Sigla da turma.</param>
        /// <param name="prof">Nome/ID do professor.</param>
        /// <param name="disc">Unidade Curricular.</param>
        /// <param name="estaEmColisao">Estado inicial de colisão.</param>
        /// <param name="aulasTemporarias">Lista de aulas a processar para a forma canónica.</param>
        /// <returns>Uma instância de <see cref="HorarioGene"/> (nova ou reutilizada).</returns>
        public static HorarioGene GetGene(string turma, string prof, string disc, bool estaEmColisao, List<HorarioDiasGene> aulasTemporarias)
        {
            Chamadas++;
            List<HorarioDiasGene> listaAulasCanonica = HorarioDiasGeneFactory.GetListaCanonica(aulasTemporarias);

            GeneKey key = new GeneKey(turma, prof, disc, listaAulasCanonica);

            return _genePool.GetOrAdd(key, key => new HorarioGene
            {
                Turma = key.Turma,
                Professor = key.Professor,
                Disciplina = key.Disciplina,
                EstaEmColisao = estaEmColisao,
                Aulas = key.AulasPointer
            });
        }
    }
}
