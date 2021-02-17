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
                    while (reader.Read())
                    {
                        int rowIndex = 0;
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
            ColumnDefinition columnDefinition, string jsonPath, ICollection<Cell> invalidCells = null)
        {
            SheetToJsonFile(sheet, columnDefinition, false, jsonPath, invalidCells);
        }

        public static void SheetToJsonFile(Sheet sheet,
            ColumnDefinition columnDefinition, bool ignoreFirstRow, string jsonPath,
            ICollection<Cell> invalidCells = null)
        {
            var jsonObject = SheetToJson(sheet, columnDefinition, ignoreFirstRow, invalidCells);
            var jsonText = jsonObject.ToString(Formatting.Indented);
            File.WriteAllText(jsonPath, jsonText);
        }

        public static JArray SheetToJson(Sheet sheet,
            ColumnDefinition columnDefinition, bool ignoreFirstRow = false, ICollection<Cell> invalidCells = null)
        {
            Ensure.Argument.NotNull(sheet, nameof(sheet));
            Ensure.Argument.NotNull(columnDefinition, nameof(columnDefinition));

            var json = new JArray();
            foreach (var row in sheet.Rows)
            {
                if (ignoreFirstRow && row.Index == 0)
                    continue;

                var jsonRow = new JObject();
                foreach (var cell in row.Cells)
                {
                    var value = cell.Value;
                    if (!Index.IsValid(cell.ColumnIndex, columnDefinition.Count))
                    {
                        invalidCells?.Add(cell);
                        continue;
                    }

                    var def = columnDefinition[cell.ColumnIndex];
                    var jsonValue = ParseJsonValue(value, def.ValueType, def.FallbackValue);
                    if (jsonValue != null)
                        jsonRow[def.JsonKeyName] = jsonValue;
                    else
                        invalidCells?.Add(cell);
                }
                json.Add(jsonRow);
            }
            return json;
        }

        private static JValue ParseJsonValue(object value, CellValueType valueType, object fallbackValue)
        {
            switch (valueType)
            {
                case CellValueType.String:
                    switch (value)
                    {
                        case DBNull _:
                            return new JValue(fallbackValue);
                        case string strValue:
                            return new JValue(strValue);
                    }
                    break;
                case CellValueType.Integer:
                    switch (value)
                    {
                        case DBNull _:
                            return new JValue(fallbackValue);
                        case string strValue:
                            if (int.TryParse(strValue, out var i))
                                return new JValue(i);
                            break;
                        case int intValue:
                            return new JValue(intValue);
                    }
                    break;
                case CellValueType.Float:
                    switch (value)
                    {
                        case DBNull _:
                            return new JValue(fallbackValue);
                        case string strValue:
                            if (float.TryParse(strValue, out var f))
                                return new JValue(f);
                            break;
                        case int intValue:
                            return new JValue((double)intValue);
                        case float floatValue:
                            return new JValue(floatValue);
                        case double doubleValue:
                            return new JValue(doubleValue);
                    }
                    break;
                case CellValueType.Boolean:
                    switch (value)
                    {
                        case DBNull _:
                            return new JValue(fallbackValue);
                        case string strValue:
                            if (bool.TryParse(strValue, out var b))
                                return new JValue(b);
                            break;
                        case bool boolValue:
                            return new JValue(boolValue);
                    }
                    break;
                case CellValueType.Null:
                    switch (value)
                    {
                        case DBNull _:
                            return JValue.CreateNull();
                        case string strValue:
                            if (string.IsNullOrEmpty(strValue))
                                return JValue.CreateNull();
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(valueType), valueType, null);
            }
            return null;
        }
    }
}
