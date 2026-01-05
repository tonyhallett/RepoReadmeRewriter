namespace RepoReadmeRewriter.Processing
{
    internal interface IImageDomainValidator
    {
        bool IsTrustedImageDomain(string uriString);
    }
}
