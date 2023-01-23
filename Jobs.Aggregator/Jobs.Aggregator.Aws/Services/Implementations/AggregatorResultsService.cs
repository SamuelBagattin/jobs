using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Core.FinalModels;

namespace Jobs.Aggregator.Aws.Services.Implementations
{
    public class AggregatorResultsService : IAggregatorResultsService
    {
        private readonly IAwsConfigurationService _awsConfigurationService;
        private readonly IS3Service _s3Service;
        private readonly ICloudfrontService _cloudfrontService;

        public AggregatorResultsService(IAwsConfigurationService awsConfigurationService, IS3Service s3Service,
            ICloudfrontService cloudfrontService)
        {
            _awsConfigurationService = awsConfigurationService;
            _s3Service = s3Service;
            _cloudfrontService = cloudfrontService;
        }

        public async Task UploadAggregatedJobs(ResponseRoot body, CancellationToken cancellationToken)
        {
            if (_awsConfigurationService.UploadResults)
            {
                 await Task.WhenAll(body.Companies.Companies.Select(e => _s3Service.PutJsonObjectAsync(
                    _awsConfigurationService.DestinationBucketName,
                    $"api/companies/{e.Id}", JsonSerializer.Serialize(e.Jobs), cancellationToken)));
                 await _s3Service.PutJsonObjectAsync(_awsConfigurationService.DestinationBucketName, "api/companies", JsonSerializer.Serialize(body.Companies.Companies.Select(e => new
                 {
                     e.Id,
                     e.CompanyName
                 })), cancellationToken);

                await _cloudfrontService.CreateInvalidationByPath(
                    _awsConfigurationService.DestinationCloudfrontDistributionId,
                    new []{"/*"}, cancellationToken);
            }

            if (_awsConfigurationService.WriteResultsToLocal)
            {
                Directory.CreateDirectory(_awsConfigurationService.LocalResultsPath);
                var semaphore = new SemaphoreSlim(100);
                var test = body.Companies.Companies.Select(async e =>
                {
                    await semaphore.WaitAsync(cancellationToken);

                    try
                    {
                        var currentLocalFilePath = Path.Combine(_awsConfigurationService.LocalResultsPath, e.Id);
                        await File.WriteAllTextAsync(currentLocalFilePath, JsonSerializer.Serialize(e), cancellationToken);
     
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                });
                await Task.WhenAll(test);
            }
        }

        public async Task<ResponseRoot> GetLastUploadedAggregatedJobs(CancellationToken cancellationToken)
        {
            var stringRes = await _s3Service.ReadObjectDataAsync(_awsConfigurationService.DestinationBucketName,
                _awsConfigurationService.DestinationFileKey, cancellationToken);
            return JsonSerializer.Deserialize<ResponseRoot>(stringRes);
        }
    }
}