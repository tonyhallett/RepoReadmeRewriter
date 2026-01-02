using System.Diagnostics.CodeAnalysis;

namespace NugetRepoReadme
{
    [ExcludeFromCodeCoverage]
    internal static class IsDebug
    {
        public static bool Value() =>
#if DEBUG
            true;
#else
            false;
#endif

    }
}
