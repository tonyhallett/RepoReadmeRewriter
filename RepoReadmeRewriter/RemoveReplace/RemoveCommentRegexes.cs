using System.Text.RegularExpressions;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace RepoReadmeRewriter.RemoveReplace
{
    internal sealed class RemoveCommentRegexes
    {
        public RemoveCommentRegexes(Regex startRegex, Regex? endRegex)
        {
            StartRegex = startRegex;
            EndRegex = endRegex;
        }

        public Regex StartRegex { get; }

        public Regex? EndRegex { get; }

        public static RemoveCommentRegexes Create(RemoveCommentIdentifiers removeCommentIdentifiers)
            => new(
                CreateRegex(removeCommentIdentifiers.Start, false),
                removeCommentIdentifiers.End != null ? CreateRegex(removeCommentIdentifiers.End, false) : null);

        public static Regex CreateRegex(string commentIdentifier, bool exact)
        {
            string end = exact ? @"\s*" : @"\b[^>]*";
            string pattern = @"<!--\s*" + Regex.Escape(commentIdentifier) + end + "-->";
            return new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
    }
}
