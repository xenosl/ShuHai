using System.Collections.Generic;

namespace ShuHai.XlsxToJson
{
    public class Workbook
    {
        public string Name { get; }

        public List<Sheet> Sheets { get; } = new List<Sheet>();

        public Workbook(string name) { Name = name; }
    }
}