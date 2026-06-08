using System;
using System.Collections.Generic;
using System.Linq;
using TotoAnalyzerProject.Models;
using TotoAnalyzerProject.Services;

namespace TotoAnalyzerProject.UI
{
    internal class Menu
    {
        private readonly List<TotoDraw> allDraws;
        private int fromYear;
        private int toYear;

        public Menu(List<TotoDraw> allDraws)
        {
            this.allDraws = allDraws;
            this.fromYear = allDraws.Min(d => d.Year);
            this.toYear = allDraws.Max(d => d.Year);
        }

        public void Run()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("============================================");
                Console.WriteLine("            ТОТО АНАЛИЗАТОР");
                Console.WriteLine("============================================");
                Console.WriteLine($" Current period: {fromYear} - {toYear}");
                Console.WriteLine("[1] Избери период (от година - до година)");
                Console.WriteLine("[2] Топ N най-чести числа");
                Console.WriteLine("[3] Горещи двойки");
                Console.WriteLine("[4] Разпределение по десетици");
                Console.WriteLine("[5] Bar Chart");
                Console.WriteLine("[6] Heat Map");
                Console.WriteLine("[0] Изход");
                Console.WriteLine("============================================");
                Console.Write("Избор: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        SelectPeriod();
                        break;

                    case "2":
                        ShowTopNumbers();
                        break;

                    case "3":
                        ShowHotPairs();
                        break;

                    case "4":
                        ShowDistributionByTens();
                        break;

                    case "5":
                        ShowBarChart();
                        break;

                    case "6":
                        ShowHeatMap();
                        break;

                    case "0":
                        return;

                    default:
                        Console.WriteLine("Невалиден избор.");
                        WaitForKey();
                        break;
                }
            }
        }

        private IEnumerable<TotoDraw> GetFilteredDraws()
        {
            return allDraws.Where(d => d.Year >= fromYear && d.Year <= toYear);
        }

        private void SelectPeriod()
        {
            Console.Clear();

            int minYear = allDraws.Min(d => d.Year);
            int maxYear = allDraws.Max(d => d.Year);

            Console.WriteLine($"Въведи период между {minYear} и {maxYear}");

            int newFromYear = ReadInt("От година: ");
            int newToYear = ReadInt("До година: ");

            if (newFromYear > newToYear)
            {
                Console.WriteLine("Началната година не може да е по-голяма от крайната.");
                WaitForKey();
                return;
            }

            if (newFromYear < minYear || newToYear > maxYear)
            {
                Console.WriteLine("Периодът е извън наличните данни.");
                WaitForKey();
                return;
            }

            fromYear = newFromYear;
            toYear = newToYear;

            Console.WriteLine("Периодът беше обновен успешно.");
            WaitForKey();
        }

        private void ShowTopNumbers()
        {
            Console.Clear();
            Console.WriteLine("Статистика: Топ N най-чести числа");

            int n = ReadInt("Въведи N: ");

            Statistics statistics = new Statistics(GetFilteredDraws());
            var topNumbers = statistics.GetTopNumbers(n);

            foreach (var kvp in topNumbers)
            {
                Console.WriteLine($"{kvp.Key} -> {kvp.Value}");
            }

            WaitForKey();
        }

        private void ShowHotPairs()
        {
            Console.Clear();
            Console.WriteLine("Статистика: Горещи двойки");

            int n = ReadInt("Въведи N: ");

            Statistics statistics = new Statistics(GetFilteredDraws());
            var hotPairs = statistics.GetHotPairs(n);

            foreach (var pair in hotPairs)
            {
                Console.WriteLine($"{pair.Number1}, {pair.Number2} -> {pair.Count}");
            }

            WaitForKey();
        }

        private void ShowDistributionByTens()
        {
            Console.Clear();
            Console.WriteLine("Статистика: Разпределение по десетици");

            Statistics statistics = new Statistics(GetFilteredDraws());
            var distribution = statistics.GetDistributionByTens();

            string[] ranges = { "1-10", "11-20", "21-30", "31-40", "41-49" };

            foreach (string range in ranges)
            {
                if (distribution.ContainsKey(range))
                {
                    Console.WriteLine($"{range} -> {distribution[range]}");
                }
            }

            WaitForKey();
        }

        private void ShowBarChart()
        {
            Console.Clear();
            Console.WriteLine("Визуализация: Horizontal Bar Chart");

            int n = ReadInt("Въведи N: ");

            Statistics statistics = new Statistics(GetFilteredDraws());
            var topNumbers = statistics.GetTopNumbers(n);

            Visualizer visualizer = new Visualizer();
            visualizer.PrintBarChart(topNumbers);

            WaitForKey();
        }

        private void ShowHeatMap()
        {
            Console.Clear();
            Console.WriteLine("Визуализация: Heat Map 7x7");

            Visualizer visualizer = new Visualizer();
            visualizer.PrintHeatMap(GetFilteredDraws());

            WaitForKey();
        }

        private int ReadInt(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string? input = Console.ReadLine();

                if (int.TryParse(input, out int value))
                {
                    return value;
                }

                Console.WriteLine("Моля въведи валидно число.");
            }
        }

        private void WaitForKey()
        {
            Console.WriteLine();
            Console.WriteLine("Натисни клавиш за да се върнеш в менюто...");
            Console.ReadKey();
        }
    }
}
