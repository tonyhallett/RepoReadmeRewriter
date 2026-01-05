using System.Diagnostics.CodeAnalysis;

namespace RepoReadmeRewriter.Messages
{
    [ExcludeFromCodeCoverage]
    internal sealed class MessageProvider : IMessageProvider
    {
        public string CannotFindGitRepository() => "Cannot find Git repository";

        public string CouldNotParseRepositoryUrl(string? repositoryUrl)
            => $"Could not parse the repository url :{repositoryUrl}";

        public string MissingReadmeAsset(string missingReadmeAsset) => $"Missing readme asset - {missingReadmeAsset}";

        public string ReadmeFileDoesNotExist(string readmeFilePath) => $"Readme file does not exist at '{readmeFilePath}'";

        public string ReadmeHasUnsupportedHTML() => "Readme has unsupported HTML";

        public string UnsupportedImageDomain(string imageDomain) => $"Unsupported image domain found in README: {imageDomain}";
    }
}
