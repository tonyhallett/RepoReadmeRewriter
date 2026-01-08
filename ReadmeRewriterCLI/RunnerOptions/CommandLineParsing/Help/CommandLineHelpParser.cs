using System.CommandLine;
using System.CommandLine.Help;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal static class CommandLineHelpParser
    {
        private static readonly LoggingHelpAction s_loggingHelpAction = new();

        public static CommandLineHelpParseResult Parse(RootCommand rootCommand, IReadOnlyList<string> args)
        {
            ReplaceHelpOptionAction(rootCommand);
            ParseResult parseResult = rootCommand.Parse(args);
            return new CommandLineHelpParseResult(parseResult, parseResult.Errors.Count <= 0 && InvokeHelp(parseResult));
        }

        private static bool InvokeHelp(ParseResult parseResult)
        {
            _ = parseResult.Invoke();
           return s_loggingHelpAction.Invoked;
        }

        private static void ReplaceHelpOptionAction(RootCommand rootCommand)
          => rootCommand.Options.First(option => option is HelpOption).Action = s_loggingHelpAction;
    }
}
