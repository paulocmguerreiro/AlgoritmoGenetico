
using System.Collections.Frozen;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Core.Entidades;
using HorarioEscolar.Infraestrutura.Persistencia.Abstracoes;
using HorarioEscolar.Infraestrutura.Persistencia.CSV.Mapeamentos;

namespace HorarioEscolar.Infraestrutura.Persistencia.CSV
{
    /// <summary>
    /// Implementação do serviço de horários que carrega os dados a partir de arquivos CSV. Esta classe é responsável por ler os arquivos CSV contendo as informações dos horários, processar esses dados e disponibilizá-los para o restante da aplicação por meio dos métodos definidos na interface IHorarioService. A implementação inclui tratamento de erros para garantir a robustez do processo de carregamento dos dados, como verificação da existência dos arquivos, validação do formato dos dados e captura de exceções inesperadas durante a leitura dos arquivos CSV.
    /// 
    /// </summary>
    public class HorarioCSVService : HorarioBaseService
    {
        public override void CarregarDados()
        {
            if (!TryCarregarHorario()) throw new Exception("Falha ao carregar o horário.");
        }

        /// <summary>
        /// Tenta carregar os dados dos horários a partir de um arquivo CSV, processando as informações e armazenando-as em um dicionário. O método inclui tratamento de erros para lidar com situações como arquivo não encontrado, erros de formatação do CSV, erros de validação dos dados (como definição incorreta dos pesos da semana) e outros erros inesperados, garantindo que a aplicação possa lidar adequadamente com falhas no processo de carregamento dos dados. Além disso, o método valida se os dados dos horários estão consistentes (como a quantidade correta de dias da semana) antes de armazená-los, garantindo a integridade dos dados durante o processo de carregamento.
        /// </summary>
        /// <returns></returns>
        public bool TryCarregarHorario()
        {
            string filePath = Path.Combine("Data", "horario.csv");

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
                    var records = csv.GetRecords<HorarioRecord>().ToList();
                    if (records.Any(x => x.Dias.Count() != 5))
                    {
                        throw new ArgumentOutOfRangeException("Definição dos pesos da semana estão incorretos");
                    }

                    Dictionary<string, Horario> HorariosDict = [];
                    records.ForEach(record =>
                    {
                        HorariosDict[record.Hora] = new Horario()
                        {
                            Hora = record.Hora,
                            Dias = record.Dias
                        };

                    });
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
