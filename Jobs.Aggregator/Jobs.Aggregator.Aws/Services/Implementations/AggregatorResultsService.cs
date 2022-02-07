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
        private readonly IAwsConfigurationService _iawsConfigurationService;
        private readonly IS3Service _s3Service;
        private readonly ICloudfrontService _cloudfrontService;

        public AggregatorResultsService(IAwsConfigurationService iawsConfigurationService, IS3Service s3Service, ICloudfrontService cloudfrontService)
        {
            _iawsConfigurationService = iawsConfigurationService;
            _s3Service = s3Service;
            _cloudfrontService = cloudfrontService;
        }

        public async Task UploadAggregatedJobs(ResponseRoot body)
        {
            await _s3Service.PutJsonObjectAsync(_iawsConfigurationService.DestinationBucketName,
                _iawsConfigurationService.DestinationFileKey, JsonSerializer.Serialize(body));
            await _cloudfrontService.CreateInvalidationByPath(_iawsConfigurationService.DestinationCloudfrontDistributionId, $"/{_iawsConfigurationService.DestinationFileKey}");
        }

        public async Task<ResponseRoot> GetLastUploadedAggregatedJobs()
        {
            var stringRes = await _s3Service.ReadObjectDataAsync(_iawsConfigurationService.DestinationBucketName,
                _iawsConfigurationService.DestinationFileKey);
            return JsonSerializer.Deserialize<ResponseRoot>(stringRes);
        }
    }
}