namespace NugetRepoReadme.Repo
{
    internal sealed class RepoReadmeFilePathsProvider : IRepoReadmeFilePathsProvider
    {
        public RepoReadmeFilePaths? Provide(string readmePath)
        {
            List<string> parentDirectories = [];
            FileInfo readmeFile = new(readmePath);
            DirectoryInfo readmeDirectory = readmeFile.Directory!;
            string readmeDirectoryPath = readmeDirectory.FullName;
            DirectoryInfo? searchDirectory = readmeDirectory;
            while (searchDirectory != null)
            {
                var gitDirectory = new DirectoryInfo(Path.Combine(searchDirectory.FullName, ".git"));
                if (gitDirectory.Exists)
                {
                    break;
                }

                parentDirectories.Add(searchDirectory.Name);
                searchDirectory = searchDirectory.Parent;
            }

            if (searchDirectory == null)
            {
                return null;
            }

            parentDirectories.Reverse();
            string repoRelativeReadmePath = GetRepoRelativeReadmePath(parentDirectories, readmeFile.Name);
            return new RepoReadmeFilePaths(searchDirectory.FullName, readmeDirectoryPath, repoRelativeReadmePath);
        }

        public static string GetRepoRelativeReadmePath(List<string> parentDirectories, string readmeFileName)
        {
            string relativeReadme = Path.DirectorySeparatorChar + readmeFileName;
            return parentDirectories.Count == 0
                ? relativeReadme
                : Path.DirectorySeparatorChar + string.Join(Path.DirectorySeparatorChar.ToString(), parentDirectories) + relativeReadme;
        }
    }
}
