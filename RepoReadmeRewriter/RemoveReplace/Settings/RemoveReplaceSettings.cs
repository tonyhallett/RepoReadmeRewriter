using System.Collections.Generic;

namespace RepoReadmeRewriter.RemoveReplace.Settings
{
    public class RemoveReplaceSettings(
        RemoveCommentIdentifiers? removeCommentIdentifiers,
        List<RemovalOrReplacement> removalsOrReplacements,
        List<RemoveReplaceWord> removeReplaceWords)
    {
        public RemoveCommentIdentifiers? RemoveCommentIdentifiers { get; } = removeCommentIdentifiers;

        public List<RemovalOrReplacement> RemovalsOrReplacements { get; } = removalsOrReplacements;

        public List<RemoveReplaceWord> RemoveReplaceWords { get; } = removeReplaceWords;
    }
}
