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

            if (shortYear >= 58 && shortYear <= 99)
            {
                return 1900 + shortYear;
            }
            else if (shortYear >= 0 && shortYear <= 4)
            {
                return 2000 + shortYear;
            }
            else return shortYear;   
        }

       
    }
}

/*
 * Да се добави отделна подръжка за txt и docx
 * Да се погледнат вероятни повторения при Urls/fullUrl
 */