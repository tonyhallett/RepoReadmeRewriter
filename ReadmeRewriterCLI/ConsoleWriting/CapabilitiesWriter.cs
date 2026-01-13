using Spectre.Console;

namespace ReadmeRewriterCLI.ConsoleWriting
{
    internal static class CapabilitiesWriter
    {
        public static void WriteCapabilities(IAnsiConsole ansiConsole)
        {
            Capabilities capabilities = ansiConsole.Profile.Capabilities;
            ansiConsole.WriteLine($"Legacy - {capabilities.Legacy}");
            ansiConsole.WriteLine($"Unicode - {capabilities.Unicode}");
            ansiConsole.WriteLine($"Ansi - {capabilities.Ansi}");
            ansiConsole.WriteLine($"ColorSystem - {capabilities.ColorSystem}");
            ansiConsole.WriteLine($"Links - {capabilities.Links}");
        }
    }
}
