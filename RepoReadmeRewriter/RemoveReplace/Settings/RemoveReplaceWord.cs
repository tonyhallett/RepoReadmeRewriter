namespace NugetRepoReadme.RemoveReplace.Settings
{
    public class RemoveReplaceWord(string word, string? replacement) : IEquatable<RemoveReplaceWord>
    {
        public string Word { get; } = word;

        public string? Replacement { get; } = replacement;

        public bool Equals(RemoveReplaceWord? other) => other != null &&
               Word == other.Word &&
               Replacement == other.Replacement;

        public override bool Equals(object? obj) => obj is RemoveReplaceWord word && Equals(word);

        public override int GetHashCode() => HashCode.Combine(Word, Replacement);
    }
}
