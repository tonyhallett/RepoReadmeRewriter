using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing;

namespace ReadmeRewriterCLI.RunnerOptions
{
    internal interface IOptionsProvider
    {
        (Options? options, IEnumerable<string>? errors) Provide(ReadmeRewriterParseResult parseResult);
    }
}
