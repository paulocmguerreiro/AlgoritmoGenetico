using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Estrutura;

namespace HorarioEscolar.Helper
{
    public class DisciplinaCSVService : DisciplinaBaseService
    {
        public DisciplinaCSVService(IHorarioService horarioService) : base(horarioService)
        {
        }

        public override void CarregarDados()
        {
            if (!TryCarregarDisciplinas()) throw new Exception("Falha ao carregar disciplinas.");
            if (!TryCarregarSalas()) throw new Exception("Falha ao carregar salas.");
        }

        private bool TryCarregarDisciplinas()
        {
            string filePath = @"./DATA/disciplinas.csv";
            _disciplinas = null;

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = false,
                }))
                {
                    var records = csv.GetRecords<DisciplinaCSV>().ToList();
                    Dictionary<string, DisciplinaCSV> DisciplinasDict = [];

                    records.ForEach(record =>
                    {
                        record.Sigla = record.Sigla.ToUpper();
                        DisciplinasDict[record.Sigla] = record;
                    });

                    _disciplinas = DisciplinasDict;
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

        private bool TryCarregarSalas()
        {
            string filePath = @"./DATA/disciplinas_salas.csv";

            ValidarInicializacao();

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = false,
                }))
                {
                    var records = csv.GetRecords<DisciplinaSalasCSV>().ToList();

                    if (records.Any(x => x.Salas.Count() == 0))
                    {
                        throw new ArgumentOutOfRangeException("Definição das salas das disciplinas estão incorretas");
                    }

                    Dictionary<string, DisciplinaCSV> DisciplinasSalasDict = [];
                    records.ForEach(record =>
                    {
                        record.Sigla = record.Sigla.ToUpper();

                        DisciplinasSalasDict[record.Sigla] = new DisciplinaCSV
                        {
                            Sigla = record.Sigla,
                            TemposLetivos = _disciplinas![record.Sigla].TemposLetivos,
                            Salas = record.Salas,
                        };
                    });

                    _disciplinas = DisciplinasSalasDict;
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
