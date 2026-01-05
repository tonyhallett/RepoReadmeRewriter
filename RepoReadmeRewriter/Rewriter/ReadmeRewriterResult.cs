namespace NugetRepoReadme.Rewriter
{
    internal sealed class ReadmeRewriterResult(
        string? rewrittenReadme,
        IEnumerable<string> unsupportedImageDomains,
        IEnumerable<string> missingReadmeAssets,
        bool hasUnsupportedHTML,
        bool unsupportedRepo)
    {
        public string? RewrittenReadme { get; } = rewrittenReadme;

        public IEnumerable<string> UnsupportedImageDomains { get; } = unsupportedImageDomains;

        public IEnumerable<string> MissingReadmeAssets { get; } = missingReadmeAssets;

        public bool HasUnsupportedHTML { get; } = hasUnsupportedHTML;

        public bool UnsupportedRepo { get; } = unsupportedRepo;
    }
}
