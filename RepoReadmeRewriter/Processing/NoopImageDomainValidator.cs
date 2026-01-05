namespace NugetRepoReadme.Processing
{
    internal sealed class NoopImageDomainValidator : IImageDomainValidator
    {
        public bool IsTrustedImageDomain(string uriString) => true;
    }
}
