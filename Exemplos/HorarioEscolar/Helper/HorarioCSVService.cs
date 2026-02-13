using System.Collections.ObjectModel;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Estrutura;

namespace HorarioEscolar.Helper
{
    public class HorarioCSVService : HorarioBaseService
    {
        public override void CarregarDados()
        {
            if (!TryCarregarHorario()) throw new Exception("Falha ao carregar o horário.");
        }

        public bool TryCarregarHorario()
        {
            string filePath = @"./DATA/horario.csv";

            _horarios = null;
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = false,
                }))
                {
                    var records = csv.GetRecords<HorarioCSV>().ToList();
                    if (records.Any(x => x.Dias.Count() != 5))
                    {
                        throw new ArgumentOutOfRangeException("Definição dos pesos da semana estão incorretos");
                    }

                    Dictionary<string, HorarioCSV> HorariosDict = [];
                    records.ForEach(record => HorariosDict[record.Hora] = record);
                    _horarios = HorariosDict;
                    _horariosList = _horarios.Keys.ToList();
                    _horariosIndexMap = _horariosList.Select((hora, index) => (hora, index)).ToDictionary(x => x.hora, x => x.index);

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
