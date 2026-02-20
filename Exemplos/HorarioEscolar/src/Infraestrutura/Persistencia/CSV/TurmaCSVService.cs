
using System.Collections.Frozen;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Core.Entidades;
using HorarioEscolar.Core.Interfaces;
using HorarioEscolar.Infraestrutura.Persistencia.Abstracoes;
using HorarioEscolar.Infraestrutura.Persistencia.CSV.Mapeamentos;
using Spectre.Console;

namespace HorarioEscolar.Infraestrutura.Persistencia.CSV
{
    /// <summary>
    /// Implementação do serviço de turmas que carrega os dados a partir de arquivos CSV. Esta classe é responsável por ler os arquivos CSV contendo as informações das turmas, seus horários e professores associados, processar esses dados e disponibilizá-los para o restante da aplicação por meio dos métodos definidos na interface ITurmaService. A implementação inclui tratamento de erros para garantir a robustez do processo de carregamento dos dados, como verificação da existência dos arquivos, validação do formato dos dados, validação da consistência dos dados (como turmas sem disciplinas ou horários) e captura de exceções inesperadas durante a leitura dos arquivos CSV. 
    /// </summary>
    /// <param name="horarioService"></param>
    public class TurmaCSVService(IHorarioService horarioService) : TurmaBaseService(horarioService)
    {

        public override void CarregarDados()
        {
            if (!TryCarregarTurmas()) throw new Exception("Falha ao carregar as Turmas");
            if (!TryCarregarTurmaHorario()) throw new Exception("Falha ao carregar os Horários das Turmas");
            if (!TryCarregarTurmaProfessores()) throw new Exception("Falha ao carregar os Professores das Turmas");
        }

        /// <summary>
        /// Tenta carregar os dados das turmas a partir de um arquivo CSV, processando as informações e armazenando-as em um dicionário. O método inclui tratamento de erros para lidar com situações como arquivo não encontrado, erros de formatação do CSV, dados inconsistentes (como turmas sem disciplinas ou horários) e outros erros inesperados, garantindo que a aplicação possa lidar adequadamente com falhas no processo de carregamento dos dados. Além disso, o método valida se os dados das turmas estão consistentes (como a quantidade correta de disciplinas associadas a cada turma) antes de armazená-los, garantindo a integridade dos dados durante o processo de carregamento.
        /// </summary>
        /// <returns></returns>
        public bool TryCarregarTurmas()
        {
            string filePath = Path.Combine("Data", "turmas.csv");
            _turmas = null;
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = false,
                }))
                {
                    var records = csv.GetRecords<TurmaRecord>().ToList();
                    if (records.Any(x => x.Disciplinas.Count() == 0))
                    {
                        throw new ArgumentOutOfRangeException("Definição das turmas estão incorretas");
                    }

                    _turmas = records.Select(record =>
                        new Turma()
                        {
                            Sigla = record.Sigla.ToUpper(),
                            Disciplinas = record.Disciplinas.Select(x => x.ToUpper()).ToList(),
                            Professores = [],
                            TemposLetivos = []
                        })
                    .ToDictionary(t => t.Sigla, t => t);

                    _turmasList = _turmas.Values.ToList();

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
        /// Tenta carregar os dados dos horários associados às turmas a partir de um arquivo CSV, processando as informações e armazenando-as em um dicionário. O método inclui tratamento de erros para lidar com situações como arquivo não encontrado, erros de formatação do CSV, dados inconsistentes (como turmas sem horários) e outros erros inesperados, garantindo que a aplicação possa lidar adequadamente com falhas no processo de carregamento dos dados. Além disso, o método valida se os dados dos horários das turmas estão consistentes (como a quantidade correta de tempos letivos associada a cada turma) antes de armazená-los, garantindo a integridade dos dados durante o processo de carregamento.
        /// </summary>
        /// <returns></returns>
        public bool TryCarregarTurmaHorario()
        {
            string filePath = Path.Combine("Data", "turmas_horarios.csv");

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
                    var records = csv.GetRecords<TurmaHorarioRecord>().ToList();
                    if (records.Any(x => x.TemposLetivos.Count() == 0))
                    {
                        throw new ArgumentOutOfRangeException("Definição dos Tempos Letivos da turma estão incorretos");
                    }

                    records.ForEach(record => _turmas![record.Sigla.ToUpper()].TemposLetivos = record.TemposLetivos);

                    _turmasList = _turmas!.Values.ToList();
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
        /// Tenta carregar os dados dos professores associados às turmas a partir de um arquivo CSV, processando as informações e armazenando-as em um dicionário. O método inclui tratamento de erros para lidar com situações como arquivo não encontrado, erros de formatação do CSV, dados inconsistentes (como turmas sem professores) e outros erros inesperados, garantindo que a aplicação possa lidar adequadamente com falhas no processo de carregamento dos dados. Além disso, o método valida se os dados dos professores das turmas estão consistentes (como a quantidade correta de disciplinas associada a cada professor) antes de armazená-los, garantindo a integridade dos dados durante o processo de carregamento.
        /// </summary>
        /// <returns></returns>
        public bool TryCarregarTurmaProfessores()
        {
            string filePath = Path.Combine("Data", "turmas_disciplinas_profs.csv");

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
                    var records = csv.GetRecords<TurmaDisciplinaProfessorRecord>().ToList();

                    records.ForEach(record => _turmas![record.Sigla.ToUpper()].Professores.Add(record.Disciplina.ToUpper(), record.Professor.ToUpper()));

                    _turmasList = _turmas!.Values.ToList();
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
