namespace RepoReadmeRewriter.ReadmeReplacement
{
    internal interface IReplacement
    {
        int Start { get; }

        int End { get; }

        string GetReplacement();
    }
}
