using System.CommandLine;
using System.CommandLine.Completions;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Extensions
{
    internal sealed class EnumLookUpOption<T> : Option<T> where T : Enum
    {
        private readonly Dictionary<T, string[]> _lookup;

        public EnumLookUpOption(
            string name,
            Dictionary<T, string[]> lookup,
            params string[] aliases) : base(name, aliases
        )
        {
            DefaultValueFactory = (_) => default!;
            CustomParser = (argumentResult) =>
            {
                if (argumentResult.Tokens.Count == 0)
                {
                    return default;
                }

                string value = argumentResult.Tokens[0].Value.ToLowerInvariant();

                foreach (KeyValuePair<T, string[]> de in lookup)
                {
                    foreach (string allowedValue in de.Value)
                    {
                        if (allowedValue == value)
                        {
                            return de.Key;
                        }
                    }
                }

                // if Build() is used, this should never be hit
                argumentResult.AddError($"Invalid value '{value}' for {Name}");
                return default;
            };
            _lookup = lookup;
        }

        public Option<T> Build()
        {
            Option<T> configured = AcceptOnlyFromAmong([.. _lookup.Values.SelectMany(v => v)]);
            CompletionSources.Clear();
            IEnumerable<CompletionItem> completionItems = _lookup.SelectMany(
                kvp => kvp.Value.Select(value => new CompletionItem(
                    value,
                    kind: kvp.Key.ToString(),
                    sortText: $"{kvp.Key}:{value}")));
            CompletionSources.Add(ctx => completionItems);
            return configured;
        }
    }
}
