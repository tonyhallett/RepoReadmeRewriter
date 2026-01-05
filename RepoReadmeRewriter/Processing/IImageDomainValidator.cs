namespace RepoReadmeRewriter.Processing
{
    public interface IImageDomainValidator
    {
        bool IsTrustedImageDomain(string uriString);
    }
}
