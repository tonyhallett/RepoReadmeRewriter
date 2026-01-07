namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing
{
    internal sealed class ReadmeRewriterParseResult(
        string repoUrl,
        string readmeRelative,
        string? configPath,
        string? repoRef,
        GitRefKind gitRefKind,
        string projectDir,
        string outputReadme,
        bool errorOnHtml,
        bool removeHtml,
        bool extractDetailsSummary
    )
    {
        public string RepoUrl { get; } = repoUrl;

        public string ReadmeRelative { get; } = readmeRelative;

        public string? ConfigPath { get; } = configPath;

        public string? RepoRef { get; } = repoRef;

        public GitRefKind GitRefKind { get; } = gitRefKind;

        public string ProjectDir { get; } = projectDir;

        public string OutputReadme { get; } = outputReadme;

        public bool ErrorOnHtml { get; } = errorOnHtml;

        public bool RemoveHtml { get; } = removeHtml;

        public bool ExtractDetailsSummary { get; } = extractDetailsSummary;
    }
}
