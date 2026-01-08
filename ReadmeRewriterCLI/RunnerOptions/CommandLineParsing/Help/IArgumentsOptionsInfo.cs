namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal interface IArgumentsOptionsInfo
    {
        List<IArgumentInfo> Arguments { get; }

        List<IOptionInfo> Options { get; }
    }
}
