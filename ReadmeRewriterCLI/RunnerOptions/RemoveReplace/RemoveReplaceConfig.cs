namespace ReadmeRewriterCLI.RunnerOptions.RemoveReplace
{
    internal sealed class RemoveReplaceConfig
    {
        public RemoveCommentIdentifiersConfig? RemoveCommentIdentifiers { get; set; }

        public List<RemovalOrReplacementConfig>? RemovalsOrReplacements { get; set; }

        public List<string>? RemoveReplaceWordsFilePaths { get; set; }
    }
}
