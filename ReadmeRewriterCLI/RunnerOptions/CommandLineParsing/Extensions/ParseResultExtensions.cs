using System.CommandLine;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Extensions
{
    internal static class ParseResultExtensions
    {
        public static string GetRequiredStringValue(this ParseResult parseResult, DefinedStringOption option) => parseResult.GetValue(option)!;
    }
}
