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
        private readonly IS3ConfigurationService _s3ConfigurationService;
        private readonly IS3Service _s3Service;

        public AggregatorResultsService(IS3ConfigurationService s3ConfigurationService, IS3Service s3Service)
        {
            _s3ConfigurationService = s3ConfigurationService;
            _s3Service = s3Service;
        }

        public Task UploadAggregatedJobs(ResponseRoot body)
        {
            return _s3Service.PutObjectAsync(_s3ConfigurationService.DestinationBucketName,
                _s3ConfigurationService.DestinationFileKey, JsonSerializer.Serialize(body));
        }
    }
}