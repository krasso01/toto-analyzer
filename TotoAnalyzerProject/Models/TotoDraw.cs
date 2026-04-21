using System.Collections.Generic;


namespace TotoAnalyzerProject.Models
{
    public class TotoDraw
    {
        public int Year { get; set; }

        public int DrawNumber { get; set; }
        public int CombinationIndex { get; set; }

        public List<int> WinningNumbers { get; set; } = new();



    }
}
