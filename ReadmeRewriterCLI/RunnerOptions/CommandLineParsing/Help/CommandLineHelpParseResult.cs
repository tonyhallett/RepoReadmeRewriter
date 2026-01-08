using System.CommandLine;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal sealed class CommandLineHelpParseResult(ParseResult parseResult, bool helpInvoked)
    {
        public ParseResult ParseResult { get; } = parseResult;

        public bool HelpInvoked { get; } = helpInvoked;
    }
}
