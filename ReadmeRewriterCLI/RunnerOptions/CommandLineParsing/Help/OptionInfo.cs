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

        public List<string> CompletionLines { get; } = [];

        public OptionInfo(Option option)
        {
            Required = option.Required;
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
                return;
            }

            IEnumerable<IGrouping<string?, CompletionItem>> completionItemsKindGrouping = option.GetCompletions(CompletionContext.Empty).GroupBy(ci => ci.Kind);
            CompletionLines = [.. completionItemsKindGrouping.Select(g =>
                // sorting ?
                string.Join(",", g.Select(ci => ci.Label)))];
        }
    }
}
