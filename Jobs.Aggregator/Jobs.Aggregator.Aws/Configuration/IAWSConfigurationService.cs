namespace Jobs.Aggregator.Aws.Configuration
{
    public interface IAwsConfigurationService
    {
        string SourceDataBucketName { get; }
        string DestinationBucketName { get; }
        string DestinationFileKey { get; }
        string DestinationCloudfrontDistributionId { get; }
        bool UploadResults { get; }

        bool WriteResultsToLocal { get; }
    }
}