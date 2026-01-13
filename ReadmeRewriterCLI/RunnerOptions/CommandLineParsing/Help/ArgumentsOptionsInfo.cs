using System.CommandLine;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal sealed class ArgumentsOptionsInfo(Command rootCommand) : IArgumentsOptionsInfo
    {
        public List<IArgumentInfo> Arguments { get; } = [.. rootCommand.Arguments.Where(a => !a.Hidden).Select(a => (IArgumentInfo)new ArgumentInfo(a))];

        public List<IOptionInfo> Options { get; } = [.. rootCommand.Options.Where(o => !o.Hidden).Select(o => (IOptionInfo)new OptionInfo(o))];
    }
}
