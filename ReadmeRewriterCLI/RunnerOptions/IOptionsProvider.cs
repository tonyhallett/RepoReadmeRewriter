using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing;

namespace ReadmeRewriterCLI.RunnerOptions
{
    internal interface IOptionsProvider
    {
        string RefKindAutoBehaviour { get; }

        (Options? options, IEnumerable<string>? errors) Provide(ReadmeRewriterParseResult parseResult);
    }
}
