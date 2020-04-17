using System;
using System.IO;

namespace MilkmenUnion.Import
{
    public class FileToImport
    {
        public string FileName { get; }
        public Func<Stream> OpenFile { get; }
        public string ContentType { get; }

        public FileToImport(string fileName, Func<Stream> openFile, string contentType)
        {
            FileName = fileName;
            OpenFile = openFile ?? throw new ArgumentNullException(nameof(openFile));
            ContentType = contentType;
        }

    }
}