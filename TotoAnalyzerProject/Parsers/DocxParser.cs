using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Text;
using TotoAnalyzerProject.Models;
using System.Text.RegularExpressions;

namespace TotoAnalyzerProject.Parsers
{
    public class DocxParser
    {
        public string ExtractTextFromDocx(string filePath)
        {
            StringBuilder sb = new StringBuilder();

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                Body? body = wordDoc.MainDocumentPart?.Document.Body;

                if (body == null)
                {
                    return string.Empty;
                }

                foreach (Paragraph paragraph in body.Descendants<Paragraph>())
                {
                    foreach (Text text in paragraph.Descendants<Text>())
                    {
                        sb.Append(text.Text);
                    }

                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
        public IEnumerable<TotoDraw> ParseExtractedDocxText(string content)
        {
            string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<TotoDraw> totoDraws = new();

            foreach(string line in lines)
            {
                List<int> numbers = new List<int>();
                string cleanLine = line.Trim();
                MatchCollection matches = Regex.Matches(cleanLine, @"\d+");

                foreach(Match match in matches)
                {
                    numbers.Add(int.Parse(match.Value));
                }
                if(numbers.Count != 9)
                {
                    continue;
                }
                TotoDraw currentDraw = new();
                currentDraw.DrawNumber = numbers[0];
                currentDraw.Year = numbers[1];
                currentDraw.CombinationIndex = numbers[2];
                List<int> winningNumbers = new List<int>();
                for (int i = 3; i < 9; i++)
                {
                    winningNumbers.Add(numbers[i]);
                }
                currentDraw.WinningNumbers.AddRange(winningNumbers);
                totoDraws.Add(currentDraw);
            }
            return totoDraws;
        }
        public void PrintDocxContent(IEnumerable<TotoDraw> draws)
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
