using System.CommandLine.Parsing;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help;

namespace CLITests
{
    internal sealed class ReadmeRewriterCommandLineParser_Tests
    {
        [Test]
        public void Should_Support_Help()
        {
            (IEnumerable<string>? errors, ReadmeRewriterParseResult? result, IArgumentsOptionsInfo? helpOutput) = new ReadmeRewriterCommandLineParser().Parse(["--help"]);
            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Null);
                Assert.That(result, Is.Null);
                Assert.That(helpOutput, Is.Not.Null);
            });
        }

        [Test]
        public void Should_Have_Errors_When_Do_Not_Supply_Required_Options()
        {
            (IEnumerable<string>? errors, ReadmeRewriterParseResult? result, IArgumentsOptionsInfo? helpOutput) = new ReadmeRewriterCommandLineParser().Parse([]);

            List<string> errorList = [.. errors!];
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(helpOutput, Is.Null);
                Assert.That(errorList, Has.Count.EqualTo(2));
                Assert.That(errorList.Any(e => e.Contains(ReadmeRewriterCommandLineParser.s_repoUrlOption.Name)), Is.True);
                Assert.That(errorList.Any(e => e.Contains(ReadmeRewriterCommandLineParser.s_outputReadmeOption.Name)), Is.True);
            });
        }

        [Test]
        public void Should_Have_Correct_Default_Values_When_Only_Required_Options_Supplied()
        {
            (IEnumerable<string>? errors, ReadmeRewriterParseResult? result, IArgumentsOptionsInfo? helpOutput) = new ReadmeRewriterCommandLineParser().Parse(ArgsWithRequired(""));

            Assert.Multiple(() =>
            {
                Assert.That(helpOutput, Is.Null);
                Assert.That(errors, Is.Null);
                if (result is null)
                {
                    Assert.Fail("Expected result to be non-null");
                    return;
                }

                Assert.That(result.ErrorOnHtml, Is.False);
                Assert.That(result.RemoveHtml, Is.False);
                Assert.That(result.ExtractDetailsSummary, Is.False);
                Assert.That(result.ReadmeRelative, Is.EqualTo("README.md"));
                Assert.That(result.ProjectDir, Is.EqualTo(Environment.CurrentDirectory));
                Assert.That(result.ConfigPath, Is.Null);
                Assert.That(result.RepoRef, Is.Null);
                Assert.That(result.GitRefKind, Is.EqualTo(GitRefKind.Auto));
            });
        }

        [TestCaseSource(nameof(GitRefKindCases))]
        public void Should_Parse_GitRefKind_When_Specified(string gh, GitRefKind expectedRefKind)
        {
            (IEnumerable<string>? errors, ReadmeRewriterParseResult? result, IArgumentsOptionsInfo? helpOutput) = GhOptionTest(gh);

            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Null);
                Assert.That(result!.GitRefKind, Is.EqualTo(expectedRefKind));
            });
        }

        private static IEnumerable<TestCaseData> GitRefKindCases => ReadmeRewriterCommandLineParser.s_gitRefKindLookup
            .SelectMany(de => de.Value.Select(v => new TestCaseData(v, de.Key)));

        [Test]
        public void Should_Error_For_Unknown_Gh_Argument()
        {
            (IEnumerable<string>? errors, ReadmeRewriterParseResult? result, IArgumentsOptionsInfo? helpOutput) = GhOptionTest("bad");

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(errors!.Single(), Does.StartWith("Argument 'bad' not recognized. Must be one of"));
            });
        }

        private static (IEnumerable<string>? errors, ReadmeRewriterParseResult? result, IArgumentsOptionsInfo? helpOutput) GhOptionTest(string ghArg)
            => new ReadmeRewriterCommandLineParser().Parse(ArgsWithRequired($"--gh {ghArg}"));

        private static IReadOnlyList<string> ArgsWithRequired(string other)
        {
            const string repoUrl = "http:www.example.com/repo.git";
            const string outputReadme = "output/README.md";
            return [.. CommandLineParser.SplitCommandLine($"{ReadmeRewriterCommandLineParser.s_repoUrlOption.Name} {repoUrl} {ReadmeRewriterCommandLineParser.s_outputReadmeOption.Name} {outputReadme} {other}")];
        }
    }
}
