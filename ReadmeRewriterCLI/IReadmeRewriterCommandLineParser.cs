namespace ReadmeRewriterCLI
{
    internal interface IReadmeRewriterCommandLineParser
    {
        ReadmeRewriterParseResult Parse(string[] args);
    }
}
