using System.CommandLine;
using System.CommandLine.Invocation;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help
{
    internal sealed class LoggingHelpAction : SynchronousCommandLineAction
    {
        public bool Invoked { get; private set; } = false;

        public override bool ClearsParseErrors => true;

        public override int Invoke(ParseResult parseResult)
        {
            Invoked = true;
            return 0;
        }
    }
}
