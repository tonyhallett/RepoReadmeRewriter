using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace ReadmeRewriterCLI.RunnerOptions
{
    internal sealed class Options(
        string projectDir,
        string repoUrl,
        string repoRef,
        string readmeRel,
        RewriteTagsOptions rewriteTagsOptions,
        RemoveReplaceSettings? removeReplaceSettings,
        string outputReadme)
    {
        public string ProjectDir { get; } = projectDir;
        public string RepoUrl { get; } = repoUrl;
        public string RepoRef { get; } = repoRef;
        public string ReadmeRel { get; } = readmeRel;
        public RewriteTagsOptions RewriteTagsOptions { get; } = rewriteTagsOptions;
        public RemoveReplaceSettings? RemoveReplaceSettings { get; } = removeReplaceSettings;
        public string OutputReadme { get; } = outputReadme;
    }
}
