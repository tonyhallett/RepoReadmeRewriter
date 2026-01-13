using System;

namespace RepoReadmeRewriter.RemoveReplace.Settings
{
    public class RemoveReplaceWord(string word, string? replacement) : IEquatable<RemoveReplaceWord>
    {
        public string Word { get; } = word;

        public string? Replacement { get; } = replacement;

        public bool Equals(RemoveReplaceWord? other) => other != null &&
               Word == other.Word &&
               Replacement == other.Replacement;

        public override bool Equals(object? obj) => obj is RemoveReplaceWord word && Equals(word);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + Word.GetHashCode();
                hash = (hash * 31) + (Replacement?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }
}
