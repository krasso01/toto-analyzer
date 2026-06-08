using System;
using System.Collections.Generic;
using System.Linq;

namespace TotoAnalyzerProject.Services
{
    internal class Visualizer
    {
        public void PrintBarChart(Dictionary<int, int> topNumbers)
        {
            if (topNumbers == null || topNumbers.Count == 0)
            {
                Console.WriteLine("No data available.");
                return;
            }

            int maxValue = topNumbers.Values.Max();
            int maxBarWidth = 30;

            foreach (var item in topNumbers)
            {
                int barLength = (int)Math.Round((double)item.Value / maxValue * maxBarWidth);
                string bar = new string('#', barLength);

                Console.WriteLine($"{item.Key,2} | {bar,-30} {item.Value}");
            }
        }

        public void PrintHeatMap(IEnumerable<Models.TotoDraw> draws)
        {
            Dictionary<int, int> numberCounts = draws
                .SelectMany(draw => draw.WinningNumbers)
                .GroupBy(number => number)
                .ToDictionary(group => group.Key, group => group.Count());

            List<int> allCounts = numberCounts.Values.OrderBy(value => value).ToList();

            if (allCounts.Count == 0)
            {
                Console.WriteLine("No data available.");
                return;
            }

            int lowThreshold = allCounts[(int)(allCounts.Count * 0.3)];
            int highThreshold = allCounts[(int)(allCounts.Count * 0.7)];

            for (int number = 1; number <= 49; number++)
            {
                int count = numberCounts.ContainsKey(number) ? numberCounts[number] : 0;

                if (count >= highThreshold)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else if (count >= lowThreshold)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }

                Console.Write($"{number,2} ");
                Console.ResetColor();

                if (number % 7 == 0)
                {
                    Console.WriteLine();
                }
            }

            Console.WriteLine();
        }
    }
}