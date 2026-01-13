using System.CommandLine;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal sealed class ArgumentInfo : IArgumentInfo
    {
        public string Name { get; }

        public string? DefaultValue { get; }

        public string? Description { get; }

        public ArgumentInfo(Argument argument)
        {
            Name = argument.Name;
            if (argument.HasDefaultValue)
            {
                DefaultValue = argument is IHelpDefaultValue helpDefaultValue ? helpDefaultValue.DefaultValue : argument.GetDefaultValue()!.ToString();
            }

            Description = argument.Description;
        }
    }
}
