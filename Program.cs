using RealTimeAnalysis.Io;
using RealTimeAnalysis.Models;
using RealTimeAnalysis.Reporting;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string dataDir = Path.Combine(AppContext.BaseDirectory, "data");

SystemConfig system = DataLoader.LoadSystem(Path.Combine(dataDir, "system.json"));
List<TaskItem> tasks = DataLoader.LoadTasks(Path.Combine(dataDir, "tasks.csv"));

ConsoleReport.PrintTable(tasks);
ConsoleReport.PrintConclusion(tasks, system);

Console.WriteLine();
Console.WriteLine("Нажмите Enter для выхода...");
Console.ReadLine();