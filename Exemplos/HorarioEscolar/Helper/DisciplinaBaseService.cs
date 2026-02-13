using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HorarioEscolar.Estrutura;
using HorarioEscolar.Factories;
using HorarioEscolar.Individuo;

namespace HorarioEscolar.Helper
{
    public abstract class DisciplinaBaseService(IHorarioService horarioService) : IDisciplinaService
    {

        protected Dictionary<string, DisciplinaCSV>? _disciplinas;
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
