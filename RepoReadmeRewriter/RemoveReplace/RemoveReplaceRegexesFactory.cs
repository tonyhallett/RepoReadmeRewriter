using NugetRepoReadme.RemoveReplace.Settings;

namespace NugetRepoReadme.RemoveReplace
{
    internal sealed class RemoveReplaceRegexesFactory : IRemoveReplaceRegexesFactory
    {
        public IRemoveReplaceRegexes Create(RemoveReplaceSettings removeReplaceSettings)
        {
            RemoveCommentRegexes? removeCommentRegexes = null;
            if (removeReplaceSettings.RemoveCommentIdentifiers != null)
            {
                removeCommentRegexes = RemoveCommentRegexes.Create(removeReplaceSettings.RemoveCommentIdentifiers);
            }

            List<RegexRemovalOrReplacement> regexRemovalOrReplacements = removeReplaceSettings.RemovalsOrReplacements.ConvertAll(RegexRemovalOrReplacement.Create);
            return new RemoveReplaceRegexes(regexRemovalOrReplacements, removeCommentRegexes, removeReplaceSettings.RemoveReplaceWords.ConvertAll(rrw => new RemoveReplaceWordRegex(rrw.Word, rrw.Replacement)));
        }
    }
}
