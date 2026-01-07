namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing
{
    internal interface IReadmeRewriterCommandLineParser
    {
        (IEnumerable<string>? errors, ReadmeRewriterParseResult? result) Parse(IReadOnlyList<string> args);
    }
}
