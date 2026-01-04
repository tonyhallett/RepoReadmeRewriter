using NugetRepoReadme.Processing;
using NugetRepoReadme.RemoveReplace.Settings;

namespace ReadmeRewriterCLI
{
    internal class Options(
        string projectDir,
        string repoUrl,
        string repoRef,
        string readmeRel,
        RewriteTagsOptions rewriteTagsOptions,
        RemoveReplaceSettings? removeReplaceSettings)
    {
        public string ProjectDir { get; } = projectDir;
        public string RepoUrl { get; } = repoUrl;
        public string RepoRef { get; } = repoRef;
        public string ReadmeRel { get; } = readmeRel;
        public RewriteTagsOptions RewriteTagsOptions { get; } = rewriteTagsOptions;
        public RemoveReplaceSettings? RemoveReplaceSettings { get; } = removeReplaceSettings;
    }
}
