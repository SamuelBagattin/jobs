using System;
using Jobs.Aggregator.Aws.Exceptions;
using Microsoft.Extensions.Configuration;

namespace Jobs.Aggregator.Aws.Configuration
{
    public class AwsConfigurationService : IAwsConfigurationService
    {
        private readonly IConfiguration _configuration;

        public AwsConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string SourceDataBucketName => Environment.GetEnvironmentVariable("SOURCE_DATA_BUCKET_NAME") ?? throw new MissingEnvironmentVariableException("SOURCE_DATA_BUCKET_NAME");
        public string DestinationBucketName => Environment.GetEnvironmentVariable("DESTINATION_DATA_BUCKET_NAME") ?? throw new MissingEnvironmentVariableException("DESTINATION_DATA_BUCKET_NAME");
        
        private static bool OnLambda
        {
            get
            {
                var envVar = Environment.GetEnvironmentVariable("ON_LAMBDA");
                var isParsed = bool.TryParse(envVar, out var result);
                return isParsed && result;
            }
        }

        public string DestinationFileKey => _configuration["aws:s3:destination_file_key"];

        public string DestinationCloudfrontDistributionId =>
            Environment.GetEnvironmentVariable("DESTINATION_DATA_DISTRIBUTION_ID") ??
            throw new MissingEnvironmentVariableException("DESTINATION_DATA_DISTRIBUTION_ID");

        public bool UploadResults => OnLambda;

        public bool WriteResultsToLocal => !OnLambda;
    }
}