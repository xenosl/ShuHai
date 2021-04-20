using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ShuHai.XlsxToJson;

namespace ShuHai.I18N.XlsxToJson
{
    using XlsxToJsonConvert = ShuHai.XlsxToJson.Convert;

    /// <summary>
    ///     <para>该程序将包含关键字到对应文本映射的Excel表格文件转换为程序运行时需要读取的json文件。</para>
    ///     <para>
    ///         转换规则：
    ///         <para>单个文件中的每个表格都对应一个json文件。</para>
    ///         <para>每个表格中的第一列为关键字名字，第二列为对应的本地化文本。</para>
    ///     </para>
    /// </summary>
    internal class Program
    {
        private const string XlsxDirectoryName = "Xlsx";
        private const string JsonDirectoryName = "Json";

        private static void Main()
        {
            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                var xlsxDir = Path.Combine(Environment.CurrentDirectory, XlsxDirectoryName);
                var jsonDir = Path.Combine(Environment.CurrentDirectory, JsonDirectoryName);
                Directory.CreateDirectory(jsonDir);

                var xlsxFiles = EnumerateXlsxFiles(xlsxDir);
                foreach (var xlsxPath in xlsxFiles)
                {
                    Console.WriteLine($"Convert file '{xlsxPath}'.");

                    var workbook = XlsxToJsonConvert.ParseXlsxFile(xlsxPath);
                    foreach (var sheet in workbook.Sheets)
                    {
                        Console.WriteLine($"Convert Sheet '{sheet.Name}'.");

                        var path = Path.Combine(jsonDir, MakeJsonFileName(workbook.Name, sheet.Name));
                        var invalidCells = new List<Cell>();
                        XlsxToJsonConvert.SheetToJsonFile(sheet, _sheetParser, true, path, invalidCells);

                        if (invalidCells.Count > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            var invalidCellTexts = invalidCells
                                .Select(c => $"Row={c.RowIndex}, Column={c.ColumnIndex}");
                            Console.WriteLine("Error when parsing cell:");
                            Console.WriteLine(string.Join('\n', invalidCellTexts));
                            Console.ResetColor();
                        }
                    }
                }

                Console.WriteLine("Convert Done.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static readonly SheetParser _sheetParser = new SheetParser(new[]
        {
            ("Key", (CellParser)new StringCellParser()),
            ("Text", new StringCellParser()),
        });

        private static string MakeJsonFileName(string workbookName, string sheetName)
        {
            return $"{workbookName}-{sheetName}" + ".json";
        }

        private static IEnumerable<string> EnumerateXlsxFiles(string dir)
        {
            return Directory.EnumerateFiles(dir, "*.xlsx");
        }
    }
}
