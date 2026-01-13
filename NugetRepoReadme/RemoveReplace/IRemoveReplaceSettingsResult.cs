using System.Collections.Generic;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace NugetRepoReadme.RemoveReplace
{
    internal interface IRemoveReplaceSettingsResult
    {
        IReadOnlyList<string> Errors { get; }

        RemoveReplaceSettings? Settings { get; }
    }
}
