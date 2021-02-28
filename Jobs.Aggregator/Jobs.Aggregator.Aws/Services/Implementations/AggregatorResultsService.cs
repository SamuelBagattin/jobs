using System.Text.Json;
using System.Threading.Tasks;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Core.FinalModels;
using Microsoft.Extensions.Options;

namespace Jobs.Aggregator.Aws.Services.Implementations
{
    public class AggregatorResultsService : IAggregatorResultsService
    {
        private readonly S3ConfigurationService _s3ConfigurationService;
        private readonly IS3Service _s3Service;

        public AggregatorResultsService(IOptions<S3ConfigurationService> s3Options, IS3Service s3Service)
        {
            _s3ConfigurationService = s3Options.Value;
            _s3Service = s3Service;
        }

        public Task UploadAggregatedJobs(Response body)
        {
            return _s3Service.PutObjectAsync(_s3ConfigurationService.DestinationBucketName,
                _s3ConfigurationService.DestinationFileKey, JsonSerializer.Serialize(body));
        }
    }
}