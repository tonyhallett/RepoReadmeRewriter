namespace RepoReadmeRewriter.ReadmeReplacement
{
    internal interface IReplacementParent : IReplacement
    {
        void AddChild(IReplacementParent child);
    }
}
