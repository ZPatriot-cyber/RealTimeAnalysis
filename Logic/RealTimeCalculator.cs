using System;
using System.Collections.Generic;
using System.Text;
using RealTimeAnalysis.Models;


namespace RealTimeAnalysis.Logic
{
    /// Расчёты классификации и временных характеристик задач реального времени.
    public static class RealTimeCalculator
    {
        // Запас: L = D − C
        public static double Slack(TaskItem task) => task.D - task.C;

        // Признак выполнимости дедлайна: L ≥ 0
        public static bool IsFeasible(TaskItem task) => Slack(task) >= 0;

        // Тип дедлайна по соотношению D и T
        public static string DeadlineType(TaskItem task) =>
            task.D == task.T ? "неявный"
            : task.D < task.T ? "ограниченный"
            : "произвольный";

        // Класс системы: жёсткого РВ, если есть хотя бы одна жёсткая задача, иначе мягкого
        public static string ClassifySystem(IEnumerable<TaskItem> tasks) =>
            tasks.Any(t => t.Hardness == Hardness.Hard)
                ? "жёсткого реального времени"
                : "мягкого реального времени";

        // Критическая задача: наименьший запас среди жёстких
        public static TaskItem? CriticalTask(IEnumerable<TaskItem> tasks) =>
            tasks.Where(t => t.Hardness == Hardness.Hard)
                 .OrderBy(Slack)
                 .FirstOrDefault();

        // R_треб = min(D) по жёстким задачам, при их отсутствии — по всем
        public static double RequiredReactionTime(IEnumerable<TaskItem> tasks)
        {
            var list = tasks.ToList();
            var hard = list.Where(t => t.Hardness == Hardness.Hard).ToList();
            var pool = hard.Count > 0 ? hard : list;
            return pool.Min(t => t.D);
        }

        // Коэффициент загрузки U = Σ(C / T)
        public static double Utilization(IEnumerable<TaskItem> tasks) =>
            tasks.Sum(t => t.C / t.T);

        // Архитектурный класс по числу процессоров и наличию сети
        public static string ClassifyArchitecture(int cpuCount, bool hasNetwork) =>
            cpuCount <= 1 ? "однопроцессорная"
            : hasNetwork ? "распределённая"
            : "многопроцессорная";

        public static string HardnessName(Hardness h) => h switch
        {
            Hardness.Hard => "жёсткое",
            Hardness.Firm => "твёрдое",
            Hardness.Soft => "мягкое",
            _ => throw new ArgumentOutOfRangeException(nameof(h))
        };
    }
}
