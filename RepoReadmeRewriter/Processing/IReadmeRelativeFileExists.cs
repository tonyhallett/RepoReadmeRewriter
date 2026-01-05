namespace RepoReadmeRewriter.Processing
{
    internal interface IReadmeRelativeFileExists
    {
        bool Exists(string relativePath);
    }
}
