using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using TotoAnalyzerProject.Models;
namespace TotoAnalyzerProject.Services
{
    public class DataLoader
    {
        private readonly HttpClient httpClient;
        public DataLoader(HttpClient client)
        {
            this.httpClient = client;
        }
        public async Task<string> GetPageContentAsync(string url)
        {
            string html = await httpClient.GetStringAsync(url);
            return html;
        }
        public async Task<List<string>> GetFilesUrlAsync()
        {
            string html = await GetPageContentAsync("https://info.toto.bg/statistika/6x49");
            List<string> urls = new List<string>();
            MatchCollection matches = Regex.Matches(html, "href=\"([^\"]+)\"");

            foreach (Match match in matches)
            {
                string link = match.Groups[1].Value;

                if (link.Contains("/content/files/stats-tiraji/"))
                {
                    string fullUrl = "https://info.toto.bg" + link;
                    urls.Add(fullUrl);
                }
            }

            return urls;
        }

        public List<string> ExtractFileUrls(string html)
        {
            List<string> urls = new List<string>();
            MatchCollection matches = Regex.Matches(html, "href=\"([^\"]+)\"");

            foreach (Match match in matches)
            {
                string link = match.Groups[1].Value.Trim();
                link = link.Replace(" ", "");

                if (link.Contains("content/files/stats-tiraji/"))
                {
                    string fullUrl = "https://info.toto.bg" + link;

                    if (!urls.Contains(fullUrl))
                    {
                        urls.Add(fullUrl);
                    }
                }
            }

            return urls;
        }

        public async Task<string> FileContentAsync(string url)
        {
            string content = await httpClient.GetStringAsync(url);
            return content;


        }
        public int ExtractYearFromUrl(string url)
        {
            int FileNameIndex = url.IndexOf("649");
            string FileName = url.Substring(FileNameIndex,url.Length - FileNameIndex);
            int FileNameExtensionIndex = FileName.IndexOf(".txt");
            string FileNameWithoutExtension = FileName.Substring(0,
                FileNameExtensionIndex);
            int yearIndex = FileNameWithoutExtension.IndexOf("_");     // 649_58
            string yearPart = FileNameWithoutExtension.Substring(yearIndex + 1,
                FileNameWithoutExtension.Length - yearIndex - 1);

            int shortYear = int.Parse(yearPart);

            if (shortYear >= 58)
            {
                return 1900 + shortYear;
            }

            return 2000 + shortYear;
        }

        public IEnumerable<TotoDraw> ParseTxtContent(string content,int year)
        {
            // 601,18,20,21,22,39,46   3,15,23,26,31,34
            string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            List<TotoDraw> TotoDraws = new List<TotoDraw>();
            foreach(string line in lines)
            { 
                MatchCollection matches = Regex.Matches(line, @"\d+");
                List<int> numbers = new List<int>();
                foreach(Match match in matches)
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
                    TotoDraws.Add(currentDraw);
                    combinationIndex++;

                }

            }
                
               
            return TotoDraws;
        }

        public void PrintTxtContent(IEnumerable<TotoDraw> draws,int year)
        {   
            Console.WriteLine($"Year:{year}");
            foreach(TotoDraw draw in draws)
            {
                Console.WriteLine($"Draw Number: {draw.DrawNumber}");
                Console.WriteLine($"Combination Index: {draw.CombinationIndex}");
                Console.WriteLine($"Winning numbers: {string.Join(", ", draw.WinningNumbers)}");
                Console.WriteLine();
            }
        }
    }
}

/*
 * Да се добави отделна подръжка за txt и docx
 * Да се погледнат вероятни повторения при Urls/fullUrl
 */