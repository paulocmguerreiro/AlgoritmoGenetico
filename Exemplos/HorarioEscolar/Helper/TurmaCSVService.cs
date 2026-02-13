using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Estrutura;

namespace HorarioEscolar.Helper
{
    public class TurmaCSVService : TurmaBaseService
    {
        public TurmaCSVService(IHorarioService horarioService) : base(horarioService)
        {
        }

        public override void CarregarDados()
        {
            if (!TryCarregarTurmas()) throw new Exception("Falha ao carregar as Turmas");
            if (!TryCarregarTurmaHorario()) throw new Exception("Falha ao carregar os Horários das Turmas");
            if (!TryCarregarTurmaProfessores()) throw new Exception("Falha ao carregar os Professores das Turmas");
        }

        public bool TryCarregarTurmas()
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

        public bool TryCarregarTurmaHorario()
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

        public bool TryCarregarTurmaProfessores()
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
