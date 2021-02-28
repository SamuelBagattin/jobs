namespace Jobs.Aggregator.Aws.Configuration
{
    public interface IS3ConfigurationService
    {
        string SourceDataBucketName { get; }
        string DestinationBucketName { get; }
        string DestinationFileKey { get; }

        string UploadResults { get; }

        string WriteResultsToLocal { get; }
    }
}