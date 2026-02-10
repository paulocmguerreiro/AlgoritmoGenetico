using System.Collections.ObjectModel;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using HorarioEscolar.Estrutura;

namespace HorarioEscolar.Helper
{
    public static class HorarioHelper
    {
        private static Dictionary<string, HorarioCSV>? _horarios;
        private static List<string>? _horariosList;
        private static Dictionary<string, int>? _horariosIndexMap;
        public static readonly ReadOnlyCollection<string> DiasDaSemanaDescritivo = Array.AsReadOnly(new string[] { "Segunda", "Terça", "Quarta", "Quinta", "Sexta" });
        public static readonly ReadOnlyCollection<int> DiasDaSemanaIndex = Array.AsReadOnly(new int[] { 0, 1, 2, 3, 4 });

        public static ReadOnlyCollection<string> ObterHorasDoDia()
        {
            return _horariosList!.AsReadOnly();
        }
        public static int ObterHorarioIndicacaoDeTempoBloqueado(int diaDaSemana, string hora)
        {
            HorarioCSV horaAConsultar = _horarios![hora];
            return horaAConsultar.Dias[diaDaSemana];

        }
        public static int ObterIndexDaHoraAleatoriaDoDia(int duracaoTemposLetivos)
        {
            ValidarHorarioInicializacao();
            return Random.Shared.Next(_horarios!.Count - (duracaoTemposLetivos - 1));
        }
        public static string ObterHoraAPartirDoIndex(int horaInicioDaAulaIdx)
        {
            ValidarHorarioInicializacao();
            return _horarios!.ElementAt(horaInicioDaAulaIdx).Value.Hora;
        }

        public static string ObterHoraAleatoriaDoDia(int duracaoTemposLetivos)
        {
            return ObterHoraAPartirDoIndex(ObterIndexDaHoraAleatoriaDoDia(duracaoTemposLetivos));
        }
        public static string ObterHoraSeguinte(string horaInicial)
        {
            ValidarHorarioInicializacao();
            int posicaoNoDicionario = ObterIndexAPartirDaHora(horaInicial);
            return posicaoNoDicionario switch
            {
                -1 => horaInicial,
                _ when posicaoNoDicionario == _horarios!.Count - 1 => horaInicial,
                _ => ObterHoraAPartirDoIndex(posicaoNoDicionario + 1)
            };
        }

        public static string ObterHoraAnterior(string horaInicial)
        {
            ValidarHorarioInicializacao();
            int posicaoNoDicionario = ObterIndexAPartirDaHora(horaInicial);
            return posicaoNoDicionario <= 0 ? horaInicial : ObterHoraAPartirDoIndex(posicaoNoDicionario - 1);

        }

        public static int ObterIndexAPartirDaHora(string hora)
        {
            ValidarHorarioInicializacao();
            return _horariosIndexMap!.TryGetValue(hora, out int index) ? index : -1;

        }


        private static void ValidarHorarioInicializacao()
        {
            if (_horarios is null)
            {
                throw new NullReferenceException("Horários não foram inicializados");
            }
        }

        public static bool TryCarregarHorario()
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
