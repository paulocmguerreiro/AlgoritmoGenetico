
using System.Collections.Frozen;
using HorarioEscolar.Applicacao.GA.Modelo;
using HorarioEscolar.Core.Cache;
using HorarioEscolar.Core.Entidades;
using HorarioEscolar.Core.Interfaces;

namespace HorarioEscolar.Infraestrutura.Persistencia.Abstracoes
{
    /// <summary>
    /// Classe base abstrata para o serviço de disciplinas, fornecendo uma implementação comum para os métodos relacionados às disciplinas.
    /// </summary>
    /// <param name="horarioService"></param>
    public abstract class DisciplinaBaseService(IHorarioService horarioService) : IDisciplinaService
    {

        protected Dictionary<string, Disciplina>? _disciplinas;

        /// <summary>
        /// Valida se as disciplinas foram inicializadas, lançando uma exceção caso contrário. Este método é utilizado para garantir que os dados das disciplinas estejam disponíveis antes de acessar seus métodos, evitando erros de referência nula e garantindo a integridade dos dados durante a execução do programa.
        /// </summary>
        /// <exception cref="NullReferenceException"></exception>
        protected void ValidarInicializacao()
        {
            if (_disciplinas is null)
            {
                throw new NullReferenceException("Disciplinas não foram inicializados");
            }

        }
        public List<int> ObterTemposLetivos(string disciplinaProfessor)
        {
            ValidarInicializacao();
            return _disciplinas![disciplinaProfessor].TemposLetivos;
        }
        public List<string> ObterSalas(string disciplinaProfessor)
        {
            ValidarInicializacao();
            return _disciplinas![disciplinaProfessor].Salas;
        }

        public string ObterSalaAleatoria(string disciplinaProfessor)
        {
            List<string> salasDaDisciplina = ObterSalas(disciplinaProfessor);
            return salasDaDisciplina[Random.Shared.Next(salasDaDisciplina.Count)];
        }
        public List<HorarioDiasGene> DistribuirTemposLetivos(string disciplinaSigla, List<int> temposLetivosDaDisciplina)
        {
            List<int> diasDisponiveis = horarioService.DiasDaSemanaIndex.ToList();

            List<HorarioDiasGene> aulas = new List<HorarioDiasGene>();

            foreach (int tempoLetivo in temposLetivosDaDisciplina)
            {
                int diaDaAula = Random.Shared.Next(diasDisponiveis.Count);
                string horaInicioDaAula = horarioService.ObterHoraAleatoriaDoDia(tempoLetivo);
                string salaDeAula = ObterSalaAleatoria(disciplinaSigla);
                int distribuirTempoLetivo = tempoLetivo;
                while (distribuirTempoLetivo >= 1)
                {
                    aulas.Add(
                       HorarioDiasGeneFactory.GetAula(
                            diasDisponiveis[diaDaAula],
                            tempoLetivo,
                            salaDeAula,
                            horaInicioDaAula
                        ));
                    horaInicioDaAula = horarioService.ObterHoraSeguinte(horaInicioDaAula);
                    distribuirTempoLetivo--;
                }
                diasDisponiveis.RemoveAt(diaDaAula);
            }
            return HorarioDiasGeneFactory.GetListaCanonica(aulas);
        }

        public abstract void CarregarDados();
    }
}
