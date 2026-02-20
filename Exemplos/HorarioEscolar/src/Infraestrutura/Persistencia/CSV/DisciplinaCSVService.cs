
using System.Collections.Frozen;
using System.Globalization;
using System.Reflection.Metadata;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Core.Entidades;
using HorarioEscolar.Core.Interfaces;
using HorarioEscolar.Infraestrutura.Persistencia.Abstracoes;
using HorarioEscolar.Infraestrutura.Persistencia.CSV.Mapeamentos;

namespace HorarioEscolar.Infraestrutura.Persistencia.CSV
{
    /// <summary>
    /// Implementação do serviço de disciplinas que carrega os dados a partir de arquivos CSV. Esta classe é responsável por ler os arquivos CSV contendo as informações das disciplinas e suas respectivas salas, processar esses dados e disponibilizá-los para o restante da aplicação por meio dos métodos definidos na interface IDisciplinaService. A implementação inclui tratamento de erros para garantir a robustez do processo de carregamento dos dados, como verificação da existência dos arquivos, validação do formato dos dados e captura de exceções inesperadas durante a leitura dos arquivos CSV.
    /// </summary>
    public class DisciplinaCSVService(IHorarioService horarioService) : DisciplinaBaseService(horarioService)
    {

        public override void CarregarDados()
        {
            if (!TryCarregarDisciplinas()) throw new Exception("Falha ao carregar disciplinas.");
            if (!TryCarregarSalas()) throw new Exception("Falha ao carregar salas.");
        }

        /// <summary>
        /// Tenta carregar os dados das disciplinas a partir de um arquivo CSV, processando as informações e armazenando-as em um dicionário. O método inclui tratamento de erros para lidar com situações como arquivo não encontrado, erros de formatação do CSV e outros erros inesperados, garantindo que a aplicação possa lidar adequadamente com falhas no processo de carregamento dos dados.
        /// </summary>
        /// <returns></returns>
        private bool TryCarregarDisciplinas()
        {
            string filePath = Path.Combine("Data", "disciplinas.csv");
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
                    var records = csv.GetRecords<DisciplinaRecord>().ToList();

                    _disciplinas = records.Select(record =>
                    new Disciplina()
                    {
                        Sigla = record.Sigla.ToUpper(),
                        TemposLetivos = record.TemposLetivos
                    })
                    .ToDictionary(d => d.Sigla, d => d);

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

        /// <summary>
        /// Tenta carregar os dados das salas associadas às disciplinas a partir de um arquivo CSV, processando as informações e armazenando-as em um dicionário. O método inclui tratamento de erros para lidar com situações como arquivo não encontrado, erros de formatação do CSV, dados inconsistentes (como disciplinas sem salas) e outros erros inesperados, garantindo que a aplicação possa lidar adequadamente com falhas no processo de carregamento dos dados. Além disso, o método valida se as disciplinas foram carregadas antes de tentar associar as salas, garantindo a integridade dos dados durante o processo de carregamento.
        /// </summary>
        /// <returns></returns>
        private bool TryCarregarSalas()
        {
            string filePath = Path.Combine("Data", "disciplinas_salas.csv");
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
                    var records = csv.GetRecords<DisciplinaSalasRecord>().ToList();

                    if (records.Any(x => x.Salas.Count() == 0))
                    {
                        throw new ArgumentOutOfRangeException("Definição das salas das disciplinas estão incorretas");
                    }

                    records.ForEach(record => _disciplinas![record.Sigla.ToUpper()].Salas = record.Salas);

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
