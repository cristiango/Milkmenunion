using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MilkmenUnion.Import
{
    /// <summary>
    /// Lets try without interfaces to implement a factory model
    /// </summary>
    public class FileImporter
    {
        private readonly ILogger<FileImporter> _logger;

        public delegate Task<DataLoadResult> Load(Stream file);

        public FileImporter(ILogger<FileImporter> logger)
        {
            _logger = logger;
        }

        public Load GetDataLoader(FileToImport file, CancellationToken ct = default) =>
            file.ContentType?.ToLower() switch
            {
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => (Load) (s =>
                    new XlsxEmployeesLoader().Load(s, ct)),
                "application/json" => (Load) (s=> new JsonEmployeesLoader().Load(s,ct)),
                
                _ => throw new NotSupportedException($"{file.ContentType} content type is not yet supported")
            };

        public async Task<DataLoadResult> ImportFiles(IReadOnlyList<FileToImport> filesToImport,
            CancellationToken ct = default)
        {
            var sw = Stopwatch.StartNew(); //poor man performance monitor. No time now to add proper metrics
            var issues = new List<DataLoadResult.Issue>();
            int newEmployees = 0;
            foreach (var fileToImport in filesToImport)
            {
                var loader = GetDataLoader(fileToImport, ct);
                await using var stream = fileToImport.OpenFile();
                var result = await loader(stream);
                issues.AddRange(result.Issues);

                _logger.LogDebug("Loaded file {file} containing #{items} new employees", fileToImport.FileName, result.LoadedEmployees);
                newEmployees += result.LoadedEmployees;
            }

            _logger.LogInformation("Finished importing from {files} files #{newEmployees} new employees in {duration}", filesToImport.Count, newEmployees, sw.Elapsed);
            return new DataLoadResult(issues, newEmployees);
        }
    }
}