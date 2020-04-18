using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using MilkmenUnion.Import;

namespace MilkmenUnion.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RequestSizeLimit(RequestLimitSize)]
    public class ImportController: ControllerBase
    {
        public const long RequestLimitSize = 10_000_000_000;

        private readonly FileImporter _fileImporter;

        public ImportController(FileImporter fileImporter)
        {
            _fileImporter = fileImporter;
        }

        [HttpPost]
        public async Task<IActionResult> Import(CancellationToken ct = default)
        {
            var (result, processedFiles) =
                Request.ContentType.StartsWith("multipart/form-data", StringComparison.OrdinalIgnoreCase)
                    ? await ProcessMultiPartFormUpload(ct)
                    : await ProcessUpload(ct);

            return Ok(new ImportResult
            {
                Imported = result.LoadedEmployees,
                Issues = result.Issues.Select(x => new ImportResult.Issue
                    {
                        Content = x.Content,
                        Message = x.Message,
                        Position = x.Position,
                        FileName = x.FileName
                    })
                    .ToArray(),
                ProcessedFiles = processedFiles,
            });
        }

        private async Task<(DataLoadResult result, string[] processedFiles)> ProcessMultiPartFormUpload(CancellationToken ct)
        {
            var filesToImport = Request.Form.Files
                .Select(x => new FileToImport(x.FileName, x.OpenReadStream, x.ContentType))
                .ToArray();

            return (
                await _fileImporter.ImportFiles(filesToImport, ct),
                Request.Form.Files.Select(x => x.FileName).ToArray());
        }

        private async Task<(DataLoadResult result, string[] processedFiles)> ProcessUpload(
            CancellationToken ct)
        {
            string fileName = Request.Headers["content-disposition"];
            if (ContentDispositionHeaderValue.TryParse(new StringSegment(Request.Headers["content-disposition"]), out var header))
            {
                fileName = header.FileName.ToString()?.Trim('\"');
            }

            var file = new FileToImport(fileName,
                () => Request.Body,
                Request.ContentType);

            return
                (await _fileImporter.ImportFiles(new[] { file }, ct),
                    null);
        }
    }

    public class ImportResult
    {
        public object Issues { get; set; }
        public string[] ProcessedFiles { get; set; }
        public int Imported { get; set; }

        public class Issue
        {
            public string Content { get; set; }
            public string Message { get; set; }
            public string Position { get; set; }
            public string FileName { get; set; }
        }
    }
}
