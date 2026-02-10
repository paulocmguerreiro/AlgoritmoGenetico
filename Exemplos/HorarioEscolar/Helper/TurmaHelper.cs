using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Estrutura;

namespace HorarioEscolar.Helper
{
    public static class TurmaHelper
    {

        public static Dictionary<string, TurmaCSV>? Turmas;
        private static List<TurmaCSV>? _turmasList;

        public static void ValidarTurmasInicializacao()
        {
            if (Turmas is null)
            {
                throw new NullReferenceException("Turmas não foram inicializadas");
            }
        }
        public static Dictionary<string, TurmaCSV> ToDictionary()
        {
            ValidarTurmasInicializacao();
            return Turmas!;

        }

        public static List<TurmaCSV> AsList()
        {
            ValidarTurmasInicializacao();
            return _turmasList!;
        }

        public static int ObterTurmaIndicacaoDeTempoBloqueado(string? turma, int diaDaSemana, string hora)
        {
            if (turma is null)
            {
                return 0;
            }
            ValidarTurmasInicializacao();
            TurmaCSV turmaAConsultar = Turmas![turma];
            return turmaAConsultar.TemposLetivos[HorarioHelper.ObterIndexAPartirDaHora(hora) * HorarioHelper.DiasDaSemanaIndex.Count + diaDaSemana];
        }

        public static bool TryCarregarTurmas()
        {
            string filePath = @"./DATA/turmas.csv";
            Turmas = null;

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = false,
                }))
                {
                    var records = csv.GetRecords<TurmaCSV>().ToList();
                    Dictionary<string, TurmaCSV> TurmasDict = [];

                    if (records.Any(x => x.Disciplinas.Count() == 0))
                    {
                        throw new ArgumentOutOfRangeException("Definição das turmas estão incorretas");
                    }

                    records.ForEach(record =>
                    {
                        record.Sigla = record.Sigla.ToUpper();
                        record.Disciplinas = record.Disciplinas.Select(x => x.ToUpper()).ToList();
                        TurmasDict[record.Sigla] = record;
                    });

                    Turmas = TurmasDict;
                    _turmasList = Turmas.Values.ToList();

                    return true;
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine($"O ficheiro não foi encontrado: {ex.Message}");
            }
            catch (CsvHelperException ex)
            {
                Console.Error.WriteLine($"Erro ao processar o arquivo CSV: {ex.Message}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.Error.WriteLine($"Erro nos dados do CSV: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
            }

            return false;
        }

        public static bool TryCarregarTurmaHorario()
        {
            string filePath = @"./DATA/turmas_horarios.csv";

            ValidarTurmasInicializacao();
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = false,
                }))
                {
                    var records = csv.GetRecords<TurmaHorarioCSV>().ToList();
                    if (records.Any(x => x.TemposLetivos.Count() == 0))
                    {
                        throw new ArgumentOutOfRangeException("Definição dos Tempos Letivos da turma estão incorretos");
                    }

                    Dictionary<string, TurmaCSV> TurmaHorariosDict = [];
                    records.ForEach(record =>
                    {
                        record.Sigla = record.Sigla.ToUpper();
                        TurmaHorariosDict[record.Sigla] = new TurmaCSV
                        {
                            Sigla = record.Sigla,
                            Disciplinas = Turmas![record.Sigla].Disciplinas.ToList(),
                            TemposLetivos = record.TemposLetivos,
                        };
                    });

                    Turmas = TurmaHorariosDict;
                    _turmasList = Turmas.Values.ToList();
                    return true;
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine($"O ficheiro não foi encontrado: {ex.Message}");
            }
            catch (CsvHelperException ex)
            {
                Console.Error.WriteLine($"Erro ao processar o arquivo CSV: {ex.Message}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.Error.WriteLine($"Erro nos dados do CSV: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
            }

            return false;
        }

        public static bool TryCarregarTurmaProfessores()
        {
            string filePath = @"./DATA/turmas_disciplinas_profs.csv";

            ValidarTurmasInicializacao();
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = false,
                }))
                {
                    var records = csv.GetRecords<TurmaDisciplinaProfessorCSV>().ToList();
                    Dictionary<string, TurmaCSV> TurmaDisciplinasProfessoresDict = [];

                    records.ForEach(record =>
                    {
                        if (!TurmaDisciplinasProfessoresDict.ContainsKey(record.Sigla))
                        {
                            record.Sigla = record.Sigla.ToUpper();
                            TurmaDisciplinasProfessoresDict[record.Sigla] = new TurmaCSV
                            {
                                Sigla = record.Sigla,
                                Disciplinas = Turmas![record.Sigla].Disciplinas.ToList(),
                                TemposLetivos = Turmas![record.Sigla].TemposLetivos.ToList(),
                                Professores = []
                            };
                        }
                        TurmaDisciplinasProfessoresDict[record.Sigla].Professores.Add(record.Disciplina.ToUpper(), record.Professor.ToUpper());
                    });

                    Turmas = TurmaDisciplinasProfessoresDict;
                    _turmasList = Turmas.Values.ToList();
                    return true;
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.Error.WriteLine($"O ficheiro não foi encontrado: {ex.Message}");
            }
            catch (CsvHelperException ex)
            {
                Console.Error.WriteLine($"Erro ao processar o arquivo CSV: {ex.Message}");
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.Error.WriteLine($"Erro nos dados do CSV: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Ocorreu um erro inesperado: {ex.Message}");
            }

            return false;
        }

    }
}
