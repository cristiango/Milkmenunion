using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MilkmenUnion.Storage;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace MilkmenUnion.Import
{
    /// <summary>
    /// Disclaimer: due to time constraints we choose to ignore possible big files and memory issues.
    /// We should come back to this and add some limits
    /// </summary>
    public class XlsxEmployeesLoader
    {
        private readonly string _fileName;

        public XlsxEmployeesLoader(string fileName)
        {
            _fileName = fileName;
        }

        public Task<DataLoadResult> Load(Stream file, CancellationToken ct = default)
        {
            var issues = new List<DataLoadResult.Issue>();
            var totalImported = 0;
            var commands = new List<Commands.ImportEmployee>();

            var workbook = new XSSFWorkbook(file);
            var sheet = workbook.GetSheetAt(workbook.ActiveSheetIndex);

            try
            {
                for (var i = 1; i <= sheet.LastRowNum; i++)
                {
                    ct.ThrowIfCancellationRequested();

                    var row = sheet.GetRow(i);
                    var line = ParseRow(row, GetColumns(sheet));
                    var rawRecord = string.Join(";", row.Cells.Select(c => c.ToString()));

                    //Happy flow scenario where we don't have to do data validation
                    if (!line.TryGetValue("Height", out string heightRaw))
                    {
                        issues.Add(new DataLoadResult.Issue(_fileName, "Missing value for height", i.ToString(), rawRecord));
                        continue;
                    }

                    if (!double.TryParse(heightRaw, out var height))
                    {
                        issues.Add(new DataLoadResult.Issue(_fileName, "Missing value for height", i.ToString(), rawRecord));
                        continue;
                    }

                    var importCommand = new Commands.ImportEmployee
                    {
                        Age = int.Parse(line.GetValueOrDefault("Age")), //all this should be constants somewhere
                        LastName = line.GetValueOrDefault("LastName"),
                        FirstName = line.GetValueOrDefault("FirstName"),
                        HeightInMeters = height
                    };

                    //Memory concerns here. But even if we merge with Walmart a normal sized server can handle 2Mil records in memory
                    //If this becomes an issue we can batch this. OR create some sort of producer/consumer model
                    commands.Add(importCommand);

                }
            }
            catch (Exception e)
            {
                throw new Exception("The XLSX file was invalid. " + e.Message,
                    e); //craft new exception Type `ImportException`
            }

            return Task.FromResult(new DataLoadResult(issues, totalImported, commands));
        }

        private static IEnumerable<string> GetColumns(ISheet sheet)
        {
            var firstRow = sheet.GetRow(0);
            for (var i = 0; i < firstRow.LastCellNum; i++)
            {
                yield return firstRow.GetCell(i, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString();
            }
        }

        private static IDictionary<string, string> ParseRow(IRow row, IEnumerable<string> headers)
        {
            var values = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            var i = 0;

            foreach (var header in headers)
            {
                values.Add(header, row.GetCell(i, MissingCellPolicy.CREATE_NULL_AS_BLANK).ToString());
                i++;
            }

            return values;
        }
    }

    public static class ImportExtensions
    {
        public static TT GetValueOrDefault<T, TT>(this IDictionary<T, TT> reader, T key)
            => reader.TryGetValue(key, out var value) ? value : default;
    }
}