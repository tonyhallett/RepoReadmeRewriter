using System.CommandLine.Parsing;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help;

namespace CLITests
{
    internal sealed class ReadmeRewriterCommandLineParser_Tests
    {
        [Test]
        public void Should_Not_Have_Version_Option()
        {
            var parser = new ReadmeRewriterCommandLineParser();
            Assert.That(() => parser.Parse(["--version"]), Throws.Nothing);
        }

        [Test]
        public void Should_Support_Help()
        {
            var parser = new ReadmeRewriterCommandLineParser();
            parser.SetRefKindAutoBehaviour("auto behaviour");
            (IEnumerable<string>? errors, ReadmeRewriterParseResult? result, IArgumentsOptionsInfo? helpOutput) = parser.Parse(["--help"]);
            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Null);
                Assert.That(result, Is.Null);

                IArgumentInfo helpArgument = helpOutput!.Arguments.Single();
                Assert.That(helpArgument.DefaultValue, Is.EqualTo("Environment.CurrentDirectory"));

                List<IOptionInfo> options = helpOutput.Options;
                // includes readme ( version removed )
                Assert.That(options, Has.Count.EqualTo(10));

                ShouldNotHaveCompletionLinesForBooleanOptions();
                ShouldHaveRequiredOptions();
                AssertGitRefKindOptionProperties();

                void ShouldNotHaveCompletionLinesForBooleanOptions()
                {
                    var booleanOptions = options.Where(o => o.DefaultValue is "False" or "True").ToList();
                    booleanOptions.ForEach(o => Assert.That(o.CompletionLines, Is.Empty));
                }

                void ShouldHaveRequiredOptions() => Assert.That(options.Where(o => o.Required).Select(o => o.Name), Is.EqualTo(new[]
                {
                    ReadmeRewriterCommandLineParser.s_repoUrlOption.Name,
                    ReadmeRewriterCommandLineParser.s_outputReadmeOption.Name
                }));

                void AssertGitRefKindOptionProperties()
                {
                    IOptionInfo gitRefKindOption = options.First(o => o.Name == ReadmeRewriterCommandLineParser.s_gitRefKindOptionName);
                    Assert.That(gitRefKindOption.CompletionLines, Has.Count.EqualTo(ReadmeRewriterCommandLineParser.s_gitRefKindLookup.Count));
                    Assert.That(gitRefKindOption.DefaultValue, Is.EqualTo(GitRefKind.Auto.ToString()));
                    Assert.That(gitRefKindOption.Description, Is.EqualTo("Resolve ref using git. Defaults to auto behaviour"));
                }
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
