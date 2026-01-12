using System.CommandLine;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal sealed class HelpArgument<T>(string name, string defaultValue) : Argument<T>(name), IHelpDefaultValue
    {
        public string DefaultValue { get; } = defaultValue;
    }
}
