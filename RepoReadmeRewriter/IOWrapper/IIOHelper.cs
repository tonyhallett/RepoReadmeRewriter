namespace RepoReadmeRewriter.IOWrapper
{
    public interface IIOHelper
    {
        bool FileExists(string filePath);

        string CombinePaths(string path1, string path2);

        string ReadAllText(string readmePath);

        string[] ReadAllLines(string filePath);

        void WriteAllText(string filePath, string contents);

        string EnsureAbsolute(string relativeDirectory, string absoluteOrRelativePath);

        bool DirectoryExists(string dir);

        string? GetDirectoryName(string filePath);
    }
}
