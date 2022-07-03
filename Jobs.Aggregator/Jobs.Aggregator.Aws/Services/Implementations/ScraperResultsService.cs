using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Jobs.Aggregator.Aws.Configuration;
using Jobs.Aggregator.Aws.Services.Contracts;
using Microsoft.Extensions.Logging;

namespace Jobs.Aggregator.Aws.Services.Implementations
{
    public class ScraperResultsService : IScraperResultsService
    {
        private readonly ILogger<ScraperResultsService> _logger;
        private readonly IAwsConfigurationService _iawsConfigurationService;
        private readonly IS3Service _s3Service;

        public ScraperResultsService(ILogger<ScraperResultsService> logger, IS3Service s3Service,
            IAwsConfigurationService iawsConfigurationService)
        {
            _logger = logger;
            _s3Service = s3Service;
            _iawsConfigurationService = iawsConfigurationService;
        }


        public async Task<IEnumerable<Job>> GetAllScrapedJobs()
        {
            var objectList = await _s3Service.ListObjectsKeysAsync(_iawsConfigurationService.SourceDataBucketName);
            var enumerable = objectList.ToList();
            _logger.LogInformation("Number of files to download : {0}", enumerable.Count);
            var semaphore = new SemaphoreSlim(100);
            var test = enumerable.Select(async e =>
            {
                await semaphore.WaitAsync();

                try
                {
                    var jsonData =
                        await _s3Service.ReadObjectDataAsync(_iawsConfigurationService.SourceDataBucketName, e);
                    return JsonSerializer.Deserialize<Job[]>(jsonData);
                }
                finally
                {
                    semaphore.Release();
                }
            });
            var truc = await Task.WhenAll(test);
            _logger.LogInformation("Got the files");
            return truc.Where(e => e != null).SelectMany(e => e);
        }
    }
}