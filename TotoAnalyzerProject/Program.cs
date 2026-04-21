using System.Net.Http;
using System.Threading.Tasks;
using TotoAnalyzerProject.Services;
using TotoAnalyzerProject.Models;
using System.Linq;

namespace TotoAnalyzerProject
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            HttpClientHandler? handler = new HttpClientHandler()
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip |
                System.Net.DecompressionMethods.Deflate
            };
            HttpClient httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.Clear();
            httpClient.DefaultRequestHeaders.Add("User-Agent",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");
            httpClient.DefaultRequestHeaders.Add("Accept",
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,*/*;q=0.8");
            httpClient.DefaultRequestHeaders.Add("Accept-Language", "bg-BG,bg;q=0.9,en-US;q=0.8,en;q=0.7");


            try
            {
                DataLoader dataLoader = new DataLoader(httpClient);
                 string html = await dataLoader.GetPageContentAsync("https://info.toto.bg/statistika/6x49");


                List<string> fileUrls = await dataLoader.GetFilesUrlAsync();

                Console.WriteLine($"Found URLs: {fileUrls.Count}");
                if (fileUrls.Count == 0)
                {
                    Console.WriteLine("No file URLs found. The site may have returned a captcha or blocked the request.");
                    return;
                }
                string firstUrl = fileUrls[0];
                Console.WriteLine($"First URL: {firstUrl}");

                string fileContent = await dataLoader.FileContentAsync(firstUrl);
                int year = dataLoader.ExtractYearFromUrl(firstUrl);
                IEnumerable<TotoDraw> totoDraws = dataLoader.ParseTxtContent(fileContent, year);
                dataLoader.PrintTxtContent(totoDraws, year);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

        }
    }
    }

 