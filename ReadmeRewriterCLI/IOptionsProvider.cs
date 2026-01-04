namespace ReadmeRewriterCLI
{
    internal interface IOptionsProvider
    {
        (Options? options, IEnumerable<string>? errors) Provide(ReadmeRewriterParseResult parseResult);
    }
}
