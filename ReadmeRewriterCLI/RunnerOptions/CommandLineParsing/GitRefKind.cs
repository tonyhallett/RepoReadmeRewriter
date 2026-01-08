namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing
{
    internal enum GitRefKind
    {
        Auto,
        Tag,
        TagOrSha,
        CommitSha,
        ShortCommitSha,
        BranchName
    }
}
