using System.Net.Http;
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
            HttpClientHandler handler = new HttpClientHandler()
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
            TxtParser txtParser = new TxtParser();
            DocxParser docxParser = new DocxParser();

            string html = await dataLoader.GetPageContentAsync("https://info.toto.bg/statistika/6x49");
            List<string> fileUrls = dataLoader.ExtractFileUrls(html);

            if (fileUrls.Count == 0)
            {
                Console.WriteLine("No file URLs found.");
                return;
            }

            List<TotoDraw> allDraws = new();

            foreach (string fileUrl in fileUrls)
            {
                try
                {
                    if (fileUrl.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        string fileContent = await dataLoader.FileContentAsync(fileUrl);
                        int year = dataLoader.ExtractYearFromUrl(fileUrl);

                        IEnumerable<TotoDraw> txtDraws = txtParser.ParseTxtContent(fileContent, year);
                        allDraws.AddRange(txtDraws);
                    }
                    else if (fileUrl.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
                    {
                        byte[] docxBytes = await dataLoader.DownloadFileBytesAsync(fileUrl);
                        string tempFilePath = Path.Combine(Path.GetTempPath(), "toto_temp.docx");
                        await File.WriteAllBytesAsync(tempFilePath, docxBytes);

                        string extractedText = docxParser.ExtractTextFromDocx(tempFilePath);
                        IEnumerable<TotoDraw> docxDraws = docxParser.ParseExtractedDocxText(extractedText);
                        allDraws.AddRange(docxDraws);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to process file: {fileUrl}");
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine($"Total draws loaded: {allDraws.Count}");

            Statistics statistics = new Statistics(allDraws);

            var topNumbers = statistics.GetTopNumbers(10);
            Console.WriteLine("Top 10 numbers:");
            foreach (var kvp in topNumbers)
            {
                Console.WriteLine($"{kvp.Key} -> {kvp.Value}");
            }
            var hotPairs = statistics.GetHotPairs(10);
            Console.WriteLine();
            Console.WriteLine("Top 10 hot pairs:");
            foreach (var pair in hotPairs)
            {
                Console.WriteLine($"{pair.Number1}, {pair.Number2} -> {pair.Count}");
            }

            var tensDistribution = statistics.GetDistributionByTens();
            Console.WriteLine();
            Console.WriteLine("Distribution by tens:");
            string[] ranges = { "1-10", "11-20", "21-30", "31-40", "41-49" };

            foreach (string range in ranges)
            {
                if (tensDistribution.ContainsKey(range))
                {
                    Console.WriteLine($"{range} -> {tensDistribution[range]}");
                }
            }
        }
    }
}

