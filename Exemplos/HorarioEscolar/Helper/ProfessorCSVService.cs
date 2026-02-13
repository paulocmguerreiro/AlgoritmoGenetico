using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Estrutura;

namespace HorarioEscolar.Helper
{
    public class ProfessorCSVService : ProfessorBaseService
    {
        public ProfessorCSVService(IHorarioService horarioService) : base(horarioService)
        {
        }

        public override void CarregarDados()
        {
            // Unificamos o carregamento dos dois CSVs aqui
            if (!TryCarregarProfessores()) throw new Exception("Falha ao carregar professores.");
        }
        public bool TryCarregarProfessores()
        {
            string filePath = @"./DATA/profs_horarios.csv";

            Professores = null;
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = false,
                }))
                {
                    var records = csv.GetRecords<ProfessorCSV>().ToList();
                    Dictionary<string, ProfessorCSV> ProfessoresDict = [];

                    if (records.Any(x => x.TemposLetivos.Count() == 0))
                    {
                        throw new ArgumentOutOfRangeException("Definição dos pesos da semana estão incorretos");
                    }

                    records.ForEach(record =>
                    {
                        record.Sigla = record.Sigla.ToUpper();
                        ProfessoresDict[record.Sigla] = record;
                    });

                    Professores = ProfessoresDict;
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
