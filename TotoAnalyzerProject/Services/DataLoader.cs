using System.Net.Http;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
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
            List<string> urls = new();
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
        public async Task<string> FileContentAsync(string url)
        {
            string content = await httpClient.GetStringAsync(url);
            return content;


        }
    }
}

/*
 * Да се добави отделна подръжка за txt и docx
 * Да се погледнат вероятни повторения при Urls/fullUrl
 */