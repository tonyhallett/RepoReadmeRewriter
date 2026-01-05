namespace NugetRepoReadme.Processing
{
    internal interface IImageDomainValidator
    {
        bool IsTrustedImageDomain(string uriString);
    }
}
