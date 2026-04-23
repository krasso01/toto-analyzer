using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TotoAnalyzerProject.Models;

namespace TotoAnalyzerProject.Parsers
{
    public class TxtParser
    {
        public IEnumerable<TotoDraw> ParseLegacyTxtContent(string content, int year)
        {
            // 601,18,20,21,22,39,46   3,15,23,26,31,34
            string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<TotoDraw> totoDraws = new List<TotoDraw>();
            foreach (string line in lines)
            {
                MatchCollection matches = Regex.Matches(line, @"\d+");
                List<int> numbers = new List<int>();
                foreach (Match match in matches)
                {
                    numbers.Add(int.Parse(match.Value));
                }
                if (numbers.Count < 7)
                {
                    continue;
                }
                int combinationIndex = 1;

                for (int i = 1; i < numbers.Count; i += 6)
                {
                    if (i + 5 >= numbers.Count)
                    {
                        break;
                    }
                    List<int> winningNumbers = new List<int>();
                    for (int j = i; j < i + 6; j++)
                    {
                        winningNumbers.Add(numbers[j]);
                    }
                    TotoDraw currentDraw = new TotoDraw();
                    currentDraw.Year = year;
                    currentDraw.DrawNumber = numbers[0];
                    currentDraw.CombinationIndex = combinationIndex;
                    currentDraw.WinningNumbers.AddRange(winningNumbers);
                    totoDraws.Add(currentDraw);
                    combinationIndex++;

                }

            }


            return totoDraws;
        }
        private IEnumerable<TotoDraw> ParseFormattedTxtContent(string content)
        {
            string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<TotoDraw> totoDraws = new List<TotoDraw>();
            foreach(string line in lines)
            {
                MatchCollection matches = Regex.Matches(line, @"\d+");
                if(matches.Count < 9)
                {
                    continue;
                }
                List<int> numbers = new List<int>();
                foreach(Match match in matches)
                {
                    
                    numbers.Add(int.Parse(match.Value));
                }
                TotoDraw currentDraw = new TotoDraw();
                currentDraw.DrawNumber = numbers[0];
                currentDraw.Year = numbers[1];
                currentDraw.CombinationIndex = numbers[2];
                List<int> winningNumbers = new List<int>();
                for (int i = 3; i < numbers.Count; i++)
                {
                    winningNumbers.Add(numbers[i]);
                }
                currentDraw.WinningNumbers.AddRange(winningNumbers);
                totoDraws.Add(currentDraw);
            }
            return totoDraws;
        }

        public IEnumerable<TotoDraw> ParseTxtContent(string content, int year)
        {
            if (IsFormattedTxt(content))
            {
                return ParseFormattedTxtContent(content);
            }

            return ParseLegacyTxtContent(content, year);
        }

        private bool IsFormattedTxt(string content)
        {
            return content.Contains("/2018") ||
           content.Contains("/2019") ||
           content.Contains("/2020") ||
           content.Contains("/2021");
        }

        public void PrintTxtContent(IEnumerable<TotoDraw> draws)
        {
            
            foreach (TotoDraw draw in draws)
            {
                Console.WriteLine($"Year: {draw.Year}");
                Console.WriteLine($"Draw Number: {draw.DrawNumber}");
                Console.WriteLine($"Combination Index: {draw.CombinationIndex}");
                Console.WriteLine($"Winning numbers: {string.Join(", ", draw.WinningNumbers)}");
                Console.WriteLine();
            }
        }

       
    }
}
