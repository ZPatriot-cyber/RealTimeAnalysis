using System.Globalization;
using RealTimeAnalysis.Logic;
using RealTimeAnalysis.Models;

namespace RealTimeAnalysis.Reporting
{
    /// <summary>
    /// Вывод таблицы результатов и текстового заключения в консоль.
    /// </summary>
    public static class ConsoleReport
    {
        public static void PrintTable(List<TaskItem> tasks)
        {
            string[] headers = { "Задача", "Класс", "C", "D", "T", "L", "Выполнимо", "Тип дедлайна" };

            var rows = tasks.Select(t => new[]
            {
            t.Name,
            RealTimeCalculator.HardnessName(t.Hardness),
            Fmt(t.C),
            Fmt(t.D),
            Fmt(t.T),
            Fmt(RealTimeCalculator.Slack(t)),
            RealTimeCalculator.IsFeasible(t) ? "да" : "нет",
            RealTimeCalculator.DeadlineType(t)
        }).ToList();

            var widths = headers
                .Select((h, i) => Math.Max(h.Length, rows.Max(r => r[i].Length)))
                .ToArray();

            string FormatRow(IEnumerable<string> cells) =>
                string.Join(" | ", cells.Select((c, i) => c.PadRight(widths[i])));

            Console.WriteLine("=== Таблица задач (времена в мс) ===");
            Console.WriteLine(FormatRow(headers));
            Console.WriteLine(string.Join("-+-", widths.Select(w => new string('-', w))));
            foreach (var row in rows)
                Console.WriteLine(FormatRow(row));
            Console.WriteLine();
        }

        public static void PrintConclusion(List<TaskItem> tasks, SystemConfig sys)
        {
            var critical = RealTimeCalculator.CriticalTask(tasks);
            double u = RealTimeCalculator.Utilization(tasks);
            bool allFeasible = tasks.All(RealTimeCalculator.IsFeasible);

            Console.WriteLine("=== Заключение ===");
            Console.WriteLine($"Класс системы: {RealTimeCalculator.ClassifySystem(tasks)}");
            Console.WriteLine($"Архитектура: {RealTimeCalculator.ClassifyArchitecture(sys.CpuCount, sys.HasNetwork)} " +
                              $"(процессоров: {sys.CpuCount}, сеть: {(sys.HasNetwork ? "есть" : "нет")})");

            Console.WriteLine(critical is null
                ? "Критическая задача: жёстких задач нет"
                : $"Критическая задача: «{critical.Name}» (L = {Fmt(RealTimeCalculator.Slack(critical))} мс)");

            Console.WriteLine($"R_треб = {Fmt(RealTimeCalculator.RequiredReactionTime(tasks))} мс");
            Console.WriteLine($"Коэффициент загрузки U = {u.ToString("0.00", CultureInfo.InvariantCulture)}");
            Console.WriteLine(u <= 1
                ? "Необходимое условие U ≤ 1 выполнено"
                : "U > 1: дедлайны заведомо невыполнимы");

            Console.WriteLine();
            if (allFeasible && u <= 1)
                Console.WriteLine("Вывод: все дедлайны по запасу выполнимы, загрузка допустима. " +
                                  "Условие U ≤ 1 необходимо, но не достаточно для гарантии планируемости.");
            else
                Console.WriteLine("Вывод: есть задачи, которые не укладываются в дедлайн (L < 0).");
        }

        private static string Fmt(double value) =>
            value.ToString("0.###", CultureInfo.InvariantCulture);
    }
}
