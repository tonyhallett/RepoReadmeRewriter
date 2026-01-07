namespace ReadmeRewriterCLI.RunnerOptions.Git
{
    internal interface IGitHelper
    {
        string BranchName(string repoRoot);

        string CommitSha(string repoRoot);

        string? FindGitRoot(string startDirectory);

        string ShortCommitSha(string repoRoot);

        string TagOrSha(string repoRoot);
    }
}
