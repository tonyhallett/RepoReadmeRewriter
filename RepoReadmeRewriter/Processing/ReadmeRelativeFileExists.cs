using System.IO;
using RepoReadmeRewriter.Repo;

namespace RepoReadmeRewriter.Processing
{
    internal sealed class ReadmeRelativeFileExists(string repoDirectoryPath, string readmeDirectoryPath) : IReadmeRelativeFileExists
    {
        public string RepoDirectoryPath { get; } = repoDirectoryPath;

        public string ReadmeDirectoryPath { get; } = readmeDirectoryPath;

        public bool Exists(string relativePath) => File.Exists(GetPath(relativePath));

        private static string NormalizeDirectorySeparators(string path) => path
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);

        private string GetPath(string relativePath) => RepoRelative.RelativePathIsRepoRelative(relativePath)
                ? Path.Combine(RepoDirectoryPath, relativePath.TrimStart(RepoRelative.Char))
                : Path.Combine(ReadmeDirectoryPath, NormalizeDirectorySeparators(relativePath));
    }
}
