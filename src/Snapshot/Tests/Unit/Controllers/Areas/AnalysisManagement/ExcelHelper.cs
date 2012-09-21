using System.Collections.Generic;

namespace Tests.Unit.Controllers.Areas.AnalysisManagement
{
    public class ExcelHelper
    {

        public readonly string ExcelContentType = "application/vnd.xls";

        public List<List<string>> ParseExcel(string content)
        {
            string[] split = content.Split('\n');

            List<List<string>> parsed=new List<List<string>>();

            foreach (var l in split)
            {
                List<string> line=new List<string>();

                foreach (string c in l.Split('\t'))
                {
                    line.Add(c.Trim());
                }

                parsed.Add(line);
            }

            return parsed;
        }
    }
}