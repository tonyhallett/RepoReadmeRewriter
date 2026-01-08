using System.CommandLine;
using System.CommandLine.Completions;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal sealed class OptionInfo : IOptionInfo
    {
        public bool Required { get; }

        public string Name { get; }

        public string? DefaultValue { get; }

        public string? Description { get; }

        public ICollection<string> Aliases { get; }

        public List<string>? Completions { get; }

        public OptionInfo(Option option)
        {
            // could create custom interface type to support help if needed.  ICustomizableHelpOption => GetOptionInfo
            Required = option.Required;
            // System.CommandLine - HelpName => <{HelpName}>
            Name = option.Name;
            if (option.HasDefaultValue)
            {
                DefaultValue = option.GetDefaultValue()!.ToString();
            }

            Description = option.Description;

            Aliases = option.Aliases;
            Type valueType = option.ValueType;
            if (valueType == typeof(bool) ||
                valueType == typeof(bool?) ||
                option.Arity.MaximumNumberOfValues <= 0)
            {
                return; // todo
            }

            Completions = [.. option
					.GetCompletions(CompletionContext.Empty)
					.Select(item => item.Label)];
        }
    }
}
