using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace RepoReadmeRewriter.IOWrapper
{
    [ExcludeFromCodeCoverage]
    public class IOHelper : IIOHelper
    {
        public static IOHelper Instance { get; } = new IOHelper();

        public string CombinePaths(string path1, string path2) => Path.Combine(path1, path2);

        public bool FileExists(string filePath) => File.Exists(filePath);

        public string ReadAllText(string readmePath) => File.ReadAllText(readmePath);

        public string[] ReadAllLines(string filePath) => File.ReadAllLines(filePath);

        public void WriteAllText(string filePath, string contents)
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                _ = Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, contents);
        }

        public string EnsureAbsolute(string relativeDirectory, string absoluteOrRelativePath)
            => Path.IsPathRooted(absoluteOrRelativePath) ? absoluteOrRelativePath : Path.Combine(relativeDirectory, absoluteOrRelativePath);

        public bool DirectoryExists(string dir) => Directory.Exists(dir);

        public string? GetDirectoryName(string filePath) => Path.GetDirectoryName(filePath);
    }
}
