using System.CommandLine;
using System.CommandLine.Parsing;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Extensions
{
    internal sealed class DefinedStringOption : Option<string>
    {
        private DefinedStringOption(string name, params string[] aliases)
        : base(name, aliases) {}

        public static DefinedStringOption CreateRequired(string name, params string[] aliases)
        {
            var option = new DefinedStringOption(name, aliases)
            {
                Required = true
            };
            return option;
        }

        public static DefinedStringOption CreateDefault(string name, Func<ArgumentResult, string> defaultValueFactory, params string[] aliases)
        {
            var option = new DefinedStringOption(name, aliases)
            {
                DefaultValueFactory = defaultValueFactory
            };
            return option;
        }
    }
}
