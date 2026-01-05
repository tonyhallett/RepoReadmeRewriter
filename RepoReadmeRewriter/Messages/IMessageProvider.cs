namespace RepoReadmeRewriter.Messages
{
    internal interface IMessageProvider
    {
        string UnsupportedImageDomain(string imageDomain);

        string ReadmeFileDoesNotExist(string readmeFilePath);

        string CouldNotParseRepositoryUrl(string? repositoryUrl);

        string ReadmeHasUnsupportedHTML();

        string MissingReadmeAsset(string missingReadmeAsset);

        string CannotFindGitRepository();
    }
}
