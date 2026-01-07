using System.CommandLine;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Extensions
{
    internal static class ParseResultExtensions
    {
        public static string GetDefinedStringOptionValue(this ParseResult parseResult, DefinedStringOption option) => parseResult.GetValue(option)!;
    }
}
