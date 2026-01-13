namespace ReadmeRewriterCLI.RunnerOptions.RemoveReplace
{
    internal sealed class RemovalOrReplacementConfig
    {
        public string? CommentOrRegex { get; set; }

        public string? Start { get; set; }

        public string? End { get; set; }

        public string? ReplacementText { get; set; }

        public bool? ReplacementFromFile { get; set; }
    }
}
