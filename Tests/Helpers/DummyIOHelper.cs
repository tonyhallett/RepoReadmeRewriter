using NugetRepoReadme.IOWrapper;

namespace Tests.Helpers
{
    internal sealed class DummyIOHelper : IIOHelper
    {
        public const string ReadmeText = "readme";

        public bool DoesFileExist { get; set; }

        public string CombinePaths(string path1, string path2) => $"{path1};{path2}";

        public string? FileExistsPath { get; private set; }

        public bool FileExists(string filePath)
        {
            FileExistsPath = filePath;
            return DoesFileExist;
        }

        public string ReadAllText(string readmePath) => ReadmeText;

        public string[] ReadAllLines(string filePath) => throw new NotImplementedException();
    }
}
