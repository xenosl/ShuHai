using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using ExcelDataReader;
using ExcelNumberFormat;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ShuHai.XlsxToJson
{
    public static class Convert
    {
        /// <remarks>
        ///     实际使用时需要注意编码问题，可通过Encoding.RegisterProvider(CodePagesEncodingProvider.Instance)注册解码器。
        ///     CodePagesEncodingProvider不属于.NetStandard，所以无法在此库中内置调用。
        /// </remarks>
        public static Workbook ParseXlsxFile(string xlsxPath)
        {
            using (var stream = File.Open(xlsxPath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var fileName = Path.GetFileNameWithoutExtension(xlsxPath);
                var workbook = new Workbook(fileName);
                var sheets = workbook.Sheets;
                do
                {
                    var sheet = new Sheet(workbook, sheets.Count, reader.Name);
                    int rowIndex = 0;
                    while (reader.Read())
                    {
                        var row = new Row(rowIndex);
                        for (int colIndex = 0; colIndex < reader.FieldCount; ++colIndex)
                        {
                            var formatString = reader.GetNumberFormatString(colIndex);
                            var value = reader.GetValue(colIndex);
                            if (!ReferenceEquals(value, null) && formatString != null)
                                value = new NumberFormat(formatString).Format(value, CultureInfo.InvariantCulture);
                            row.Cells.Add(new Cell(rowIndex, colIndex, value));
                            Console.WriteLine($"[{rowIndex},{colIndex}] {value}");
                        }
                        sheet.Rows.Add(row);
                        ++rowIndex;
                    }
                    sheets.Add(sheet);
                } while (reader.NextResult());
                return workbook;
            }
        }

        public static void SheetToJsonFile(Sheet sheet,
            SheetParser parser, string jsonPath, ICollection<Cell> notParsedCells = null)
        {
            SheetToJsonFile(sheet, parser, false, jsonPath, notParsedCells);
        }

        public static void SheetToJsonFile(Sheet sheet,
            SheetParser parser, bool ignoreFirstRow, string jsonPath,
            ICollection<Cell> notParsedCells = null)
        {
            var jsonObject = SheetToJson(sheet, parser, ignoreFirstRow, notParsedCells);
            var jsonText = jsonObject.ToString(Formatting.Indented);
            File.WriteAllText(jsonPath, jsonText);
        }

        public static JArray SheetToJson(Sheet sheet,
            SheetParser parser, bool ignoreFirstRow = false, ICollection<Cell> notParsedCells = null)
        {
            Ensure.Argument.NotNull(sheet, nameof(sheet));
            Ensure.Argument.NotNull(parser, nameof(parser));

            var json = new JArray();
            foreach (var row in sheet.Rows)
            {
                if (ignoreFirstRow && row.Index == 0)
                    continue;

                var jsonRow = new JObject();
                foreach (var cell in row.Cells)
                {
                    var value = cell.Value;
                    if (!Index.IsValid(cell.ColumnIndex, parser.ColumnCount))
                    {
                        notParsedCells?.Add(cell);
                        continue;
                    }

                    var columnParser = parser[cell.ColumnIndex];
                    var jsonValue = columnParser.Parse(value);
                    if (jsonValue != null)
                        jsonRow[columnParser.Name] = jsonValue;
                    else
                        notParsedCells?.Add(cell);
                }
                json.Add(jsonRow);
            }
            return json;
        }
    }
}
