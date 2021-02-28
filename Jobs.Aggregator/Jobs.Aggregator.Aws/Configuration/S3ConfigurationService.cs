using Microsoft.Extensions.Configuration;

namespace Jobs.Aggregator.Aws.Configuration
{
    public class S3ConfigurationService : IS3ConfigurationService
    {
        private readonly IConfiguration _configuration;

        public S3ConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string SourceDataBucketName => _configuration["aws:s3:source_data_bucket_name"];
        public string DestinationBucketName => _configuration["aws:s3:destination_bucket_name"];
        public string DestinationFileKey => _configuration["aws:s3:destination_file_key"];

        public string UploadResults => _configuration["aws:s3:upload_results"];

        public string WriteResultsToLocal => _configuration["aws:s3:write_results_to_local"];
    }
}