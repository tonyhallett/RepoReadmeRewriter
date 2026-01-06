using System.CommandLine.Parsing;
using ReadmeRewriterCLI;

namespace CLITests
{
    internal sealed class ReadmeRewriterCommandLineParser_Tests
    {
        [Test]
        public void Should_Have_Errors_When_Do_Not_Supply_Required_Options()
        {
            (IEnumerable<string>? errors, ReadmeRewriterParseResult? result) = new ReadmeRewriterCommandLineParser().Parse([]);
            
            List<string> errorList = [.. errors!];
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Null);
                Assert.That(errorList, Has.Count.EqualTo(2));
                Assert.That(errorList.Any(e => e.Contains(ReadmeRewriterCommandLineParser.s_repoUrlOption.Name)), Is.True);
                Assert.That(errorList.Any(e => e.Contains(ReadmeRewriterCommandLineParser.s_outputReadmeOption.Name)), Is.True);
            });
        }

        [Test]
        public void Should_Have_Correct_Default_Values_When_Only_Required_Options_Supplied()
        {
            const string repoUrl = "http:www.example.com/repo.git";
            const string outputReadme = "output/README.md";
            IEnumerable<string> args = CommandLineParser.SplitCommandLine($"{ReadmeRewriterCommandLineParser.s_repoUrlOption.Name} {repoUrl} {ReadmeRewriterCommandLineParser.s_outputReadmeOption.Name} {outputReadme}");
            (IEnumerable<string>? errors, ReadmeRewriterParseResult? result) = new ReadmeRewriterCommandLineParser().Parse([.. args]);

            Assert.Multiple(() =>
            {
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
            });
        }
    }
}
