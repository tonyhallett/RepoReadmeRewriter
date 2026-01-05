using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace RepoReadmeRewriter.Runner
{
    internal interface IReadmeRewriterRunner
    {
        ReadmeRewriterRunnerResult Run(
            string projectDirectoryPath,
            string? readmeRelativePath,
            string? repositoryUrl,
            string? repositoryRef,
            RewriteTagsOptions rewriteTagsOptions,
            RemoveReplaceSettings? removeReplaceSettings);
    }
}
