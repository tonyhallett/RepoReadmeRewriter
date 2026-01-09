using System.CommandLine;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal class HelpArgument<T> : Argument<T>, IHelpDefaultValue
    {
        public HelpArgument(string name, string defaultValue) : base(name)
            => DefaultValue = defaultValue;

        public string DefaultValue { get; }
    }
}
