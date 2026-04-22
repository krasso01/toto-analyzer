using System.Net.Http;
using System.Threading.Tasks;
using TotoAnalyzerProject.Services;
using TotoAnalyzerProject.Models;
using TotoAnalyzerProject.Parsers;
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


                DataLoader dataLoader = new DataLoader(httpClient);
                string html = await dataLoader.GetPageContentAsync("https://info.toto.bg/statistika/6x49");
                TxtParser txtParser = new();

                List<string> fileUrls = dataLoader.ExtractFileUrls(html);
                Console.WriteLine($"Found URLs: {fileUrls.Count}");

                if (fileUrls.Count == 0)
                {
                    Console.WriteLine("No file URLs found. The site may have returned a captcha or blocked the request.");
                    return;
                }

            List<string> txtUrls = fileUrls
            .Where(url => url.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            .ToList();

            List<TotoDraw> allDraws = new();

            foreach(string txtUrl in txtUrls)
            {
                try
                {
                    string fileContent = await dataLoader.FileContentAsync(txtUrl);
                    int year = dataLoader.ExtractYearFromUrl(txtUrl);

                    IEnumerable<TotoDraw> currentDraws = txtParser.ParseTxtContent(fileContent,year);
                    allDraws.AddRange(currentDraws);

                    Console.WriteLine($"Successfully parsed TXT file for year {year}");
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Failed to process TXT file: {txtUrl}");
                    Console.WriteLine(ex.Message);
                }
                Console.WriteLine();
                Console.WriteLine($"Total TXT files parsed: {txtUrls.Count}");
                Console.WriteLine($"Total parsed draw entries: {allDraws.Count}");
                Console.WriteLine();

                txtParser.PrintTxtContent(allDraws.Take(5), allDraws.FirstOrDefault().Year);
            }

            //  allDraws.FirstOrDefault()?.Year ?? 0


            //   IEnumerable<TotoDraw> totoDraws = dataLoader.ParseTxtContent(fileContent, year);
            //   dataLoader.PrintTxtContent(totoDraws, year);





        }
    }
    }

 