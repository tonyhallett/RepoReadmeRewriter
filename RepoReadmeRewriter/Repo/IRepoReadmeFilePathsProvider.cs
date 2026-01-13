namespace RepoReadmeRewriter.Repo
{
    internal interface IRepoReadmeFilePathsProvider
    {
        RepoReadmeFilePaths? Provide(string readmePath);
    }
}
