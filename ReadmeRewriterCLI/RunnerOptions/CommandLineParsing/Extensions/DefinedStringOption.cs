using System.CommandLine;
using System.CommandLine.Parsing;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Extensions
{
    internal sealed class DefinedStringOption : Option<string>
    {
        private DefinedStringOption(string name, params string[] aliases)
        : base(name, aliases) { }

        public static DefinedStringOption CreateRequired(string name, params string[] aliases) => new(name, aliases) { Required = true };

        public static DefinedStringOption CreateDefault(string name, Func<ArgumentResult, string> defaultValueFactory, params string[] aliases)
            => new(name, aliases)
            {
                DefaultValueFactory = defaultValueFactory
            };
    }
}
