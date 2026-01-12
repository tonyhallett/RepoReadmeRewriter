using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.Messages;
using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.RemoveReplace.Settings;
using RepoReadmeRewriter.Repo;
using RepoReadmeRewriter.Rewriter;

namespace RepoReadmeRewriter.Runner
{
    public class ReadmeRewriterRunner : IReadmeRewriterRunner
    {
        private readonly IIOHelper _ioHelper;
        private readonly IRepoReadmeFilePathsProvider _repoReadmeFilePathsProvider;
        private readonly IReadmeRewriter _readmeRewriter;
        private readonly IMessageProvider _messageProvider;

        public ReadmeRewriterRunner(
            IImageDomainValidator imageDomainValidator,
            IMessageProvider messageProvider)
        : this(
            IOHelper.Instance,
            new RepoReadmeFilePathsProvider(),
            new ReadmeRewriter(imageDomainValidator),
            messageProvider)
            {
            }

        internal ReadmeRewriterRunner(
            IIOHelper ioHelper,
            IRepoReadmeFilePathsProvider repoReadmeFilePathsProvider,
            IReadmeRewriter readmeRewriter,
            IMessageProvider messageProvider)
        {
            _ioHelper = ioHelper;
            _repoReadmeFilePathsProvider = repoReadmeFilePathsProvider;
            _readmeRewriter = readmeRewriter;
            _messageProvider = messageProvider;
        }

        public ReadmeRewriterRunnerResult Run(
            string projectDirectoryPath,
            string? readmeRelativePath,
            string? repositoryUrl,
            string? repositoryRef,
            RewriteTagsOptions rewriteTagsOptions,
            RemoveReplaceSettings? removeReplaceSettings)
        {
            var result = new ReadmeRewriterRunnerResult();
            string readmePath = _ioHelper.CombinePaths(projectDirectoryPath, readmeRelativePath ?? "readme.md");

            if (!_ioHelper.FileExists(readmePath))
            {
                result.Errors.Add(_messageProvider.ReadmeFileDoesNotExist(readmePath));
                return result;
            }

            RepoReadmeFilePaths? repoReadmeFilePaths = _repoReadmeFilePathsProvider.Provide(readmePath);
            if (repoReadmeFilePaths == null)
            {
                result.Errors.Add(_messageProvider.CannotFindGitRepository());
                return result;
            }

            string readmeContents = _ioHelper.ReadAllText(readmePath);

            var readmeRelativeFileExists = new ReadmeRelativeFileExists(
                repoReadmeFilePaths.RepoDirectoryPath,
                repoReadmeFilePaths.ReadmeDirectoryPath);

            ReadmeRewriterResult rewriterResult = _readmeRewriter.Rewrite(
                rewriteTagsOptions,
                readmeContents,
                repoReadmeFilePaths.RepoRelativeReadmeFilePath,
                repositoryUrl,
                repositoryRef ?? "master",
                removeReplaceSettings,
                readmeRelativeFileExists);

            foreach (string unsupportedImageDomain in rewriterResult.UnsupportedImageDomains)
            {
                result.Errors.Add(_messageProvider.UnsupportedImageDomain(unsupportedImageDomain));
            }

            foreach (string missingReadmeAsset in rewriterResult.MissingReadmeAssets)
            {
                result.Errors.Add(_messageProvider.MissingReadmeAsset(missingReadmeAsset));
            }

            if (rewriterResult.HasUnsupportedHTML)
            {
                result.Errors.Add(_messageProvider.ReadmeHasUnsupportedHTML());
            }

            if (rewriterResult.UnsupportedRepo)
            {
                result.Errors.Add(_messageProvider.CouldNotParseRepositoryUrl(repositoryUrl));
            }

            if (result.Errors.Count == 0)
            {
                result.OutputReadme = rewriterResult.RewrittenReadme;
            }

            return result;
        }
    }
}
