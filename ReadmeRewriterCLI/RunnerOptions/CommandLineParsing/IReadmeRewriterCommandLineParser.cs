using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing
{
    internal interface IReadmeRewriterCommandLineParser
    {
        (IEnumerable<string>? errors, ReadmeRewriterParseResult? result, IArgumentsOptionsInfo? helpOutput) Parse(IReadOnlyList<string> args);
    }
}
