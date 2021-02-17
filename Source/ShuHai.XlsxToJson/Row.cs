using System.Collections.Generic;

namespace ShuHai.XlsxToJson
{
    public class Row
    {
        public int Index { get; }

        public List<Cell> Cells { get; } = new List<Cell>();

        public Row(int index) { Index = index; }
    }
}
