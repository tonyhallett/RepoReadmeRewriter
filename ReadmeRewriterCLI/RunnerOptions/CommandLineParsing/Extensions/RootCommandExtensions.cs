using System.CommandLine;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Extensions
{
    internal static class RootCommandExtensions
    {
        public static RootCommand RemoveVersionOption(this RootCommand command)
        {
            Option? versionOption = command.Options.FirstOrDefault(o => o.Name == "--version");
            if (versionOption != null)
            {
                _ = command.Options.Remove(versionOption);
            }
            return command;
        }
    }
}
