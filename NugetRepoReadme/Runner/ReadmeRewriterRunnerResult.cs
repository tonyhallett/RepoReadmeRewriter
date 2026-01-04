using System.Collections.Generic;

namespace NugetRepoReadme.Runner
{
    public class ReadmeRewriterRunnerResult
    {
        public List<string> Errors { get; } = new List<string>();

        public string? OutputReadme { get; set; }

        public bool Success => Errors.Count == 0;
    }
}
