namespace RepoReadmeRewriter.Processing
{
    public sealed class NoopImageDomainValidator : IImageDomainValidator
    {
        public bool IsTrustedImageDomain(string uriString) => true;
    }
}
