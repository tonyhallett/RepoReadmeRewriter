namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal interface IArgumentInfo
    {
        string Name { get; }

        string? DefaultValue { get; }

        string? Description { get; }
    }
}
