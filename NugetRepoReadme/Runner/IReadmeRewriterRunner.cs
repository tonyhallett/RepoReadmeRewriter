using NugetRepoReadme.Processing;
using NugetRepoReadme.RemoveReplace.Settings;

namespace NugetRepoReadme.Runner
{
    internal interface IReadmeRewriterRunner
    {
        ReadmeRewriterRunnerResult Run(string projectDirectoryPath, string? readmeRelativePath, string? repositoryUrl, string? repositoryRef, RewriteTagsOptions rewriteTagsOptions, RemoveReplaceSettings? removeReplaceSettings);
    }
}