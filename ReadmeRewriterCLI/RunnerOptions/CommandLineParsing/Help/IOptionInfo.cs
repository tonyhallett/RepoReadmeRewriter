namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal interface IOptionInfo
    {
        bool Required { get; }

        string Name { get; }

        string? DefaultValue { get; }

        string? Description { get; }

        ICollection<string> Aliases { get; }

        List<string>? Completions { get; }
    }
}
