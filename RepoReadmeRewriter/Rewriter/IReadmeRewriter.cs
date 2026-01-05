using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace RepoReadmeRewriter.Rewriter
{
    internal interface IReadmeRewriter
    {
        ReadmeRewriterResult Rewrite(
            RewriteTagsOptions rewriteTagsOptions,
            string readme,
            string repoReadmeRelativeFilePath,
            string? repoUrl,
            string @ref,
            RemoveReplaceSettings? removeReplaceSettings,
            IReadmeRelativeFileExists readmeRelativeFileExists);
    }
}
