using System.Diagnostics.CodeAnalysis;

namespace RepoReadmeRewriter.Messages
{
    [ExcludeFromCodeCoverage]
    public class MessageProvider : IMessageProvider
    {
        public virtual string CannotFindGitRepository() => "Cannot find Git repository";

        public virtual string CouldNotParseRepositoryUrl(string? repositoryUrl)
            => $"Could not parse the repository url :{repositoryUrl}";

        public virtual string MissingReadmeAsset(string missingReadmeAsset) => $"Missing readme asset - {missingReadmeAsset}";

        public virtual string ReadmeFileDoesNotExist(string readmeFilePath) => $"Readme file does not exist at '{readmeFilePath}'";

        public virtual string ReadmeHasUnsupportedHTML() => "Readme has unsupported HTML";

        public virtual string UnsupportedImageDomain(string imageDomain) => $"Unsupported image domain found in README: {imageDomain}";
    }
}
