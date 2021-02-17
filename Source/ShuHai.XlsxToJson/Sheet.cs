using System.Collections.Generic;

namespace ShuHai.XlsxToJson
{
    public class Sheet
    {
        public Workbook Owner { get; }

        public int Index { get; }

        public string Name { get; }

        public List<Row> Rows { get; } = new List<Row>();

        public Sheet(Workbook owner, int index, string name)
        {
            Ensure.Argument.NotNull(owner, nameof(owner));
            Ensure.Argument.NotNullOrEmpty(name, nameof(name));

            Owner = owner;
            Index = index;
            Name = name;
        }
    }
}
