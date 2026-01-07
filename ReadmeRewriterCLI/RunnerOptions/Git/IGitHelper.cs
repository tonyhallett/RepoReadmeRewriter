namespace ReadmeRewriterCLI.RunnerOptions.Git
{
    internal interface IGitHelper
    {
        string? FindGitRoot(string startDirectory);

        string BranchName(string repoRoot);

        string CommitSha(string repoRoot);

        string ShortCommitSha(string repoRoot);

        string TagOrSha(string repoRoot);
    }
}
