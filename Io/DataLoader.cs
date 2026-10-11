using RealTimeAnalysis.Logic;
using RealTimeAnalysis.Models;
using System.Globalization;
using System.Text.Json;

namespace RealTimeAnalysis.Io
{
    /// <summary>
    /// Чтение исходных данных варианта из файлов CSV и JSON.
    /// </summary>
    public static class DataLoader
    {
        public static SystemConfig LoadSystem(string path)
        {
            using var doc = JsonDocument.Parse(File.ReadAllText(path));
            var root = doc.RootElement;
            return new SystemConfig(
                root.GetProperty("cpuCount").GetInt32(),
                root.GetProperty("hasNetwork").GetBoolean());
        }

        public static List<TaskItem> LoadTasks(string path)
        {
            var result = new List<TaskItem>();
            var lines = File.ReadAllLines(path)
                            .Where(l => !string.IsNullOrWhiteSpace(l))
                            .Skip(1); // пропускаем заголовок

            foreach (var line in lines)
            {
                var parts = line.Split(';');
                result.Add(new TaskItem(
                    Name: parts[0].Trim(),
                    Hardness: ParseHardness(parts[1]),
                    C: ParseNumber(parts[2]),
                    D: ParseNumber(parts[3]),
                    T: ParseNumber(parts[4])));
            }
            return result;
        }

        private static Hardness ParseHardness(string s) => s.Trim().ToLowerInvariant() switch
        {
            "жёсткое" or "жесткое" => Hardness.Hard,
            "твёрдое" or "твердое" => Hardness.Firm,
            "мягкое" => Hardness.Soft,
            _ => throw new FormatException($"Неизвестный класс жёсткости: '{s}'")
        };

        private static double ParseNumber(string s) =>
            double.Parse(s.Trim().Replace(',', '.'), CultureInfo.InvariantCulture);
    }
}
