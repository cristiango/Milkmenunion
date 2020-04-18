using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MilkmenUnion.Commands;
using MilkmenUnion.Commands.Infra;
using MilkmenUnion.Domain;
using MilkmenUnion.Storage;

namespace MilkmenUnion.Import
{
    /// <summary>
    /// Lets try without interfaces to implement a factory model
    /// </summary>
    public class FileImporter
    {
        private readonly ILogger<FileImporter> _logger;
        private readonly EmployeesRepository _repository;

        private readonly ImportEmployeeHandler _importEmployeeHandler;

        public delegate Task<DataLoadResult> Load(Stream file);

        public FileImporter(ILogger<FileImporter> logger, EmployeesRepository repository, ImportEmployeeHandler importEmployeeHandler,)
        {
            _logger = logger;
            _repository = repository;
            _importEmployeeHandler = importEmployeeHandler;
        }

        public Load GetDataLoader(FileToImport file, CancellationToken ct = default) =>
            file.ContentType?.ToLower() switch
            {
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" => (Load) (s =>
                    new XlsxEmployeesLoader(file.FileName).Load(s, ct)),
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
                var parseResult = await loader(stream);
                issues.AddRange(parseResult.Issues);

                _logger.LogDebug("Loaded file {file} containing #{items} new employees", fileToImport.FileName, parseResult.LoadedEmployees);

                //push to the repo
                //Performance Alert!! Consider doing a bulk INSERT for large datasets
                foreach (var command in parseResult.Commands)
                {
                    var (status, _) = await _importEmployeeHandler.Handle(command, ct);
                    if (status.Success)
                    {
                        newEmployees++; //this is not all that true since create command is idempotent
                                        //and we cannot tell yet if new employee is created or this is a noop. Later on improve this
                    }
                }
            }

            //TODO create commandHandler for this operation with retry Policy see ImportEmployeeHandler example
            await _repository.CalculateInitialSalaryForImportedEmployees(ct);

            _logger.LogInformation("Finished importing from {files} files #{newEmployees} new employees in {duration}", filesToImport.Count, newEmployees, sw.Elapsed);
            return new DataLoadResult(issues, newEmployees, null);
        }
    }
}