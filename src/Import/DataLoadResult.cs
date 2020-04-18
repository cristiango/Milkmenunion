using System;
using System.Collections.Generic;
using MilkmenUnion.Commands;

namespace MilkmenUnion.Import
{
    public class DataLoadResult
    {
        public IReadOnlyList<Issue> Issues { get; }
        public int LoadedEmployees { get; }
        public IReadOnlyCollection<ImportEmployee> Commands { get; }

        public DataLoadResult(IReadOnlyList<Issue> issues, int loadedEmployees, IReadOnlyCollection<ImportEmployee> commands)
        {
            Issues = issues?? Array.Empty<Issue>();
            LoadedEmployees = loadedEmployees;
            Commands = commands;
        }

        public override string ToString()
        {
            return $"# items loaded: {LoadedEmployees}, {nameof(Issues)}: {string.Join("/r/n", Issues)}";
        }

        public class Issue
        {
            public string FileName { get; }
            public string Message { get; }
            public string Position { get; }
            public string Content { get; }
            private const int MaxContentLength = 100;

            public Issue(string fileName, string message, string position, string content)
            {
                FileName = fileName;
                Message = message;
                Position = position;
                Content = content ?? "";
                if (Content.Length>MaxContentLength)
                {
                    Content = Content.Substring(0, MaxContentLength) + "...";
                }

            }

            public override string ToString()
            {
                return $"{Message}, At({Position}), {nameof(Content)}: {Content}";
            }
        }
    }
}