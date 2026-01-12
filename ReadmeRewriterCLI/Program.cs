using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ReadmeRewriterCLI.ConsoleWriting;
using ReadmeRewriterCLI.RunnerOptions;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing;
using ReadmeRewriterCLI.RunnerOptions.Config;
using ReadmeRewriterCLI.RunnerOptions.Git;
using ReadmeRewriterCLI.RunnerOptions.RemoveReplace;
using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.Messages;
using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.Runner;
using Spectre.Console;

namespace ReadmeRewriterCLI
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args) => GetRunner().Run(args);

        private static Runner GetRunner()
        {
            IServiceCollection services = new ServiceCollection()
               .AddSingleton<IReadmeRewriterCommandLineParser, ReadmeRewriterCommandLineParser>()
               .AddSingleton<IIOHelper, IOHelper>()
               .AddSingleton<IConsoleWriter, SpectreConsoleWriter>()
               .AddSingleton((_) =>
               {
                   Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
                   return AnsiConsole.Console;
               })
               .AddSingleton<IOptionsProvider, OptionsProvider>()
               .AddSingleton<IConfigFileService, ConfigFileService>()
               .AddSingleton<IGitHelper, GitHelper>()
               .AddSingleton<IRemoveReplaceConfigLoader, RemoveReplaceConfigLoader>()
               .AddSingleton<IRemoveReplaceConfigDeserializer, RemoveReplaceConfigDeserializer>()
               .AddSingleton<IRemoveReplaceWordsParser, RemoveReplaceWordsParserWrapper>()
               .AddSingleton<IReadmeRewriterRunner, ReadmeRewriterRunner>()
               .AddSingleton<IImageDomainValidator, NoopImageDomainValidator>()
               .AddSingleton<IMessageProvider, MessageProvider>()
               .AddSingleton<Runner, Runner>();

            using ServiceProvider provider = services.BuildServiceProvider();
            return provider.GetRequiredService<Runner>();
        }
    }
}
