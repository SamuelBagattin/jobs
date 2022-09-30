using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Amazon.Runtime.Internal.Util;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Services.Contracts;
using Jobs.Aggregator.Core.FinalModels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Jobs.Aggregator.Aws.Services.Implementations
{
    public class AggregatorResultsService : IAggregatorResultsService
    {
        private readonly IAwsConfigurationService _iawsConfigurationService;
        private readonly IS3Service _s3Service;
        private readonly ICloudfrontService _cloudfrontService;
        private readonly ILogger<AggregatorResultsService> _logger;

        public AggregatorResultsService(IAwsConfigurationService iawsConfigurationService, IS3Service s3Service,
            ICloudfrontService cloudfrontService, ILogger<AggregatorResultsService> logger)
        {
            _iawsConfigurationService = iawsConfigurationService;
            _s3Service = s3Service;
            _cloudfrontService = cloudfrontService;
            _logger = logger;
        }

        public async Task UploadAggregatedJobs(ResponseRoot body, CancellationToken cancellationToken)
        {
            if (_iawsConfigurationService.UploadResults)
            {
                 await Task.WhenAll(body.Companies.Companies.Select(e => _s3Service.PutJsonObjectAsync(
                    _iawsConfigurationService.DestinationBucketName,
                    $"api/companies/{e.Id}", JsonSerializer.Serialize(e.Jobs), cancellationToken)));
                 await _s3Service.PutJsonObjectAsync(_iawsConfigurationService.DestinationBucketName, "api/companies", JsonSerializer.Serialize(body.Companies.Companies.Select(e => new
                 {
                     e.Id,
                     e.CompanyName
                 })), cancellationToken);

                await _cloudfrontService.CreateInvalidationByPath(
                    _iawsConfigurationService.DestinationCloudfrontDistributionId,
                    new []{"/*"}, cancellationToken);
            }

            if (_iawsConfigurationService.WriteResultsToLocal)
            {
                Directory.CreateDirectory(_iawsConfigurationService.LocalResultsPath);
                var semaphore = new SemaphoreSlim(100);
                var test = body.Companies.Companies.Select(async e =>
                {
                    await semaphore.WaitAsync(cancellationToken);

                    try
                    {
                        var currentLocalFilePath = Path.Combine(_iawsConfigurationService.LocalResultsPath, e.Id);
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
            var stringRes = await _s3Service.ReadObjectDataAsync(_iawsConfigurationService.DestinationBucketName,
                _iawsConfigurationService.DestinationFileKey, cancellationToken);
            return JsonSerializer.Deserialize<ResponseRoot>(stringRes);
        }
    }
}