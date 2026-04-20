using System.Net.Http;
using System.Threading.Tasks;
using TotoAnalyzerProject.Services;
using TotoAnalyzerProject.Models;

namespace TotoAnalyzerProject
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClientHandler? handler = new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
            };
            HttpClient httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept",
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "bg-BG,bg;q=0.9,en-US;q=0.8,en;q=0.7");
            DataLoader dataLoader = new DataLoader(httpClient);

            List<string> fileUrls = await dataLoader.GetFilesUrlAsync();
         //   Console.WriteLine(string.Join("\n", fileUrls));
            string firstUrl = fileUrls[0];
            string fileContent = await dataLoader.FileContentAsync(firstUrl);
            //  string[] fileNumbers = fileContent.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            int year = dataLoader.ExtractYearFromUrl(firstUrl);
            IEnumerable<TotoDraw> totoDraws = dataLoader.ParseTxtContent(fileContent, year);
            dataLoader.PrintTxtContent(totoDraws,year);
           // Console.WriteLine($"First file URL: {firstUrl}");
           // Console.WriteLine(string.Join("\n",fileNumbers));
        }
    }
    }

 