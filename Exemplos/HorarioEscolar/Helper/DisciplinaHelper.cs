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
    public static class DisciplinaHelper
    {
        public static Dictionary<string, DisciplinaCSV>? Disciplinas;
        public static List<int> ObterTemposLetivosDaDisciplinaProfessor(string disciplinaProfessor)
        {
            ValidarDisciplinasInicializacao();
            return Disciplinas![disciplinaProfessor].TemposLetivos;
        }
        public static List<string> ObterSalasDaDisciplinaProfessor(string disciplinaProfessor)
        {
            ValidarDisciplinasInicializacao();
            return Disciplinas![disciplinaProfessor].Salas;
        }

        public static string ObterSalaAleatoriaDaDisciplina(string disciplinaProfessor)
        {
            List<string> salasDaDisciplina = ObterSalasDaDisciplinaProfessor(disciplinaProfessor);
            return salasDaDisciplina[Random.Shared.Next(salasDaDisciplina.Count)];
        }
        public static bool TryCarregarDisciplinas()
        {
            string filePath = @"./DATA/disciplinas.csv";
            Disciplinas = null;

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

                    Disciplinas = DisciplinasDict;
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

        private static void ValidarDisciplinasInicializacao()
        {
            if (Disciplinas is null)
            {
                throw new NullReferenceException("Disciplinas não foram inicializados");
            }

        }
        public static bool TryCarregarSalas()
        {
            string filePath = @"./DATA/disciplinas_salas.csv";

            ValidarDisciplinasInicializacao();

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
                            TemposLetivos = Disciplinas![record.Sigla].TemposLetivos,
                            Salas = record.Salas,
                        };
                    });

                    Disciplinas = DisciplinasSalasDict;
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
