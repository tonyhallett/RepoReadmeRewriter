using System.Text;

namespace NugetRepoReadme.RemoveReplace
{
    internal sealed class LineBuilder
    {
        private readonly StringBuilder _sb = new();

        public void AppendLine(string line, bool isLast)
        {
            _ = _sb.Append(line);
            if (isLast)
            {
                return;
            }

            _ = _sb.Append(Environment.NewLine);
        }

        public override string ToString() => _sb.ToString();
    }
}
