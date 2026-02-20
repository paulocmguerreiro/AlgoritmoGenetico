
using System.Collections.Frozen;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Core.Entidades;
using HorarioEscolar.Core.Interfaces;
using HorarioEscolar.Infraestrutura.Persistencia.Abstracoes;
using HorarioEscolar.Infraestrutura.Persistencia.CSV.Mapeamentos;

namespace HorarioEscolar.Infraestrutura.Persistencia.CSV
{
    public class ProfessorCSVService(IHorarioService horarioService) : ProfessorBaseService(horarioService)
    {
        public override void CarregarDados()
        {
            if (!TryCarregarProfessores()) throw new Exception("Falha ao carregar professores.");
        }
        /// <summary>
        /// Tenta carregar os dados dos professores a partir de um arquivo CSV, processando as informações e armazenando-as em um dicionário. O método inclui tratamento de erros para lidar com situações como arquivo não encontrado, erros de formatação do CSV, dados inconsistentes (como professores sem horários) e outros erros inesperados, garantindo que a aplicação possa lidar adequadamente com falhas no processo de carregamento dos dados. Além disso, o método valida se os dados dos professores estão consistentes (como a quantidade correta de tempos letivos) antes de armazená-los, garantindo a integridade dos dados durante o processo de carregamento.
        /// </summary>
        /// <returns></returns>
        public bool TryCarregarProfessores()
        {
            string filePath = Path.Combine("Data", "profs_horarios.csv");

            _professores = null;
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = false,
                }))
                {
                    var records = csv.GetRecords<ProfessorRecord>().ToList();
                    Dictionary<string, Professor> ProfessoresDict = [];

                    if (records.Any(x => x.TemposLetivos.Count() == 0))
                    {
                        throw new ArgumentOutOfRangeException("Definição dos pesos da semana estão incorretos");
                    }

                    records.ForEach(record =>
                    {
                        record.Sigla = record.Sigla.ToUpper();
                        ProfessoresDict[record.Sigla] = new Professor()
                        {
                            Sigla = record.Sigla,
                            TemposLetivos = record.TemposLetivos
                        };
                    });

                    _professores = ProfessoresDict;
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
