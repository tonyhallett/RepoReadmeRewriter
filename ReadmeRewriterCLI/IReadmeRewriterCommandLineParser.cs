namespace ReadmeRewriterCLI
{
    internal interface IReadmeRewriterCommandLineParser
    {
        (IEnumerable<string>? errors, ReadmeRewriterParseResult? result) Parse(IReadOnlyList<string> args);
    }
}
