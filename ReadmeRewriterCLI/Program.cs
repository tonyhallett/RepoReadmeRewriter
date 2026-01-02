// See https://aka.ms/new-console-template for more information
using NugetRepoReadme.Processing;
using NugetRepoReadme.Runner;

var runner = new ReadmeRewriterRunner();
// testing
string dexieTypeSafeDir = "C:\\Users\\tonyh\\source\\repos\\dexie-typesafe";
string dexieTypeSafeRepoUrl = "https://github.com/tonyhallett/dexie-typesafe";
// repo ref todo
ReadmeRewriterRunnerResult result = runner.Run(dexieTypeSafeDir, null, dexieTypeSafeRepoUrl, null, RewriteTagsOptions.None, null);
File.WriteAllText(@"C:\Users\tonyh\Downloads\dexie.md", result.OutputReadme);
