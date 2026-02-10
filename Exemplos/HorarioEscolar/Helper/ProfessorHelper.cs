using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Estrutura;

namespace HorarioEscolar.Helper
{
    public static class ProfessorHelper
    {
        public static Dictionary<string, ProfessorCSV>? Professores;

        public static List<ProfessorCSV>? ToList()
        {
            return Professores!.Values.ToList();
        }

        public static int ObterProfIndicacaoDeTempoBloqueado(string? prof, int diaDaSemana, string hora)
        {
            if (prof is null)
            {
                return 0;
            }
            ProfessorCSV profAConsultar = Professores![prof];
            return profAConsultar.TemposLetivos[HorarioHelper.ObterIndexAPartirDaHora(hora) * HorarioHelper.DiasDaSemanaIndex.Count + diaDaSemana];

        }
        public static bool TryCarregarProfessores()
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
