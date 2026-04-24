using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

                foreach (Text text in body.Descendants<Text>())
                {
                    sb.Append(text.Text);
                    sb.Append(' ');
                }
            }

            return sb.ToString();
        }
    }
}
