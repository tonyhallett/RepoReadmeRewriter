using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ReadmeRewriterCLI
{
    [ExcludeFromCodeCoverage]
    internal sealed class GitHelper : IGitHelper
    {
        /// <summary>
        /// Equivalent to MSBuild SourceRevisionId
        /// </summary>
        public string CommitSha(string repoRoot) => RunGit("rev-parse HEAD", repoRoot);

        public string ShortCommitSha(string repoRoot) => RunGit("rev-parse --short HEAD", repoRoot);

        public string BranchName(string repoRoot) => RunGit("rev-parse --abbrev-ref HEAD", repoRoot);

        /// <summary>
        /// Preferred identifier for README pinning:
        /// tag (exact match) → full SHA
        /// </summary>
        public string TagOrSha(string repoRoot) => TryRunGit("describe --tags --exact-match", repoRoot) ?? CommitSha(repoRoot);

        private static string RunGit(string arguments, string _repoRoot)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "git",
                Arguments = arguments,
                WorkingDirectory = _repoRoot,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using Process process = Process.Start(psi) ?? throw new InvalidOperationException("Failed to start git process.");
            string stdout = process.StandardOutput.ReadToEnd();
            string stderr = process.StandardError.ReadToEnd();

            process.WaitForExit();

            return process.ExitCode != 0
                ? throw new InvalidOperationException(
                    $"git {arguments} failed (exit {process.ExitCode}): {stderr.Trim()}")
                : stdout.Trim();
        }

        private static string? TryRunGit(string arguments, string repoRoot)
        {
            try
            {
                return RunGit(arguments, repoRoot);
            }
            catch
            {
                return null;
            }
        }

        public string? FindGitRoot(string startDirectory)
        {
            var dir = new DirectoryInfo(startDirectory);

            while (dir != null)
            {
                if (Directory.Exists(Path.Combine(dir.FullName, ".git")))
                    return dir.FullName;

                dir = dir.Parent;
            }

            return null;
        }
    }
}
