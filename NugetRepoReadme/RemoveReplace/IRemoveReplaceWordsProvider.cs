using System.Collections.Generic;
using Microsoft.Build.Framework;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace NugetRepoReadme.RemoveReplace
{
    internal interface IRemoveReplaceWordsProvider
    {
        List<RemoveReplaceWord> Provide(ITaskItem[]? removeReplaceWordsItems, IAddError addError);
    }
}
