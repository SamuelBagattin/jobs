using System;
using Jobs.Aggregator.Aws.Exceptions;
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

        public string SourceDataBucketName => Environment.GetEnvironmentVariable("SOURCE_DATA_BUCKET_NAME") ?? throw new MissingEnvironmentVariableException("SOURCE_DATA_BUCKET_NAME");
        public string DestinationBucketName => Environment.GetEnvironmentVariable("DESTINATION_DATA_BUCKET_NAME") ?? throw new MissingEnvironmentVariableException("DESTINATION_DATA_BUCKET_NAME");
        public string DestinationFileKey => _configuration["aws:s3:destination_file_key"];

        public string UploadResults => _configuration["aws:s3:upload_results"];

        public string WriteResultsToLocal => _configuration["aws:s3:write_results_to_local"];
    }
}