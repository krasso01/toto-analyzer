using TotoAnalyzerProject.Models;

namespace TotoAnalyzerProject.Services
{
    internal class Statistics
    {
        private readonly IEnumerable<TotoDraw> draws;

        public Statistics(IEnumerable<TotoDraw> draws)
        {
            this.draws = draws;
        }

        public Dictionary<int, int> GetTopNumbers(int n)
        {
            return draws
                .SelectMany(d => d.WinningNumbers)
                .GroupBy(number => number)
                .OrderByDescending(group => group.Count())
                .Take(n)
                .ToDictionary(group => group.Key, group => group.Count());
        }

        public List<(int Number1, int Number2, int Count)> GetHotPairs(int n)
        {
            return draws
                .SelectMany(draw =>
                {
                    List<(int, int)> pairs = new();

                    for (int i = 0; i < draw.WinningNumbers.Count; i++)
                    {
                        for (int j = i + 1; j < draw.WinningNumbers.Count; j++)
                        {
                            int first = Math.Min(draw.WinningNumbers[i], draw.WinningNumbers[j]);
                            int second = Math.Max(draw.WinningNumbers[i], draw.WinningNumbers[j]);
                            pairs.Add((first, second));
                        }
                    }

                    return pairs;
                })
                .GroupBy(pair => pair)
                .OrderByDescending(group => group.Count())
                .Take(n)
                .Select(group => (group.Key.Item1, group.Key.Item2, group.Count()))
                .ToList();
        }

        public Dictionary<string, int> GetDistributionByTens()
        {
            return draws
                .SelectMany(d => d.WinningNumbers)
                .GroupBy(number =>
                {
                    if (number >= 1 && number <= 10) return "1-10";
                    if (number >= 11 && number <= 20) return "11-20";
                    if (number >= 21 && number <= 30) return "21-30";
                    if (number >= 31 && number <= 40) return "31-40";
                    return "41-49";
                })
                .ToDictionary(group => group.Key, group => group.Count());
        }

    }
}
