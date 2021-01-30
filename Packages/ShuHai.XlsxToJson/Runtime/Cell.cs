namespace ShuHai.XlsxToJson
{
    public struct Cell
    {
        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }

        public object Value { get; set; }

        public Cell(int rowIndex, int columnIndex, object value)
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Value = value;
        }
    }
}
