namespace ReadmeRewriterCLI
{
    internal sealed class ReadmeRewriterParseResult
    {
        public ReadmeRewriterParseResult(IEnumerable<string> errors) => Errors = errors;

        public ReadmeRewriterParseResult(
            string repoUrl, string? readmeRelative, string? rewriteTags, string? configPath, string? repoRef)
        {
            RepoUrl = repoUrl;
            ReadmeRelative = readmeRelative;
            RewriteTags = rewriteTags;
            ConfigPath = configPath;
            RepoRef = repoRef;
        }

        public string? RepoUrl { get; }
        public string? ReadmeRelative { get; }
        public string? RewriteTags { get; }
        public string? ConfigPath { get; }
        public string? RepoRef { get; }
        public IEnumerable<string>? Errors { get; }
    }
}
